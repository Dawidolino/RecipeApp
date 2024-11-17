using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeApp.Models
{
    public class Ingredient
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string IngredientName { get; set; }
        [StringLength(500,ErrorMessage ="Opis powinien zawierać maksymalnie 500 znaków")]
        public string IngredientDescription { get; set; }

        //many to many
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

    }
}
