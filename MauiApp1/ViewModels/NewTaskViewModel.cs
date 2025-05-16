using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;
using MauiApp1.Services;
using System.Diagnostics;

namespace MauiApp1.ViewModels
{
    public partial class NewTaskViewModel : ObservableObject
    {
        private readonly TaskService _taskService;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _priority = TaskConstants.Priority.Medium;

        [ObservableProperty]
        private DateTime? _dueDate = DateTime.Now.AddDays(1);

        [ObservableProperty]
        private bool _isBusy;

        public List<string> Priorities => TaskConstants.Priority.All.ToList();

        public NewTaskViewModel(TaskService taskService)
        {
            _taskService = taskService;
            Debug.WriteLine("NewTaskViewModel initialized");
        }

        [RelayCommand]
        private async Task CreateTaskAsync()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                await Shell.Current.DisplayAlert("Validation Error", "Title is required", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                Debug.WriteLine("Creating new task...");

                var taskInput = new TaskInput
                {
                    Title = Title,
                    Description = Description,
                    Priority = Priority,
                    DueDate = DueDate
                };

                var newTask = await _taskService.CreateTaskAsync(taskInput);
                
                if (newTask != null)
                {
                    Debug.WriteLine($"Task created successfully with ID: {newTask.Id}");
                    await Shell.Current.DisplayAlert("Success", "Task created successfully", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    Debug.WriteLine("Failed to create task: API returned null");
                    await Shell.Current.DisplayAlert("Error", "Failed to create task. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateTaskAsync: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Failed to create task: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            Debug.WriteLine("Cancelling new task creation");
            await Shell.Current.GoToAsync("..");
        }
    }
} 