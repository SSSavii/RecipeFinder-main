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
        WHERE ANY(ri IN recipe_ingredients WHERE ri IN available_ingredients)
        RETURN r.title AS title, r.url AS url, r.photo_link AS photo, 
               r.cooking_time AS cooking_time, r.instructions AS instructions";

        var result = await session.RunAsync(query, new { available_ingredients = ingredients });

        var recipes = (await result.ToListAsync()).Select(record =>
            new Recipe
            {
                Name = record["title"].As<string>(),
                Url = record["url"].As<string>(),
                Photo = record["photo"].As<string>(),
                CookingTime = record["cooking_time"].As<int>(),
                Instructions = record["instructions"].As<string>()
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
                RETURN DISTINCT i.name AS name
                ORDER BY i.name";

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
        WITH r, COLLECT(i.name) AS recipe_ingredients, available_ingredients
        WITH r, recipe_ingredients, available_ingredients,
             [ri IN recipe_ingredients WHERE NOT ri IN available_ingredients] AS missing_ingredients
        WHERE SIZE(missing_ingredients) <= 2
        RETURN r.title AS title, r.url AS url, r.photo_link AS photo, 
               r.cooking_time AS cooking_time, r.instructions AS instructions,
               CASE WHEN SIZE(missing_ingredients) > 0 THEN missing_ingredients[0] ELSE null END AS missing_ingredient";

        var result = await session.RunAsync(query, new { available_ingredients = ingredients });

        var recipes = (await result.ToListAsync()).Select(record =>
            new Recipe
            {
                Name = record["title"].As<string>(),
                Url = record["url"].As<string>(),
                Photo = record["photo"].As<string>(),
                CookingTime = record["cooking_time"].As<int>(),
                Instructions = record["instructions"].As<string>(),
                MissingIngredient = record["missing_ingredient"].As<string?>() // ✅ безопасно
            }
        ).ToList();

        _logger.LogInformation("Found {RecipeCount} recipes with missing ingredients.", recipes.Count);
        return recipes;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error while fetching recipes with missing ingredients.");
        throw;
    }
}
    }
}