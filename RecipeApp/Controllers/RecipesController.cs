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
        ViewBag.Ingredients = _context.Ingredients.ToList();
        var recipe = new Recipe();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Recipe recipe, string NewCategory, List<int> selectedIngredientIds, List<string> newIngredients)
    {
        if (string.IsNullOrEmpty(recipe.RecipeCategory) && string.IsNullOrEmpty(NewCategory))
        {
            ModelState.AddModelError("RecipeCategory", "Wybierz kategorię lub wprowadź nową kategorię.");
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Ingredients = await _context.Ingredients.ToListAsync();
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

        //assign ingredients
        if (selectedIngredientIds != null)
        {
            foreach (var ingredientId in selectedIngredientIds)
            {
                var ingredient = await _context.Ingredients.FindAsync(ingredientId);
                if (ingredient != null)
                {
                    recipe.RecipeIngredients.Add(new RecipeIngredient { Ingredient = ingredient });
                }
            }
        }

        //new ingredient from textarea
        if (newIngredients != null)
        {
            foreach (var ingredientName in newIngredients)
            {
                if (!string.IsNullOrWhiteSpace(ingredientName))
                {
                    var existingIngredient = await _context.Ingredients
                        .FirstOrDefaultAsync(i => i.IngredientName == ingredientName);

                    if (existingIngredient == null)
                    {
                        var newIngredient = new Ingredient
                        {
                            IngredientName = ingredientName,
                            IngredientDescription = "Lorem ipsum" //default description
                        };
                        _context.Ingredients.Add(newIngredient);
                        await _context.SaveChangesAsync();
                        
                        recipe.RecipeIngredients.Add(new RecipeIngredient { IngredientId = newIngredient.Id, RecipeId = recipe.Id });
                    }
                    else
                    {
                        //add existing ingredient to the recipe
                        recipe.RecipeIngredients.Add(new RecipeIngredient { IngredientId = existingIngredient.Id, RecipeId = recipe.Id });
                    }
                }
            }
        }

        _context.Add(recipe);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }



    //old create method
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create(Recipe recipe, string NewCategory)
    //{
    //    if (string.IsNullOrEmpty(recipe.RecipeCategory) && string.IsNullOrEmpty(NewCategory))
    //    {
    //        ModelState.AddModelError("RecipeCategory", "Wybierz kategorię lub wprowadź nową kategorię.");
    //        ViewBag.Categories = await _context.Categories.ToListAsync();
    //        return View(recipe);
    //    }

    //    if (!string.IsNullOrEmpty(NewCategory))
    //    {
    //        var existingCategory = await _context.Categories
    //            .FirstOrDefaultAsync(c => c.Name == NewCategory);

    //        if (existingCategory == null)
    //        {
    //            var category = new Category { Name = NewCategory };
    //            _context.Categories.Add(category);
    //            await _context.SaveChangesAsync();

    //            recipe.RecipeCategory = category.Name;
    //        }
    //        else
    //        {
    //            recipe.RecipeCategory = existingCategory.Name;
    //        }
    //    }

    //    _context.Add(recipe);
    //    await _context.SaveChangesAsync();
    //    return RedirectToAction("Index", "Home");
    //}

    public async Task<IActionResult> Edit(int id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null) return NotFound();

        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Ingredients = await _context.Ingredients.ToListAsync();
        return View(recipe);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Recipe recipe, string NewCategory, List<int> selectedIngredientIds, List<string> newIngredients)
    {
        if (id != recipe.Id)
        {
            return NotFound();
        }
        if (string.IsNullOrEmpty(recipe.RecipeCategory) && string.IsNullOrEmpty(NewCategory))
        {
            ModelState.AddModelError("RecipeCategory", "Wybierz kategorię lub wprowadź nową kategorię.");
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Ingredients = await _context.Ingredients.ToListAsync();
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
    

        //foreach (var recipeIngredient in currentIngredients.ToList())
        //{
        //    if (selectedIngredientIds == null || !selectedIngredientIds.Contains(recipeIngredient.IngredientId))
        //    {
        //        _context.RecipeIngredients.Remove(recipeIngredient);
        //    }
        //}

        ////add new ingredients from list
        //if (selectedIngredientIds != null)
        //{
        //    foreach (var ingredientId in selectedIngredientIds)
        //    {
        //        var ingredient = await _context.Ingredients.FindAsync(ingredientId);
        //        if (ingredient != null && !currentIngredients.Any(ri => ri.IngredientId == ingredient.Id))
        //        {
        //            recipe.RecipeIngredients.Add(new RecipeIngredient { IngredientId = ingredient.Id, RecipeId = recipe.Id });
        //        }
        //    }
        //}
        ////add new ingredients from textarea
        //if (newIngredients != null)
        //{
        //    foreach (var ingredientName in newIngredients)
        //    {
        //        if (!string.IsNullOrWhiteSpace(ingredientName))
        //        {
        //            var existingIngredient = await _context.Ingredients
        //                .FirstOrDefaultAsync(i => i.IngredientName == ingredientName);

        //            if (existingIngredient == null)
        //            {
        //                var newIngredient = new Ingredient 
        //                { 
        //                    IngredientName = ingredientName, 
        //                    IngredientDescription = "Lorem ipsum"
        //                };
        //                _context.Ingredients.Add(newIngredient);
        //                await _context.SaveChangesAsync();

        //                recipe.RecipeIngredients.Add(new RecipeIngredient { IngredientId = newIngredient.Id, RecipeId = recipe.Id });
        //            }
        //            else
        //            {
        //                recipe.RecipeIngredients.Add(new RecipeIngredient { IngredientId = existingIngredient.Id, RecipeId = recipe.Id });
        //            }
        //        }
        //    }
        //}


        //var currentIngredients = await _context.RecipeIngredients
        //.Where(ri => ri.RecipeId == recipe.Id)
        //.ToListAsync();

        //write code for editing ingredients in recipe inside this method
      
        // Remove deselected ingredients
        var currentIngredients = await _context.RecipeIngredients
            .Where(ri => ri.RecipeId == recipe.Id)
            .ToListAsync();
        //assign ingredients

        foreach (var recipeIngredient in currentIngredients.ToList())
        {
            if (selectedIngredientIds == null || !selectedIngredientIds.Contains(recipeIngredient.IngredientId))
            {
                _context.RecipeIngredients.Remove(recipeIngredient);
            }
        }
        if (selectedIngredientIds != null)
        {
            var distinctIngredientIds = selectedIngredientIds.Distinct();

            foreach (var ingredientId in distinctIngredientIds)
            {
                var ingredient = await _context.Ingredients.FindAsync(ingredientId);
                if (ingredient != null)
                {
                    // Check if the ingredient is already associated with the recipe
                    var existingRecipeIngredient = await _context.RecipeIngredients
                        .FirstOrDefaultAsync(ri => ri.RecipeId == recipe.Id && ri.IngredientId == ingredient.Id);

                    if (existingRecipeIngredient == null)
                    {
                        // New ingredient relationship, add it
                        recipe.RecipeIngredients.Add(new RecipeIngredient { Ingredient = ingredient });
                    }
                    else
                    {
                        // Existing ingredient, attach it
                        _context.Attach(existingRecipeIngredient);
                    }
                }
            }
        }

        //da sie przypisac skladnik z textarea ale nie z listy
        //new ingredient from textarea
        if (newIngredients != null)
        {
            foreach (var ingredientName in newIngredients)
            {
                if (!string.IsNullOrWhiteSpace(ingredientName))
                {
                    // Check for existing ingredient first
                    var existingIngredient = await _context.Ingredients
                        .FirstOrDefaultAsync(i => i.IngredientName == ingredientName);

                    var newRecipeIngredient = new RecipeIngredient();
                    if (existingIngredient != null)
                    {
                        newRecipeIngredient.IngredientId = existingIngredient.Id;
                    }
                    else
                    {
                        // Create new ingredient if it doesn't exist
                        var newIngredient = new Ingredient
                        {
                            IngredientName = ingredientName,
                            IngredientDescription = "Lorem ipsum" //default description
                        };
                        _context.Ingredients.Add(newIngredient);
                        await _context.SaveChangesAsync();
                        newRecipeIngredient.IngredientId = newIngredient.Id;
                    }
                    recipe.RecipeIngredients.Add(newRecipeIngredient);
                }
            }
        }


        _context.Update(recipe);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    //old edit

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int id, Recipe recipe, string NewCategory)
    //{
    //    if (id != recipe.Id)
    //    {
    //        return NotFound();
    //    }
    //    if (string.IsNullOrEmpty(recipe.RecipeCategory) && string.IsNullOrEmpty(NewCategory))
    //    {
    //        ModelState.AddModelError("RecipeCategory", "Wybierz kategorię lub wprowadź nową kategorię.");
    //        ViewBag.Categories = await _context.Categories.ToListAsync();
    //        return View(recipe);
    //    }

    //    if (!string.IsNullOrEmpty(NewCategory))
    //    {
    //        var existingCategory = await _context.Categories
    //            .FirstOrDefaultAsync(c => c.Name == NewCategory);

    //        if (existingCategory == null)
    //        {
    //            var category = new Category { Name = NewCategory };
    //            _context.Categories.Add(category);
    //            await _context.SaveChangesAsync();

    //            recipe.RecipeCategory = category.Name;
    //        }
    //        else
    //        {
    //            recipe.RecipeCategory = existingCategory.Name;
    //        }
    //    }
    //    _context.Update(recipe);
    //    await _context.SaveChangesAsync();

    //    return RedirectToAction("Index", "Home");
    //}

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
