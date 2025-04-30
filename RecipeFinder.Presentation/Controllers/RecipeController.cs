using Microsoft.AspNetCore.Mvc;
using RecipeFinder.BusinessLogic.Interfaces;
using RecipeFinder.Presentation.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeFinder.Presentation.Controllers
{
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
                return View(new List<string>(ingredients));
            }
            catch
            {
                return View(new List<string>());
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> SearchWithMissing([FromBody] SearchRequest request)
        {
            try
            {
                // Получаем ингредиенты, введённые пользователем
                var userIngredients = request.UserIngredients ?? new string[0];
                
                if (request.AddDefault)
                {
                    if (request.BaseIngredients != null && request.BaseIngredients.Any())
                    {
                        // Если пользователь выбрал конкретные базовые ингредиенты из модального окна,
                        // объединяем их с пользовательскими ингредиентами
                        userIngredients = userIngredients.Concat(request.BaseIngredients).ToArray();
                    }
                    else
                    {
                        // Если чекбокс активирован, но конкретный выбор не сделан – добавляем все стандартные
                        userIngredients = userIngredients.Concat(new[] { "Вода", "Масло растительное ", "Соль", "Сахар" }).ToArray();
                    }
                }
    
                var recipes = await _recipeService.GetRecipesWithMissingOneAsync(userIngredients);
                return Json(new { success = true, data = recipes });
            }
            catch
            {
                return StatusCode(500, new { success = false });
            }
        }
    }
}