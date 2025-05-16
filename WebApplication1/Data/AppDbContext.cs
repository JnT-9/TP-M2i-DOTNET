using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the TaskItem entity
            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Status)
                .HasConversion(
                    v => v,
                    v => TaskConstants.Status.All.Contains(v) ? v : TaskConstants.Status.Todo);

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Priority)
                .HasConversion(
                    v => v,
                    v => TaskConstants.Priority.All.Contains(v) ? v : TaskConstants.Priority.Medium);

            // Seed some initial data
            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem
                {
                    Id = 1,
                    Title = "Implement authentication",
                    Description = "Add JWT authentication to the API",
                    Status = TaskConstants.Status.Todo,
                    Priority = TaskConstants.Priority.High,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(7)
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Create UI mockups",
                    Description = "Design the user interface for the task management app",
                    Status = TaskConstants.Status.InProgress,
                    Priority = TaskConstants.Priority.Medium,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(3)
                },
                new TaskItem
                {
                    Id = 3,
                    Title = "Setup project structure",
                    Description = "Initialize the project and configure dependencies",
                    Status = TaskConstants.Status.Done,
                    Priority = TaskConstants.Priority.Low,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1),
                    DueDate = DateTime.UtcNow.AddDays(-2)
                }
            );
        }
    }
} 