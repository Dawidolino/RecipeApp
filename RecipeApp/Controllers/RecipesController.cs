using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Data;
using RecipeApp.Models;

namespace RecipeApp.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var recipes = await _context.Recipes.Include(r=> r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();
            return View(recipes);
        }
        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _context.Recipes.Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r=>r.Id==id);
            if (recipe == null) return NotFound();
            return View(recipe);
        }

    }
}
