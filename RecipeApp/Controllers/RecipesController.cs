using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Data;
using RecipeApp.Models;

public class RecipesController : Controller
{
    private readonly ApplicationDbContext _context;

    public RecipesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var recipes = await _context.Recipes.Include(r => r.RecipeIngredients).ToListAsync();
        return View(recipes);
    }

    public async Task<IActionResult> Details(int id)
    {
        var recipe = await _context.Recipes
        .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
        .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null) return NotFound();

        return View(recipe);

    }

    public IActionResult Create()
    {     
        ViewBag.Categories = _context.Categories.ToList();
        var recipe = new Recipe();
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Recipe recipe, string NewCategory)
    {       
        if (string.IsNullOrEmpty(recipe.RecipeCategory) && string.IsNullOrEmpty(NewCategory))
        {           
            ModelState.AddModelError("RecipeCategory", "Wybierz kategorię lub wprowadź nową kategorię.");
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(recipe);
        }

        if (!string.IsNullOrEmpty(NewCategory))
        {     
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == NewCategory);

            if (existingCategory == null)
            {
                var category = new Category { Name = NewCategory };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                recipe.RecipeCategory = category.Name;
            }
            else
            {
                recipe.RecipeCategory = existingCategory.Name;
            }
        }

        _context.Add(recipe);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null) return NotFound();

        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(recipe);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Recipe recipe, string NewCategory)
    {
        if (id != recipe.Id)
        {
            return NotFound();
        }
        if (string.IsNullOrEmpty(recipe.RecipeCategory) && string.IsNullOrEmpty(NewCategory))
        {
            ModelState.AddModelError("RecipeCategory", "Wybierz kategorię lub wprowadź nową kategorię.");
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(recipe);
        }

        if (!string.IsNullOrEmpty(NewCategory))
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == NewCategory);

            if (existingCategory == null)
            {
                var category = new Category { Name = NewCategory };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                recipe.RecipeCategory = category.Name;
            }
            else
            {
                recipe.RecipeCategory = existingCategory.Name;
            }
        }
        _context.Update(recipe);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }


    public async Task<IActionResult> Delete(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null) return NotFound();
        return View(recipe);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe != null)
        {
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index", "Home");
    }
}
