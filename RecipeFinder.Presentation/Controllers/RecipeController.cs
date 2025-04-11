using Microsoft.AspNetCore.Mvc;
using RecipeFinder.BusinessLogic.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RecipeController : Controller
{
    private readonly IRecipeService _recipeService;

    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public async Task<IActionResult> Search(string[] ingredients)
    {
        if (ingredients == null || ingredients.Length == 0)
        {
            return View(new List<RecipeFinder.Domain.Models.Recipe>());
        }

        var recipes = await _recipeService.GetRecipesByIngredientsAsync(ingredients);
        return View(recipes ?? new List<RecipeFinder.Domain.Models.Recipe>());
    }

    [HttpGet]
    public async Task<IActionResult> GetIngredients()
    {
        var ingredients = await _recipeService.GetAllIngredientsAsync();
        return Json(ingredients);
    }

    [HttpPost]
    public async Task<IActionResult> SearchWithMissing(string[] ingredients)
    {
        var recipes = await _recipeService.GetRecipesWithMissingOneAsync(ingredients);
        return Json(recipes);
    }
}