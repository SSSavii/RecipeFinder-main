using System.Collections.Generic;

namespace RecipeFinder.Domain.Models
{
    public class Recipe
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Photo { get; set; }
        public int CookingTime { get; set; }
        // Список шагов приготовления
        public List<string> Instructions { get; set; } = new List<string>();
        public string MissingIngredient { get; set; }
        // Изменено: список ингредиентов теперь представлен списком строк,
        // содержащих названия ингредиентов.
        public List<string> Ingredients { get; set; } = new List<string>();
    }
}