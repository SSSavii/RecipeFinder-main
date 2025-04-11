using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeFinder.Domain.Models;

namespace RecipeFinder.BusinessLogic.Interfaces
{
    public interface IRecipeService
    {
        Task<IEnumerable<Recipe>> GetRecipesByIngredientsAsync(string[] ingredients);
        Task<IEnumerable<string>> GetAllIngredientsAsync(); // Новый метод для получения всех ингредиентов
        Task<IEnumerable<Recipe>> GetRecipesWithMissingOneAsync(string[] ingredients); // Новый метод для рецептов с недостающим ингредиентом
    }
}