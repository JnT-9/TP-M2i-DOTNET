using MauiApp1.Models;
using MauiApp1.Services;
using MauiApp1.ViewModels;
using Moq;

namespace TestProject1;

public class TaskDetailViewModelTests
{
    private readonly Mock<TaskService> _mockTaskService;
    private readonly TaskDetailViewModel _viewModel;

    public TaskDetailViewModelTests()
    {
        _mockTaskService = new Mock<TaskService>(null, "");
        _viewModel = new TaskDetailViewModel(_mockTaskService.Object);
    }

    [Fact]
    public async Task LoadTaskAsync_ShouldLoadTaskDetails_WhenTaskIdIsSet()
    {
        // Arrange
        int taskId = 1;
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            Description = "Test Description",
            Status = TaskConstants.Status.InProgress,
            Priority = TaskConstants.Priority.High,
            CreatedAt = DateTime.Now.AddDays(-1),
            UpdatedAt = DateTime.Now,
            DueDate = DateTime.Now.AddDays(3)
        };

        _mockTaskService
            .Setup(s => s.GetTaskByIdAsync(taskId))
            .ReturnsAsync(task);

        // Act
        _viewModel.TaskId = taskId;
        await _viewModel.LoadTaskCommand.ExecuteAsync(null);

        // Assert
        Assert.NotNull(_viewModel.Task);
        Assert.Equal(taskId, _viewModel.Task.Id);
        Assert.Equal("Test Task", _viewModel.Task.Title);
        Assert.Equal("Test Description", _viewModel.Task.Description);
        Assert.Equal(TaskConstants.Status.InProgress, _viewModel.Task.Status);
        Assert.Equal(TaskConstants.Priority.High, _viewModel.Task.Priority);
    }

    [Fact]
    public void StartEditing_ShouldEnableEditMode()
    {
        // Arrange
        _viewModel.IsEditing = false;

        // Act
        _viewModel.StartEditingCommand.Execute(null);

        // Assert
        Assert.True(_viewModel.IsEditing);
    }

    [Fact]
    public void CancelEditing_ShouldDisableEditMode()
    {
        // Arrange
        _viewModel.IsEditing = true;
        _viewModel.Task = new TaskItem
        {
            Id = 1,
            Title = "Original Title",
            Description = "Original Description",
            Priority = TaskConstants.Priority.Medium
        };
        
        // Modifier les valeurs
        _viewModel.Title = "Changed Title";
        _viewModel.Description = "Changed Description";
        _viewModel.Priority = TaskConstants.Priority.High;

        // Act
        _viewModel.CancelEditingCommand.Execute(null);

        // Assert
        Assert.False(_viewModel.IsEditing);
        
        // Vérifier que les valeurs ont été restaurées
        Assert.Equal("Original Title", _viewModel.Title);
        Assert.Equal("Original Description", _viewModel.Description);
        Assert.Equal(TaskConstants.Priority.Medium, _viewModel.Priority);
    }

    [Fact]
    public async Task SaveTaskAsync_ShouldUpdateTask_WhenEditingIsComplete()
    {
        // Arrange
        int taskId = 1;
        _viewModel.TaskId = taskId;
        _viewModel.IsEditing = true;
        
        // Définir les valeurs modifiées
        _viewModel.Title = "Updated Title";
        _viewModel.Description = "Updated Description";
        _viewModel.Priority = TaskConstants.Priority.High;
        _viewModel.DueDate = DateTime.Now.AddDays(5);
        
        var updatedTask = new TaskItem
        {
            Id = taskId,
            Title = "Updated Title",
            Description = "Updated Description",
            Status = TaskConstants.Status.Todo,
            Priority = TaskConstants.Priority.High,
            CreatedAt = DateTime.Now.AddDays(-1),
            UpdatedAt = DateTime.Now,
            DueDate = _viewModel.DueDate
        };
        
        _mockTaskService
            .Setup(s => s.UpdateTaskAsync(taskId, It.IsAny<TaskInput>()))
            .ReturnsAsync(updatedTask);
        
        // Act
        await _viewModel.SaveTaskCommand.ExecuteAsync(null);
        
        // Assert
        _mockTaskService.Verify(s => s.UpdateTaskAsync(
            taskId, 
            It.Is<TaskInput>(input => 
                input.Title == "Updated Title" && 
                input.Description == "Updated Description" && 
                input.Priority == TaskConstants.Priority.High)), 
            Times.Once);
            
        Assert.False(_viewModel.IsEditing);
        Assert.Equal(updatedTask, _viewModel.Task);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateTaskStatus()
    {
        // Arrange
        int taskId = 1;
        string newStatus = TaskConstants.Status.Done;
        
        _viewModel.TaskId = taskId;
        _viewModel.Task = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            Status = TaskConstants.Status.InProgress
        };
        
        var updatedTask = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            Status = newStatus
        };
        
        _mockTaskService
            .Setup(s => s.UpdateTaskStatusAsync(taskId, newStatus))
            .ReturnsAsync(updatedTask);
        
        // Act
        await _viewModel.UpdateStatusCommand.ExecuteAsync(newStatus);
        
        // Assert
        _mockTaskService.Verify(s => s.UpdateTaskStatusAsync(taskId, newStatus), Times.Once);
        Assert.Equal(newStatus, _viewModel.Task.Status);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldDeleteTask_WhenConfirmed()
    {
        // Arrange
        int taskId = 1;
        _viewModel.TaskId = taskId;
        _viewModel.Task = new TaskItem { Id = taskId };
        
        _mockTaskService
            .Setup(s => s.DeleteTaskAsync(taskId))
            .ReturnsAsync(true);
        
        // Note: Nous ne pouvons pas facilement tester Shell.Current.DisplayAlert
        // ou Shell.Current.GoToAsync car ce sont des méthodes statiques
        
        // Act & Assert
        // Nous vérifions simplement que le service est appelé avec le bon ID
        await _viewModel.DeleteTaskCommand.ExecuteAsync(null);
        
        _mockTaskService.Verify(s => s.DeleteTaskAsync(taskId), Times.Once);
    }
} 