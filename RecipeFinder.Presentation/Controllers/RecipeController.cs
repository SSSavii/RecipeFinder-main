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

    [HttpGet]
    public async Task<IActionResult> Search()
    {
        try
        {
            var ingredients = await _recipeService.GetAllIngredientsAsync();
            return View(new List<string>(ingredients)); // Явное преобразование в List<string>
        }
        catch
        {
            return View(new List<string>());
        }
    }

    [HttpPost]
    public async Task<IActionResult> SearchWithMissing([FromBody] List<string> ingredients)
    {
        try
        {
            var recipes = await _recipeService.GetRecipesWithMissingOneAsync(ingredients.ToArray());
            return Json(new { success = true, data = recipes });
        }
        catch
        {
            return StatusCode(500, new { success = false });
        }
    }
}