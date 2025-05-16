using System;
using System.ComponentModel.DataAnnotations;

namespace MauiApp1.Models
{
    /// <summary>
    /// Représente les données d'entrée pour créer ou mettre à jour une tâche.
    /// Ce modèle correspond aux données attendues par l'API.
    /// </summary>
    public class TaskInput
    {
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string Priority { get; set; } = TaskConstants.Priority.Medium;
        
        public DateTime? DueDate { get; set; }
    }
} 