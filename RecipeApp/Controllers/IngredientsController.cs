using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Data;
using RecipeApp.Models;

public class IngredientsController : Controller
{
    private readonly ApplicationDbContext _context;

    public IngredientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var ingredients = await _context.Ingredients.ToListAsync();
        return View(ingredients);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Ingredient ingredient)
    {
        if (ModelState.IsValid)
        {
            _context.Add(ingredient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(ingredient);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var ingredient = await _context.Ingredients.FindAsync(id);
        if (ingredient == null) return NotFound();
        return View(ingredient);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Ingredient ingredient)
    {
        if (id != ingredient.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(ingredient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(ingredient);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var ingredient = await _context.Ingredients.FindAsync(id);
        if (ingredient == null) return NotFound();
        return View(ingredient);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ingredient = await _context.Ingredients.FindAsync(id);
        if (ingredient != null)
        {
            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
