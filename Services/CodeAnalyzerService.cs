using Newtonsoft.Json;
using AICodeAssistantAnalyzer.Models;

namespace AICodeAssistantAnalyzer.Services;

/// <summary>
/// Etapa 3 – Analisa qualidade, estrutura e segurança do código gerado.
/// </summary>
public class CodeAnalyzerService(GeminiService gemini)
{
    private readonly GeminiService _gemini = gemini;

    public async Task<CodeAnalysisResult> AnalyzeAsync(string code, string originalRequest)
    {
        var prompt = BuildAnalysisPrompt(code, originalRequest);

        try
        {
            var raw = await _gemini.GenerateContentAsync(prompt, temperature: 0.2);
            var json = ExtractJson(raw);
            return ParseAnalysis(json);
        }
        catch (Exception ex)
        {
            // Fallback mínimo se a análise falhar
            return new CodeAnalysisResult
            {
                Summary = $"Erro na análise: {ex.Message}",
                OverallScore = 0
            };
        }
    }

    // ── Prompt ────────────────────────────────────────────────────────────────

    private static string BuildAnalysisPrompt(string code, string originalRequest) => """
        Você é um revisor de código sênior especialista em qualidade, boas práticas e segurança.
        Analise o código abaixo que foi gerado para atender à seguinte solicitação:
        "{originalRequest}"

        CÓDIGO:
        {code}

        Retorne APENAS um objeto JSON válido (sem markdown, sem texto extra) com esta estrutura exata:
        {{
          "quality": {{
            "score": <0-10>,
            "assessment": "<avaliação geral da qualidade>",
            "suggestions": ["<sugestão 1>", "<sugestão 2>"]
          }},
          "structure": {{
            "score": <0-10>,
            "assessment": "<avaliação da estrutura e organização>",
            "suggestions": ["<sugestão 1>", "<sugestão 2>"]
          }},
          "security": {{
            "score": <0-10>,
            "riskLevel": "<Low|Medium|High|Critical>",
            "assessment": "<avaliação de segurança>",
            "vulnerabilities": ["<vulnerabilidade encontrada ou 'Nenhuma identificada'>"],
            "recommendations": ["<recomendação 1>"]
          }},
          "overallScore": <0-100>,
          "summary": "<resumo executivo da análise em 2-3 frases>"
        }}
        """;

    // ── Parsers ───────────────────────────────────────────────────────────────

    private static string ExtractJson(string raw)
    {
        // Remove possíveis blocos ```json ... ```
        var cleaned = raw.Trim();
        if (cleaned.StartsWith("```"))
        {
            var firstNewline = cleaned.IndexOf('\n');
            cleaned = firstNewline > -1 ? cleaned[(firstNewline + 1)..] : cleaned;
        }
        if (cleaned.EndsWith("```"))
            cleaned = cleaned[..^3];

        // Encontra o primeiro { e último } para extrair o JSON puro
        var start = cleaned.IndexOf('{');
        var end = cleaned.LastIndexOf('}');
        if (start > -1 && end > start)
            return cleaned[start..(end + 1)];

        return cleaned.Trim();
    }

    private static CodeAnalysisResult ParseAnalysis(string json)
    {
        dynamic? obj = JsonConvert.DeserializeObject(json)
            ?? throw new Exception("JSON de análise inválido.");

        return new CodeAnalysisResult
        {
            Quality = new QualityAnalysis
            {
                Score = (int)(obj!.quality?.score ?? 0),
                Assessment = (string)(obj.quality?.assessment ?? ""),
                Suggestions = ParseStringList(obj.quality?.suggestions)
            },
            Structure = new StructureAnalysis
            {
                Score = (int)(obj.structure?.score ?? 0),
                Assessment = (string)(obj.structure?.assessment ?? ""),
                Suggestions = ParseStringList(obj.structure?.suggestions)
            },
            Security = new SecurityAnalysis
            {
                Score = (int)(obj.security?.score ?? 0),
                RiskLevel = (string)(obj.security?.riskLevel ?? "Low"),
                Assessment = (string)(obj.security?.assessment ?? ""),
                Vulnerabilities = ParseStringList(obj.security?.vulnerabilities),
                Recommendations = ParseStringList(obj.security?.recommendations)
            },
            OverallScore = (int)(obj.overallScore ?? 0),
            Summary = (string)(obj.summary ?? "")
        };
    }

    private static List<string> ParseStringList(dynamic? arr)
    {
        var list = new List<string>();
        if (arr == null) return list;
        foreach (var item in arr)
            list.Add((string)item);
        return list;
    }
}