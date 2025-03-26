namespace RecipeFinder.Infrastructure.Database
{
    public class Neo4jSettings
    {
        public required string Uri { get; set; } // Добавлено "required"
        public required string User { get; set; } // Добавлено "required"
        public required string Password { get; set; } // Добавлено "required"
    }

}
