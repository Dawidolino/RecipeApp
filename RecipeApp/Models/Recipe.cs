using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Podaj nazwę przepisu")]
        [Display(Name = "Nazwa przepisu")]
        public string RecipeName { get; set; }

        [Required]
        [StringLength(1500, ErrorMessage ="Opis nie może przekraczać 1500 znaków")]
        [Display(Name = "Opis przepisu")]
        public string RecipeDescription { get; set; }

        [Required]
        public string RecipeCategory { get; set; }

        [Required]
        [Display(Name = "Instrukcje")]
        public string Instructions { get; set; }

        [Range(1, 1000, ErrorMessage ="Czas przygotowania musi być w przedziale 1-1000 minut")]
        [Display(Name = "Czas przygotowania (min)")]
        public int? PrepTime { get; set; }
      
        //many to many
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

    }
}
