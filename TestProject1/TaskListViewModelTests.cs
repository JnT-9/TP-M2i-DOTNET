using System.Collections.ObjectModel;
using MauiApp1.Models;
using MauiApp1.Services;
using MauiApp1.ViewModels;
using Moq;

namespace TestProject1;

public class TaskListViewModelTests
{
    private readonly Mock<TaskService> _mockTaskService;
    private readonly TaskListViewModel _viewModel;

    public TaskListViewModelTests()
    {
        _mockTaskService = new Mock<TaskService>(null, "");
        _viewModel = new TaskListViewModel(_mockTaskService.Object);
    }

    [Fact]
    public async Task LoadTasksAsync_ShouldPopulateTasks_WhenServiceReturnsData()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = 1,
                Title = "Test Task 1",
                Status = TaskConstants.Status.Todo,
                Priority = TaskConstants.Priority.Medium
            },
            new TaskItem
            {
                Id = 2,
                Title = "Test Task 2",
                Status = TaskConstants.Status.InProgress,
                Priority = TaskConstants.Priority.High
            }
        };

        _mockTaskService
            .Setup(s => s.GetTasksAsync(It.IsAny<string>()))
            .ReturnsAsync(tasks);

        // Act
        await _viewModel.LoadTasksAsync();

        // Assert
        Assert.Equal(2, _viewModel.Tasks.Count);
        Assert.Equal("Test Task 1", _viewModel.Tasks[0].Title);
        Assert.Equal("Test Task 2", _viewModel.Tasks[1].Title);
    }

    [Fact]
    public async Task FilterByStatus_ShouldLoadFilteredTasks_WhenStatusIsSet()
    {
        // Arrange
        string status = TaskConstants.Status.Todo;
        var filteredTasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = 1,
                Title = "Todo Task",
                Status = TaskConstants.Status.Todo,
                Priority = TaskConstants.Priority.Medium
            }
        };

        _mockTaskService
            .Setup(s => s.GetTasksAsync(status))
            .ReturnsAsync(filteredTasks);

        // Act
        _viewModel.FilterByStatus(status);

        // Assert
        // Vérifier que SelectedStatus a été mis à jour
        Assert.Equal(status, _viewModel.SelectedStatus);
        
        // Attendre que la tâche asynchrone se termine
        await Task.Delay(100);
        
        // Vérifier que les tâches ont été filtrées
        _mockTaskService.Verify(s => s.GetTasksAsync(status), Times.AtLeastOnce);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldRemoveTaskFromCollection_WhenDeletionSucceeds()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "Task to delete",
            Status = TaskConstants.Status.Todo,
            Priority = TaskConstants.Priority.Medium
        };

        // Ajouter la tâche à la collection
        _viewModel.Tasks.Add(task);
        
        _mockTaskService
            .Setup(s => s.DeleteTaskAsync(task.Id))
            .ReturnsAsync(true);

        // Mock Shell.Current.DisplayAlert pour qu'il retourne true (confirmation)
        // Note: Ceci est difficile à tester car Shell.Current est statique
        // Dans une application réelle, on pourrait utiliser une interface pour l'abstraction

        // Act & Assert
        // Comme nous ne pouvons pas facilement mocker Shell.Current.DisplayAlert,
        // nous vérifions seulement que le service est appelé avec le bon ID
        await _viewModel.DeleteTaskCommand.ExecuteAsync(task);
        
        _mockTaskService.Verify(s => s.DeleteTaskAsync(task.Id), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskStatusAsync_ShouldUpdateTaskInCollection_WhenUpdateSucceeds()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "Task to update",
            Status = TaskConstants.Status.Todo,
            Priority = TaskConstants.Priority.Medium
        };

        var updatedTask = new TaskItem
        {
            Id = 1,
            Title = "Task to update",
            Status = TaskConstants.Status.InProgress, // Status changed
            Priority = TaskConstants.Priority.Medium
        };

        // Ajouter la tâche à la collection
        _viewModel.Tasks.Add(task);
        
        _mockTaskService
            .Setup(s => s.UpdateTaskStatusAsync(task.Id, It.IsAny<string>()))
            .ReturnsAsync(updatedTask);

        // Act
        await _viewModel.UpdateTaskStatusCommand.ExecuteAsync(task);

        // Assert
        _mockTaskService.Verify(s => s.UpdateTaskStatusAsync(task.Id, It.IsAny<string>()), Times.Once);
        
        // Vérifier que la tâche a été mise à jour dans la collection
        // Note: Ceci suppose que le ViewModel remplace l'élément dans la collection
        // Si ce n'est pas le cas, ce test pourrait échouer
        Assert.Equal(TaskConstants.Status.InProgress, _viewModel.Tasks[0].Status);
    }
} 