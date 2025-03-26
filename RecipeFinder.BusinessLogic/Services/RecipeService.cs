using RecipeFinder.BusinessLogic.Interfaces;
using RecipeFinder.DataAccess.Interfaces;
using RecipeFinder.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeFinder.BusinessLogic.Services 
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;

        public RecipeService(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByIngredientsAsync(string[] ingredients)
        {
            var recipes = await _recipeRepository.FindByIngredientsAsync(ingredients);
            return recipes ?? new List<Recipe>();
        }
    }
}
