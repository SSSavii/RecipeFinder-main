namespace RecipeFinder.Presentation.Models
{
    public class SearchRequest
    {
        // ИНгредиенты, введённые пользователем (например, через текстовое поле)
        public string[] UserIngredients { get; set; }
        // Флаг, указывающий, нужно ли добавлять базовые ингредиенты
        public bool AddDefault { get; set; }
        // Если пользователь выбрал конкретные базовые ингредиенты через модальное окно
        public string[] BaseIngredients { get; set; }
    }
}