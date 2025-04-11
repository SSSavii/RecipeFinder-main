using Microsoft.AspNetCore.Mvc;
using RecipeFinder.BusinessLogic.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeFinder.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRecipeService _recipeService;

        public HomeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAllIngredients()
        {
            var ingredients = await _recipeService.GetAllIngredientsAsync();
            return Json(ingredients);
        }

        [HttpPost]
        public async Task<JsonResult> GetRecipes([FromBody] string[] ingredients)
        {
            var exactMatchRecipes = await _recipeService.GetRecipesByIngredientsAsync(ingredients);
            var missingOneRecipes = await _recipeService.GetRecipesWithMissingOneAsync(ingredients);

            return Json(new 
            {
                ExactMatches = exactMatchRecipes,
                MissingOneMatches = missingOneRecipes
            });
        }
    }
}