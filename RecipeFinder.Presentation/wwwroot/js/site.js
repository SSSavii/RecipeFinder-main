document.addEventListener("DOMContentLoaded", async () => {
    const searchButton = document.getElementById("search-button");

    // Обработка поиска рецептов
    searchButton.addEventListener("click", async () => {
        const select = document.getElementById("ingredient-select");
        const selectedIngredients = Array.from(select.selectedOptions).map(option => option.value);

        const response = await fetch('/Recipe/SearchWithMissing', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(selectedIngredients)
        });

        const recipes = await response.json();
        displayRecipes(recipes);
    });
});

// Отображение рецептов
function displayRecipes(recipes) {
    const resultsDiv = document.getElementById("results");
    resultsDiv.innerHTML = "";

    recipes.forEach(recipe => {
        const recipeDiv = document.createElement('div');
        recipeDiv.className = 'recipe';
        recipeDiv.innerHTML = `
            <h3>${recipe.name}</h3>
            <img src="${recipe.photo}" alt="${recipe.name}" />
            <p>Время приготовления: ${recipe.cookingTime} минут</p>
            <a href="${recipe.url}" target="_blank">Перейти к рецепту</a>
        `;
        resultsDiv.appendChild(recipeDiv);
    });
}