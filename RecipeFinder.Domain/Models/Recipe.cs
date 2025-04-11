using System.Collections.Generic;

namespace RecipeFinder.Domain.Models
{
    public class Recipe
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Photo { get; set; }
        public int CookingTime { get; set; }
        public string Instructions { get; set; }
        public string MissingIngredient { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    }
}