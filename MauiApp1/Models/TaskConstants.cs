namespace MauiApp1.Models
{
    /// <summary>
    /// Définit les constantes utilisées dans l'application pour les statuts et priorités des tâches.
    /// Ces constantes correspondent exactement à celles utilisées par l'API.
    /// </summary>
    public static class TaskConstants
    {
        public static class Status
        {
            public const string Todo = "todo";
            public const string InProgress = "in_progress";
            public const string Done = "done";
            
            public static readonly string[] All = { Todo, InProgress, Done };
        }
        
        public static class Priority
        {
            public const string Low = "low";
            public const string Medium = "medium";
            public const string High = "high";
            
            public static readonly string[] All = { Low, Medium, High };
        }
    }
} 