using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiChefApp.Application.Services.Interfaces
{
    public interface IGeminiRecipe
    {
        Task<string> GenerateRecipeAsync(string ingredients);
    }
}
