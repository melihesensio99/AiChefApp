using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using WebIU.Services;


namespace WebIU.Controllers
{
    public class PredictController : Controller
    {
        private readonly PredictionEnginePool<MLModel.ModelInput, MLModel.ModelOutput> _pool;

        public PredictController(PredictionEnginePool<MLModel.ModelInput, MLModel.ModelOutput> pool)
        {
            _pool = pool;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile image, [FromServices] PredictionService svc)
        {
            if (image == null || image.Length == 0) return View();

            byte[] bytes;

            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                bytes = ms.ToArray();          
            }

            var base64 = Convert.ToBase64String(bytes);
            ViewBag.ImageDataUrl = $"data:{image.ContentType};base64,{base64}";

            var (label, conf) = svc.Predict(bytes);

            ViewBag.Result = label;
            ViewBag.Confidence = conf;

            return View();
        }
    }
}
