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
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Recipe recipe)
    {
        if (ModelState.IsValid)
        {
            _context.Add(recipe);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Home");
        }
        return View(recipe);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null) return NotFound();
        return View(recipe);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Recipe recipe)
    {
        if (id != recipe.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(recipe);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        return View(recipe);
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
