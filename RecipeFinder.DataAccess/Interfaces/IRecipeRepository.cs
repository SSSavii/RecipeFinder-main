using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeFinder.Domain.Models;

namespace RecipeFinder.DataAccess.Interfaces
{
    public interface IRecipeRepository
    {
        Task<IEnumerable<Recipe>> FindByIngredientsAsync(string[] ingredients);
        Task<IEnumerable<string>> GetAllIngredientsAsync(); // Новый метод для получения всех ингредиентов
        Task<IEnumerable<Recipe>> FindByIngredientsWithMissingOneAsync(string[] ingredients); // Новый метод для поиска рецептов с недостающим ингредиентом
    }
}