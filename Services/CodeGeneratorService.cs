using AICodeAssistantAnalyzer.Models;

namespace AICodeAssistantAnalyzer.Services;

/// <summary>
/// Etapa 2 – Gera código a partir da solicitação do usuário usando o Gemini.
/// </summary>
public class CodeGeneratorService(GeminiService gemini)
{
    private readonly GeminiService _gemini = gemini;

    public async Task<CodeGenerationResult> GenerateAsync(CodeRequest request)
    {
        var prompt = BuildPrompt(request);

        try
        {
            var raw = await _gemini.GenerateContentAsync(prompt, temperature: 0.4);
            return ParseResponse(raw);
        }
        catch (Exception ex)
        {
            return new CodeGenerationResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    // ── Prompt ────────────────────────────────────────────────────────────────

    private static string BuildPrompt(CodeRequest request)
    {
        var langHint = string.IsNullOrWhiteSpace(request.Language)
            ? string.Empty
            : $" O código deve ser em {request.Language}.";

        return $"""
        Você é um assistente especialista em programação.
        Gere o código solicitado com as seguintes diretrizes:
        - Código limpo, legível e bem comentado
        - Siga as boas práticas da linguagem
        - Trate casos extremos quando relevante
        - Use nomes de variáveis em inglês

        Solicitação: {request.UserPrompt}{langHint}

        Responda SOMENTE neste formato (sem texto extra fora das tags):

        <CODE>
        [código aqui]
        </CODE>

        <EXPLANATION>
        [explicação breve em português do que o código faz]
        </EXPLANATION>
        """;
    }

    // ── Parser ────────────────────────────────────────────────────────────────

    private static CodeGenerationResult ParseResponse(string raw)
    {
        var code = ExtractTag(raw, "CODE");
        var explanation = ExtractTag(raw, "EXPLANATION");

        // Remove markdown code fences se o modelo incluir mesmo assim
        code = StripCodeFences(code);

        if (string.IsNullOrWhiteSpace(code))
            return new CodeGenerationResult
            {
                Success = false,
                ErrorMessage = "O modelo não retornou código no formato esperado."
            };

        return new CodeGenerationResult
        {
            Success = true,
            Code = code.Trim(),
            Explanation = explanation.Trim()
        };
    }

    private static string ExtractTag(string text, string tag)
    {
        var open = $"<{tag}>";
        var close = $"</{tag}>";
        var start = text.IndexOf(open, StringComparison.OrdinalIgnoreCase);
        var end = text.IndexOf(close, StringComparison.OrdinalIgnoreCase);

        if (start == -1 || end == -1) return string.Empty;
        return text[(start + open.Length)..end];
    }

    private static string StripCodeFences(string code)
    {
        var lines = code.Split('\n').ToList();
        if (lines.Count > 0 && lines[0].TrimStart().StartsWith("```"))
            lines.RemoveAt(0);
        if (lines.Count > 0 && lines[^1].TrimStart().StartsWith("```"))
            lines.RemoveAt(lines.Count - 1);
        return string.Join('\n', lines);
    }
}