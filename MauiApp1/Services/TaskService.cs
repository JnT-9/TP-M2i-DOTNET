using System.Net.Http.Json;
using System.Diagnostics;
using MauiApp1.Models;

namespace MauiApp1.Services
{
    public class TaskService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public TaskService(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
            Debug.WriteLine($"TaskService initialized with base URL: {baseUrl}");
        }

        public async Task<List<TaskItem>> GetTasksAsync(string? status = null)
        {
            string url = $"{_baseUrl}/api/tasks";
            if (!string.IsNullOrEmpty(status))
            {
                url += $"?status={status}";
            }

            Debug.WriteLine($"Fetching tasks from: {url}");
            try
            {
                var response = await _httpClient.GetAsync(url);
                Debug.WriteLine($"Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var tasks = await response.Content.ReadFromJsonAsync<List<TaskItem>>();
                    Debug.WriteLine($"Received {tasks?.Count ?? 0} tasks");
                    return tasks ?? new List<TaskItem>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error content: {errorContent}");
                    return new List<TaskItem>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting tasks: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return new List<TaskItem>();
            }
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            string url = $"{_baseUrl}/api/tasks/{id}";
            Debug.WriteLine($"Fetching task from: {url}");
            
            try
            {
                var response = await _httpClient.GetAsync(url);
                Debug.WriteLine($"Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var task = await response.Content.ReadFromJsonAsync<TaskItem>();
                    return task;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error content: {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting task {id}: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<TaskItem?> CreateTaskAsync(TaskInput task)
        {
            string url = $"{_baseUrl}/api/tasks";
            Debug.WriteLine($"Creating task at: {url}");
            Debug.WriteLine($"Task data: Title={task.Title}, Priority={task.Priority}");
            
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, task);
                Debug.WriteLine($"Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var createdTask = await response.Content.ReadFromJsonAsync<TaskItem>();
                    Debug.WriteLine($"Created task with ID: {createdTask?.Id}");
                    return createdTask;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error content: {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating task: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<TaskItem?> UpdateTaskAsync(int id, TaskInput task)
        {
            string url = $"{_baseUrl}/api/tasks/{id}";
            Debug.WriteLine($"Updating task at: {url}");
            
            try
            {
                var response = await _httpClient.PutAsJsonAsync(url, task);
                Debug.WriteLine($"Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var updatedTask = await response.Content.ReadFromJsonAsync<TaskItem>();
                    return updatedTask;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error content: {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating task {id}: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<TaskItem?> UpdateTaskStatusAsync(int id, string status)
        {
            string url = $"{_baseUrl}/api/tasks/{id}/status";
            Debug.WriteLine($"Updating task status at: {url}");
            Debug.WriteLine($"New status: {status}");
            
            try
            {
                var statusUpdate = new StatusUpdateDto { Status = status };
                var response = await _httpClient.PutAsJsonAsync(url, statusUpdate);
                Debug.WriteLine($"Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var updatedTask = await response.Content.ReadFromJsonAsync<TaskItem>();
                    return updatedTask;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error content: {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating task {id} status: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            string url = $"{_baseUrl}/api/tasks/{id}";
            Debug.WriteLine($"Deleting task at: {url}");
            
            try
            {
                var response = await _httpClient.DeleteAsync(url);
                Debug.WriteLine($"Response status: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting task {id}: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }
    }
} 