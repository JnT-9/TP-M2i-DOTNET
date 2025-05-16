using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class TaskInput
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [RegularExpression("low|medium|high", ErrorMessage = "Priority must be 'low', 'medium', or 'high'")]
        public string Priority { get; set; } = "medium";
        
        public DateTime? DueDate { get; set; }
    }
} 