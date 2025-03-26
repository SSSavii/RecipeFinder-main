using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeFinder.Domain.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public required string Name { get; set; } // Добавлено "required"
    }

}
