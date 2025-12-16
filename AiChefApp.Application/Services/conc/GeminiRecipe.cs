using AiChefApp.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace AiChefApp.Application.Services.conc
{
    public class GeminiRecipe : IGeminiRecipe
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public GeminiRecipe(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;

            // ✅ DOĞRU CONFIG PATH
            _apiKey = config["Ai:Gemini:ApiKey"];
            _model = config["Ai:Gemini:Model"]; // "gemini-2.5-flash" gibi
        }
        public async Task<string> GenerateRecipeAsync(string ingredients)
        {
            // ✅ DOĞRU ENDPOINT
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

            var prompt = $"""
            Sen profesyonel bir aşçısın.

            Kullanıcının girdiği malzemelere göre TÜRKÇE bir yemek tarifi üret.

            Kurallar:
            - Başlık ekle
            - Malzemeler maddeler halinde olsun
            - Yapılışı numaralı adımlar halinde yaz
            - En sona kısa bir ipucu ekle
            - Emoji kullanma
            - Açıklama veya meta yorum ekleme
            - En son "Afiyet olsun :)" de.

            Format:

            BAŞLIK:
            <yemek adı>

            MALZEMELER:
            - ...
            - ...

            YAPILIŞI:
            1. ...
            2. ...

            İPUCU:
            ...

            Malzemeler:
            {ingredients}
            """;

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    maxOutputTokens = 5000, // 5000 çok uzun; önce 900-1200 dene
                    temperature = 0.7
                }
            };

            var response = await _httpClient.PostAsync(
                url,
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            );

            // ✅ Hata olursa body’yi gör (404 sebebini net söyler)
            var raw = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Gemini {(int)response.StatusCode}: {raw}\nURL: {url}");

            using var doc = JsonDocument.Parse(raw);

            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();
        }
    }
}

