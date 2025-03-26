using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using RecipeFinder.DataAccess.Interfaces;
using RecipeFinder.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace RecipeFinder.DataAccess.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly IDriver _driver;
        private readonly ILogger<RecipeRepository> _logger;

        public RecipeRepository(IDriver driver, ILogger<RecipeRepository> logger)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _logger = logger;
        }

        public async Task<IEnumerable<Recipe>> FindByIngredientsAsync(string[] ingredients)
        {
            _logger.LogInformation("Ingredients passed to query: {Ingredients}", string.Join(", ", ingredients));

            try
            {
                // Логируем успешное подключение
                _logger.LogInformation("Connecting to Neo4j...");

                var session = _driver.AsyncSession();
                var query = @"
                WITH $available_ingredients AS available_ingredients
                MATCH (r:Recipe)-[:HAS_RECIPE]-(i:Ingredient)
                WITH r, COLLECT(i.name) AS recipe_ingredients, available_ingredients
                WHERE ALL(ri IN recipe_ingredients WHERE ri IN available_ingredients)
                RETURN r.title, r.url, r.photo_link, r.cooking_time, r.instructions";

                var result = await session.RunAsync(query, new { available_ingredients = ingredients });

                // Логируем количество найденных рецептов
                var recipes = (await result.ToListAsync()).Select(record =>
                    new Recipe
                    {
                        Name = record["r.title"].As<string>(),
                        Url = record["r.url"].As<string>(),
                        Photo = record["r.photo_link"].As<string>(),
                        CookingTime = Convert.ToInt32(record["r.cooking_time"]),
                        Instructions = record["r.instructions"].As<string>()
                    }
                ).ToList();

                _logger.LogInformation("Found {RecipeCount} recipes.", recipes.Count);
                return recipes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while executing the Neo4j query.");
                throw;
            }
        }
    }
}
