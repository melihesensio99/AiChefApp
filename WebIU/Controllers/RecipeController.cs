using AiChefApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebIU.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IGeminiRecipe _geminiService;

        public RecipeController(IGeminiRecipe geminiService)
        {
            _geminiService = geminiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe(string ingredients)
        {
            var result = await _geminiService.GenerateRecipeAsync(ingredients);
            ViewBag.Recipe = result;
            return View("Index");
        }

    }

}

