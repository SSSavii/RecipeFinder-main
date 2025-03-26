using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeFinder.Domain.Models;

namespace RecipeFinder.DataAccess.Interfaces
{
    public interface IRecipeRepository
    {
        Task<IEnumerable<Recipe>> FindByIngredientsAsync(string[] ingredients);
    }
}
