using Microsoft.Extensions.ML;
using Microsoft.ML;
using static WebIU.MLModel;

namespace WebIU.Services
{
    public class PredictionService
    {
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _pool;

        public PredictionService(PredictionEnginePool<ModelInput, ModelOutput> pool)
        {
            _pool = pool;
        }

        public (string label, float confidence) Predict(byte[] imageBytes)
        {
            var input = new ModelInput
            {
                Label = "",
                ImageSource = imageBytes
            };

            var output = _pool.Predict("MLModel", input);

            var confidence = (output.Score != null && output.Score.Length > 0)
                ? output.Score.Max()
                : 0f;

            return (output.PredictedLabel, confidence);
        }
    }
}

