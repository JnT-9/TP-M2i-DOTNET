using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class StatusUpdateDto
    {
        [Required]
        [RegularExpression("todo|in_progress|done", ErrorMessage = "Status must be 'todo', 'in_progress', or 'done'")]
        public string Status { get; set; } = string.Empty;
    }
} 