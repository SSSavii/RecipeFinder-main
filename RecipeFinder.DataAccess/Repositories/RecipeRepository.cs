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
                _logger.LogInformation("Connecting to Neo4j...");

                var session = _driver.AsyncSession();
                var query = @"
                WITH $available_ingredients AS available_ingredients
                MATCH (r:Recipe)-[:HAS_RECIPE]-(i:Ingredient)
                WITH r, COLLECT(i.name) AS recipe_ingredients, available_ingredients
                WHERE ALL(ri IN recipe_ingredients WHERE ri IN available_ingredients)
                RETURN r.title, r.url, r.photo_link, r.cooking_time, r.instructions";

                var result = await session.RunAsync(query, new { available_ingredients = ingredients });

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

        public async Task<IEnumerable<string>> GetAllIngredientsAsync()
        {
            _logger.LogInformation("Fetching all unique ingredients from Neo4j...");

            try
            {
                var session = _driver.AsyncSession();
                var query = @"
                MATCH (i:Ingredient)
                RETURN DISTINCT i.name AS name";

                var result = await session.RunAsync(query);

                var ingredients = (await result.ToListAsync())
                    .Select(record => record["name"].As<string>())
                    .ToList();

                _logger.LogInformation("Found {IngredientCount} unique ingredients.", ingredients.Count);
                return ingredients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching ingredients.");
                throw;
            }
        }

        public async Task<IEnumerable<Recipe>> FindByIngredientsWithMissingOneAsync(string[] ingredients)
        {
            _logger.LogInformation("Fetching recipes with up to one missing ingredient...");

            try
            {
                var session = _driver.AsyncSession();
                var query = @"
                WITH $available_ingredients AS available_ingredients
                MATCH (r:Recipe)-[:HAS_RECIPE]-(i:Ingredient)
                WITH r, COLLECT(i.name) AS recipe_ingredients, available_ingredients,
                     [ri IN recipe_ingredients WHERE NOT ri IN available_ingredients] AS missing_ingredients
                WHERE SIZE(missing_ingredients) <= 1
                RETURN r.title, r.url, r.photo_link, r.cooking_time, r.instructions, SIZE(missing_ingredients) AS missing_count";

                var result = await session.RunAsync(query, new { available_ingredients = ingredients });

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
                _logger.LogError(ex, "Error while fetching recipes.");
                throw;
            }
        }
    }
}