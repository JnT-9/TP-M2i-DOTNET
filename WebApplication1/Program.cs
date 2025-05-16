using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Définir explicitement les URLs d'écoute
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Create database if it doesn't exist
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }
}

// Enable CORS
app.UseCors("AllowAll");

// Configure API endpoints
app.MapGet("/api/tasks", async (AppDbContext db, string? status) =>
{
    Console.WriteLine($"GET /api/tasks called with status: {status ?? "null"}");
    
    if (string.IsNullOrEmpty(status))
    {
        var allTasks = await db.Tasks.ToListAsync();
        Console.WriteLine($"Returning {allTasks.Count} tasks");
        return Results.Ok(allTasks);
    }
    
    if (!TaskConstants.Status.All.Contains(status))
    {
        Console.WriteLine($"Invalid status: {status}");
        return Results.BadRequest($"Invalid status. Must be one of: {string.Join(", ", TaskConstants.Status.All)}");
    }
    
    var filteredTasks = await db.Tasks.Where(t => t.Status == status).ToListAsync();
    Console.WriteLine($"Returning {filteredTasks.Count} tasks with status: {status}");
    return Results.Ok(filteredTasks);
})
.WithName("GetTasks");

app.MapGet("/api/tasks/{id}", async (AppDbContext db, int id) =>
{
    Console.WriteLine($"GET /api/tasks/{id} called");
    
    var task = await db.Tasks.FindAsync(id);
    
    if (task == null)
    {
        Console.WriteLine($"Task with ID {id} not found");
        return Results.NotFound();
    }
    
    Console.WriteLine($"Returning task with ID {id}");
    return Results.Ok(task);
})
.WithName("GetTaskById");

app.MapPost("/api/tasks", async (AppDbContext db, TaskInput input) =>
{
    Console.WriteLine($"POST /api/tasks called with title: {input.Title}");
    
    var task = new TaskItem
    {
        Title = input.Title,
        Description = input.Description,
        Priority = input.Priority,
        DueDate = input.DueDate,
        Status = TaskConstants.Status.Todo
    };
    
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    
    Console.WriteLine($"Created task with ID: {task.Id}");
    return Results.Created($"/api/tasks/{task.Id}", task);
})
.WithName("CreateTask");

app.MapPut("/api/tasks/{id}", async (AppDbContext db, int id, TaskInput input) =>
{
    Console.WriteLine($"PUT /api/tasks/{id} called");
    
    var task = await db.Tasks.FindAsync(id);
    
    if (task == null)
    {
        Console.WriteLine($"Task with ID {id} not found");
        return Results.NotFound();
    }
    
    task.Title = input.Title;
    task.Description = input.Description;
    task.Priority = input.Priority;
    task.DueDate = input.DueDate;
    task.UpdatedAt = DateTime.UtcNow;
    
    await db.SaveChangesAsync();
    
    Console.WriteLine($"Updated task with ID: {id}");
    return Results.Ok(task);
})
.WithName("UpdateTask");

app.MapPut("/api/tasks/{id}/status", async (AppDbContext db, int id, StatusUpdateDto statusUpdate) =>
{
    Console.WriteLine($"PUT /api/tasks/{id}/status called with status: {statusUpdate.Status}");
    
    var task = await db.Tasks.FindAsync(id);
    
    if (task == null)
    {
        Console.WriteLine($"Task with ID {id} not found");
        return Results.NotFound();
    }
    
    task.Status = statusUpdate.Status;
    task.UpdatedAt = DateTime.UtcNow;
    
    await db.SaveChangesAsync();
    
    Console.WriteLine($"Updated status of task with ID {id} to: {statusUpdate.Status}");
    return Results.Ok(task);
})
.WithName("UpdateTaskStatus");

app.MapDelete("/api/tasks/{id}", async (AppDbContext db, int id) =>
{
    Console.WriteLine($"DELETE /api/tasks/{id} called");
    
    var task = await db.Tasks.FindAsync(id);
    
    if (task == null)
    {
        Console.WriteLine($"Task with ID {id} not found");
        return Results.NotFound();
    }
    
    db.Tasks.Remove(task);
    await db.SaveChangesAsync();
    
    Console.WriteLine($"Deleted task with ID: {id}");
    return Results.NoContent();
})
.WithName("DeleteTask");

// Afficher les URLs d'écoute
Console.WriteLine("API running at:");
foreach (var address in app.Urls)
{
    Console.WriteLine($"  {address}");
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
