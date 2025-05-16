using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels
{
    public partial class TaskListViewModel : ObservableObject
    {
        private readonly TaskService _taskService;
        private string _selectedStatus = string.Empty;

        [ObservableProperty]
        private ObservableCollection<TaskItem> _tasks = new();

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isRefreshing;

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (SetProperty(ref _selectedStatus, value))
                {
                    LoadTasksAsync().ConfigureAwait(false);
                }
            }
        }

        public ObservableCollection<string> StatusFilters { get; } = new()
        {
            "",
            TaskConstants.Status.Todo,
            TaskConstants.Status.InProgress,
            TaskConstants.Status.Done
        };

        public TaskListViewModel(TaskService taskService)
        {
            _taskService = taskService;
            Debug.WriteLine("TaskListViewModel initialized");
            LoadTasksAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        private void FilterByStatus(string status)
        {
            Debug.WriteLine($"Filtering tasks by status: {status ?? "All"}");
            SelectedStatus = status;
        }

        [RelayCommand]
        private async Task LoadTasksAsync()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                IsRefreshing = true;

                Debug.WriteLine("Loading tasks...");
                string? statusFilter = string.IsNullOrEmpty(SelectedStatus) ? null : SelectedStatus;
                Debug.WriteLine($"Status filter: {statusFilter ?? "All"}");
                
                var tasks = await _taskService.GetTasksAsync(statusFilter);
                Debug.WriteLine($"Received {tasks.Count} tasks from API");
                
                Tasks.Clear();
                foreach (var task in tasks)
                {
                    Debug.WriteLine($"Task: ID={task.Id}, Title={task.Title}, Status={task.Status}");
                    Tasks.Add(task);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading tasks: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                await Shell.Current.DisplayAlert("Error", $"Failed to load tasks: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
                Debug.WriteLine($"Task loading completed. Tasks count: {Tasks.Count}");
            }
        }

        [RelayCommand]
        private async Task ViewTaskDetailsAsync(TaskItem task)
        {
            if (task == null) return;

            Debug.WriteLine($"Viewing details for task ID: {task.Id}");
            var parameters = new Dictionary<string, object>
            {
                { "TaskId", task.Id }
            };

            await Shell.Current.GoToAsync("taskdetail", parameters);
        }

        [RelayCommand]
        private async Task AddNewTaskAsync()
        {
            Debug.WriteLine("Navigating to new task page");
            await Shell.Current.GoToAsync("newtask");
        }

        [RelayCommand]
        private async Task UpdateTaskStatusAsync(TaskItem task)
        {
            if (task == null) return;

            string newStatus;
            
            switch (task.Status)
            {
                case TaskConstants.Status.Todo:
                    newStatus = TaskConstants.Status.InProgress;
                    break;
                case TaskConstants.Status.InProgress:
                    newStatus = TaskConstants.Status.Done;
                    break;
                case TaskConstants.Status.Done:
                    newStatus = TaskConstants.Status.Todo;
                    break;
                default:
                    newStatus = TaskConstants.Status.Todo;
                    break;
            }

            Debug.WriteLine($"Updating task {task.Id} status from {task.Status} to {newStatus}");

            try
            {
                var updatedTask = await _taskService.UpdateTaskStatusAsync(task.Id, newStatus);
                if (updatedTask != null)
                {
                    Debug.WriteLine($"Task status updated successfully");
                    var index = Tasks.IndexOf(task);
                    if (index >= 0)
                    {
                        Tasks[index] = updatedTask;
                    }
                    
                    // Refresh the list if we're filtering by status
                    if (!string.IsNullOrEmpty(SelectedStatus))
                    {
                        await LoadTasksAsync();
                    }
                }
                else
                {
                    Debug.WriteLine("Failed to update task status: API returned null");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating task status: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Failed to update task status: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task DeleteTaskAsync(TaskItem task)
        {
            if (task == null) return;

            Debug.WriteLine($"Deleting task with ID: {task.Id}");
            
            bool confirm = await Shell.Current.DisplayAlert("Confirm Delete", 
                "Are you sure you want to delete this task?", "Yes", "No");

            if (!confirm) return;

            try
            {
                bool success = await _taskService.DeleteTaskAsync(task.Id);
                if (success)
                {
                    Debug.WriteLine("Task deleted successfully");
                    Tasks.Remove(task);
                }
                else
                {
                    Debug.WriteLine("Failed to delete task");
                    await Shell.Current.DisplayAlert("Error", "Failed to delete task", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting task: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Failed to delete task: {ex.Message}", "OK");
            }
        }
    }
} 