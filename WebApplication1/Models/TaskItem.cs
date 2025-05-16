using System;

namespace WebApplication1.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string Status { get; set; } = TaskConstants.Status.Todo;
        
        public string Priority { get; set; } = TaskConstants.Priority.Medium;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? DueDate { get; set; }
    }
} 