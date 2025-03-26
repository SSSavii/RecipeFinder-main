using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeFinder.Domain.Models;

namespace RecipeFinder.BusinessLogic.Interfaces
{
    public interface IRecipeService
    {
        Task<IEnumerable<Recipe>> GetRecipesByIngredientsAsync(string[] ingredients);
    }
}
