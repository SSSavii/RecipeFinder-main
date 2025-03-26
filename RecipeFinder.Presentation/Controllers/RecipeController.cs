using Microsoft.AspNetCore.Mvc;
using RecipeFinder.BusinessLogic.Interfaces;
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
            return View(new List<RecipeFinder.Domain.Models.Recipe>()); // Пустой список
        }

        var recipes = await _recipeService.GetRecipesByIngredientsAsync(ingredients);
        return View(recipes ?? new List<RecipeFinder.Domain.Models.Recipe>()); // Если null, передаем пустой список
    }
}
