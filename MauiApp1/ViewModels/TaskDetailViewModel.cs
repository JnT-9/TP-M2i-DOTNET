using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels
{
    [QueryProperty(nameof(TaskId), "TaskId")]
    public partial class TaskDetailViewModel : ObservableObject
    {
        private readonly TaskService _taskService;

        [ObservableProperty]
        private int _taskId;

        [ObservableProperty]
        private TaskItem? _task;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isEditing;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _priority = TaskConstants.Priority.Medium;

        [ObservableProperty]
        private DateTime? _dueDate;

        public List<string> Priorities => TaskConstants.Priority.All.ToList();
        public List<string> Statuses => TaskConstants.Status.All.ToList();

        public TaskDetailViewModel(TaskService taskService)
        {
            _taskService = taskService;
        }

        public async void OnTaskIdChanged()
        {
            await LoadTaskAsync();
        }

        [RelayCommand]
        private async Task LoadTaskAsync()
        {
            if (TaskId <= 0) return;

            try
            {
                IsLoading = true;
                Task = await _taskService.GetTaskByIdAsync(TaskId);

                if (Task != null)
                {
                    Title = Task.Title;
                    Description = Task.Description ?? string.Empty;
                    Priority = Task.Priority;
                    DueDate = Task.DueDate;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load task: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private void StartEditing()
        {
            IsEditing = true;
        }

        [RelayCommand]
        private void CancelEditing()
        {
            if (Task != null)
            {
                Title = Task.Title;
                Description = Task.Description ?? string.Empty;
                Priority = Task.Priority;
                DueDate = Task.DueDate;
            }
            IsEditing = false;
        }

        [RelayCommand]
        private async Task SaveTaskAsync()
        {
            if (Task == null) return;

            try
            {
                var taskInput = new TaskInput
                {
                    Title = Title,
                    Description = Description,
                    Priority = Priority,
                    DueDate = DueDate
                };

                var updatedTask = await _taskService.UpdateTaskAsync(TaskId, taskInput);
                if (updatedTask != null)
                {
                    Task = updatedTask;
                    IsEditing = false;
                    await Shell.Current.DisplayAlert("Success", "Task updated successfully", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to update task: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task UpdateStatusAsync(string status)
        {
            if (Task == null) return;

            try
            {
                var updatedTask = await _taskService.UpdateTaskStatusAsync(TaskId, status);
                if (updatedTask != null)
                {
                    Task = updatedTask;
                    await Shell.Current.DisplayAlert("Success", "Task status updated successfully", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to update task status: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task DeleteTaskAsync()
        {
            if (Task == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Confirm Delete", 
                "Are you sure you want to delete this task?", "Yes", "No");

            if (!confirm) return;

            try
            {
                bool success = await _taskService.DeleteTaskAsync(TaskId);
                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Task deleted successfully", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to delete task: {ex.Message}", "OK");
            }
        }
    }
} 