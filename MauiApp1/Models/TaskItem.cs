using System;

namespace MauiApp1.Models
{
    /// <summary>
    /// Représente une tâche reçue de l'API.
    /// Ce modèle correspond exactement au modèle TaskItem de l'API.
    /// </summary>
    public class TaskItem
    {
        public int Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string Status { get; set; } = TaskConstants.Status.Todo;
        
        public string Priority { get; set; } = TaskConstants.Priority.Medium;
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public DateTime? DueDate { get; set; }
    }
} 