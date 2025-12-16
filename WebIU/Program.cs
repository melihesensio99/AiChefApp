using AiChefApp.Application.Services.conc;
using AiChefApp.Application.Services.Interfaces;
using Microsoft.Extensions.ML;
using Microsoft.ML;
using WebIU;
using WebIU.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddPredictionEnginePool<MLModel.ModelInput, MLModel.ModelOutput>()
    .FromFile(
        modelName: "MLModel",
        filePath: Path.Combine(builder.Environment.ContentRootPath, "MLModels", "MLModel.zip"),
        watchForChanges: true
    );
builder.Services.AddScoped<PredictionService>();
// 🔥 Gemini
builder.Services.AddHttpClient("Gemini", c =>
{
    c.Timeout = TimeSpan.FromMinutes(3);
});

builder.Services.AddScoped<IGeminiRecipe, GeminiRecipe>();


var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
