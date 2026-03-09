using Newtonsoft.Json;

namespace AICodeAssistantAnalyzer.Models;

// ── Request ──────────────────────────────────────────────────────────────────

public class GeminiRequest
{
    [JsonProperty("contents")]
    public List<GeminiContent> Contents { get; set; } = [];

    [JsonProperty("generationConfig")]
    public GenerationConfig? GenerationConfig { get; set; }
}

public class GeminiContent
{
    [JsonProperty("parts")]
    public List<GeminiPart> Parts { get; set; } = [];

    [JsonProperty("role")]
    public string Role { get; set; } = "user";
}

public class GeminiPart
{
    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;
}

public class GenerationConfig
{
    [JsonProperty("temperature")]
    public double Temperature { get; set; } = 0.7;

    [JsonProperty("maxOutputTokens")]
    public int MaxOutputTokens { get; set; } = 8192;
}

// ── Response ─────────────────────────────────────────────────────────────────

public class GeminiResponse
{
    [JsonProperty("candidates")]
    public List<GeminiCandidate>? Candidates { get; set; }

    [JsonProperty("error")]
    public GeminiError? Error { get; set; }
}

public class GeminiCandidate
{
    [JsonProperty("content")]
    public GeminiContent? Content { get; set; }
}

public class GeminiError
{
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
}