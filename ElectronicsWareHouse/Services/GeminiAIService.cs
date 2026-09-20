using System.Text;
using System.Text.Json;

namespace ElectronicsWareHouse.Services
{
    public class GeminiAIService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public GeminiAIService(
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;

            // Give Gemini enough time to respond
            _httpClient.Timeout = TimeSpan.FromMinutes(3);
        }

        public async Task<string> AskGeminiAsync(string prompt)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            var model = _configuration["Gemini:Model"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return "Gemini API Key is not configured.";
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                model = "gemini-3.6-flash";
            }

            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);

            // Retry up to 3 times if Gemini temporarily returns 503
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    using var request = new HttpRequestMessage(
                        HttpMethod.Post,
                        url);

                    request.Headers.Add("x-goog-api-key", apiKey);

                    request.Content = new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json");

                    var response =
                        await _httpClient.SendAsync(request);

                    var responseContent =
                        await response.Content.ReadAsStringAsync();

                    // Success
                    if (response.IsSuccessStatusCode)
                    {
                        using var document =
                            JsonDocument.Parse(responseContent);

                        var text =
                            document.RootElement
                                .GetProperty("candidates")[0]
                                .GetProperty("content")
                                .GetProperty("parts")[0]
                                .GetProperty("text")
                                .GetString();

                        return text ?? "No response generated.";
                    }

                    // Temporary server overload
                    if ((int)response.StatusCode == 503)
                    {
                        if (attempt < 3)
                        {
                            await Task.Delay(
                                TimeSpan.FromSeconds(2 * attempt));

                            continue;
                        }

                        return "Gemini is currently experiencing high demand. Please try again in a few moments.";
                    }

                    // Other API errors
                    if ((int)response.StatusCode == 429)
                    {
                        return "Gemini request limit reached. Please wait a moment and try again.";
                    }

                    if ((int)response.StatusCode == 400)
                    {
                        return "Gemini rejected the request. Please check the model configuration and request data.";
                    }

                    if ((int)response.StatusCode == 401 ||
                        (int)response.StatusCode == 403)
                    {
                        return "Gemini authentication failed. Please check the API Key.";
                    }

                    return $"Gemini API Error ({(int)response.StatusCode}). Please try again later.";
                }
                catch (TaskCanceledException)
                {
                    if (attempt < 3)
                    {
                        await Task.Delay(
                            TimeSpan.FromSeconds(2 * attempt));

                        continue;
                    }

                    return "Gemini request timed out. Please try again.";
                }
                catch (HttpRequestException)
                {
                    if (attempt < 3)
                    {
                        await Task.Delay(
                            TimeSpan.FromSeconds(2 * attempt));

                        continue;
                    }

                    return "Could not connect to Gemini. Please check your internet connection and try again.";
                }
                catch (Exception)
                {
                    return "An unexpected error occurred while contacting Gemini.";
                }
            }

            return "Gemini is temporarily unavailable. Please try again later.";
        }
    }
}