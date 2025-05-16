using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using MauiApp1.Models;
using MauiApp1.Services;
using Moq;
using Moq.Contrib.HttpClient;

namespace TestProject1;

public class TaskServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "http://api.test";
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = _mockHttpMessageHandler.CreateClient();
        _taskService = new TaskService(_httpClient, _baseUrl);
    }

    [Fact]
    public async Task GetTasksAsync_ShouldReturnTasks_WhenApiReturnsSuccess()
    {
        // Arrange
        var expectedTasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = 1,
                Title = "Test Task 1",
                Description = "Description 1",
                Status = TaskConstants.Status.Todo,
                Priority = TaskConstants.Priority.Medium,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new TaskItem
            {
                Id = 2,
                Title = "Test Task 2",
                Description = "Description 2",
                Status = TaskConstants.Status.InProgress,
                Priority = TaskConstants.Priority.High,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }
        };

        var json = JsonSerializer.Serialize(expectedTasks);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .SetupRequest(HttpMethod.Get, $"{_baseUrl}/api/tasks")
            .ReturnsResponse(response);

        // Act
        var result = await _taskService.GetTasksAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Test Task 1", result[0].Title);
        Assert.Equal("Test Task 2", result[1].Title);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnTask_WhenApiReturnsSuccess()
    {
        // Arrange
        var expectedTask = new TaskItem
        {
            Id = 1,
            Title = "Test Task",
            Description = "Description",
            Status = TaskConstants.Status.Todo,
            Priority = TaskConstants.Priority.Medium,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var json = JsonSerializer.Serialize(expectedTask);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .SetupRequest(HttpMethod.Get, $"{_baseUrl}/api/tasks/1")
            .ReturnsResponse(response);

        // Act
        var result = await _taskService.GetTaskByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Task", result.Title);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldReturnCreatedTask_WhenApiReturnsSuccess()
    {
        // Arrange
        var taskInput = new TaskInput
        {
            Title = "New Task",
            Description = "New Description",
            Priority = TaskConstants.Priority.High,
            DueDate = DateTime.Now.AddDays(1)
        };

        var createdTask = new TaskItem
        {
            Id = 3,
            Title = taskInput.Title,
            Description = taskInput.Description,
            Status = TaskConstants.Status.Todo,
            Priority = taskInput.Priority,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            DueDate = taskInput.DueDate
        };

        var json = JsonSerializer.Serialize(createdTask);
        var response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .SetupRequest(req => 
                req.Method == HttpMethod.Post && 
                req.RequestUri == new Uri($"{_baseUrl}/api/tasks"))
            .ReturnsResponse(response);

        // Act
        var result = await _taskService.CreateTaskAsync(taskInput);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal("New Task", result.Title);
        Assert.Equal("New Description", result.Description);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldReturnUpdatedTask_WhenApiReturnsSuccess()
    {
        // Arrange
        int taskId = 1;
        var taskInput = new TaskInput
        {
            Title = "Updated Task",
            Description = "Updated Description",
            Priority = TaskConstants.Priority.Low,
            DueDate = DateTime.Now.AddDays(2)
        };

        var updatedTask = new TaskItem
        {
            Id = taskId,
            Title = taskInput.Title,
            Description = taskInput.Description,
            Status = TaskConstants.Status.Todo,
            Priority = taskInput.Priority,
            CreatedAt = DateTime.Now.AddDays(-1),
            UpdatedAt = DateTime.Now,
            DueDate = taskInput.DueDate
        };

        var json = JsonSerializer.Serialize(updatedTask);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .SetupRequest(req => 
                req.Method == HttpMethod.Put && 
                req.RequestUri == new Uri($"{_baseUrl}/api/tasks/{taskId}"))
            .ReturnsResponse(response);

        // Act
        var result = await _taskService.UpdateTaskAsync(taskId, taskInput);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(taskId, result.Id);
        Assert.Equal("Updated Task", result.Title);
        Assert.Equal("Updated Description", result.Description);
        Assert.Equal(TaskConstants.Priority.Low, result.Priority);
    }

    [Fact]
    public async Task UpdateTaskStatusAsync_ShouldReturnUpdatedTask_WhenApiReturnsSuccess()
    {
        // Arrange
        int taskId = 1;
        string newStatus = TaskConstants.Status.Done;

        var updatedTask = new TaskItem
        {
            Id = taskId,
            Title = "Task",
            Description = "Description",
            Status = newStatus,
            Priority = TaskConstants.Priority.Medium,
            CreatedAt = DateTime.Now.AddDays(-1),
            UpdatedAt = DateTime.Now
        };

        var json = JsonSerializer.Serialize(updatedTask);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .SetupRequest(req => 
                req.Method == HttpMethod.Put && 
                req.RequestUri == new Uri($"{_baseUrl}/api/tasks/{taskId}/status"))
            .ReturnsResponse(response);

        // Act
        var result = await _taskService.UpdateTaskStatusAsync(taskId, newStatus);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(taskId, result.Id);
        Assert.Equal(newStatus, result.Status);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldReturnTrue_WhenApiReturnsSuccess()
    {
        // Arrange
        int taskId = 1;
        var response = new HttpResponseMessage(HttpStatusCode.NoContent);

        _mockHttpMessageHandler
            .SetupRequest(HttpMethod.Delete, $"{_baseUrl}/api/tasks/{taskId}")
            .ReturnsResponse(response);

        // Act
        var result = await _taskService.DeleteTaskAsync(taskId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldReturnFalse_WhenApiReturnsError()
    {
        // Arrange
        int taskId = 999; // Assuming this ID doesn't exist
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);

        _mockHttpMessageHandler
            .SetupRequest(HttpMethod.Delete, $"{_baseUrl}/api/tasks/{taskId}")
            .ReturnsResponse(response);

        // Act
        var result = await _taskService.DeleteTaskAsync(taskId);

        // Assert
        Assert.False(result);
    }
} 