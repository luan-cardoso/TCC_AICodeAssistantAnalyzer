using Newtonsoft.Json;
using AICodeAssistantAnalyzer.Models;

namespace AICodeAssistantAnalyzer.Services;

/// <summary>
/// Serviço de comunicação com a API Gemini (Google AI).
/// Documentação: https://ai.google.dev/api/generate-content
/// </summary>
public class GeminiService
{
    // ── Configuração ─────────────────────────────────────────────────────────

    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta";
    private const string Model = "gemini-2.5-flash";

    private readonly HttpClient _http;
    private readonly string _apiKey;

    public GeminiService(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API Key do Gemini não pode ser vazia.", nameof(apiKey));

        _apiKey = apiKey;
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
    }

    // ── Método principal ─────────────────────────────────────────────────────

    /// <summary>
    /// Envia um prompt para o Gemini e retorna o texto de resposta.
    /// </summary>
    public async Task<string> GenerateContentAsync(string prompt, double temperature = 0.7)
    {
        var url = $"{BaseUrl}/models/{Model}:generateContent?key={_apiKey}";
        var payload = BuildRequest(prompt, temperature);
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        try
        {
            response = await _http.PostAsync(url, content);
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Erro de rede ao contactar a API Gemini: {ex.Message}", ex);
        }

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var errObj = JsonConvert.DeserializeObject<GeminiResponse>(responseBody);
            var msg = errObj?.Error?.Message ?? responseBody;
            throw new Exception($"Gemini API [{response.StatusCode}]: {msg}");
        }

        return ExtractText(responseBody);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static GeminiRequest BuildRequest(string prompt, double temperature) => new()
    {
        Contents =
        [
            new GeminiContent
            {
                Role  = "user",
                Parts = [ new GeminiPart { Text = prompt } ]
            }
        ],
        GenerationConfig = new GenerationConfig
        {
            Temperature = temperature,
            MaxOutputTokens = 8192
        }
    };

    private static string ExtractText(string responseBody)
    {
        var geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(responseBody)
            ?? throw new Exception("Resposta inválida da API Gemini.");

        var text = geminiResponse.Candidates?
            .FirstOrDefault()?.Content?.Parts?
            .FirstOrDefault()?.Text;

        if (string.IsNullOrWhiteSpace(text))
            throw new Exception("Nenhum conteúdo retornado pela API Gemini.");

        return text.Trim();
    }
}