namespace MauiApp1.Models
{
    /// <summary>
    /// Représente les données d'entrée pour mettre à jour le statut d'une tâche.
    /// Ce modèle correspond aux données attendues par l'API.
    /// </summary>
    public class StatusUpdateDto
    {
        public string Status { get; set; } = string.Empty;
    }
} 