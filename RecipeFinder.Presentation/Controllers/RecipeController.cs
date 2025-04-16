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

    // Метод для отображения страницы поиска
    public async Task<IActionResult> Search()
    {
        var ingredients = await _recipeService.GetAllIngredientsAsync();
        return View(ingredients); // Передаем список ингредиентов в представление
    }

    // Метод для поиска рецептов с учётом недостающих ингредиентов
    [HttpPost]
    public async Task<IActionResult> SearchWithMissing([FromBody] List<string> ingredients)
    {
        if (ingredients == null || ingredients.Count == 0)
        {
            return Json(new List<object>()); // Возвращаем пустой список, если ингредиенты отсутствуют
        }

        try
        {
            var recipes = await _recipeService.GetRecipesWithMissingOneAsync(ingredients.ToArray());

            // Возвращаем список рецептов с недостающими ингредиентами
            return Json(recipes.Select(r => new
            {
                name = r.Name,
                photo = r.Photo,
                cookingTime = r.CookingTime,
                url = r.Url,
                missingIngredient = r.MissingIngredient
            }));
        }
        catch (Exception ex)
        {
            // Логируем ошибку и возвращаем сообщение об ошибке
            Console.WriteLine("Ошибка на сервере: " + ex.Message);
            return StatusCode(500, new { error = "Произошла ошибка при обработке запроса." });
        }
    }
}