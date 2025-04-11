// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", async () => {
    const ingredientButtons = document.getElementById("ingredient-buttons");
    const searchButton = document.getElementById("search-button"); // Получаем кнопку поиска

    // Загрузка ингредиентов
    const response = await fetch('/Recipe/GetIngredients');
    const ingredients = await response.json();

    // Создание кнопок для ингредиентов
    ingredients.forEach(ingredient => {
        const button = document.createElement('button');
        button.textContent = ingredient;
        button.className = 'ingredient-button';
        button.onclick = () => toggleIngredient(button, ingredient);
        ingredientButtons.appendChild(button);
    });

    // Добавляем обработчик события для кнопки поиска
    searchButton.addEventListener("click", searchRecipes);
});

const selectedIngredients = new Set();

// Переключение состояния кнопок ингредиентов
function toggleIngredient(button, ingredient) {
    if (selectedIngredients.has(ingredient)) {
        selectedIngredients.delete(ingredient);
        button.classList.remove('selected');
    } else {
        selectedIngredients.add(ingredient);
        button.classList.add('selected');
    }
}

// Обработка поиска рецептов
async function searchRecipes() {
    const response = await fetch('/Recipe/Search', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(Array.from(selectedIngredients))
    });

    const recipes = await response.json();
    displayRecipes(recipes);
}

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