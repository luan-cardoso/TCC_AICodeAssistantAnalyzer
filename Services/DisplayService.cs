using AICodeAssistantAnalyzer.Models;

namespace AICodeAssistantAnalyzer.Services;

/// <summary>
/// Renderiza os resultados no console com formatação visual clara.
/// </summary>
public static class DisplayService
{
    // ── Cabeçalho ─────────────────────────────────────────────────────────────

    public static void ShowBanner()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("""
        ╔══════════════════════════════════════════════════╗
        ║        AI Code Assistant Analyzer                ║
        ║        Powered by Google Gemini                  ║
        ╚══════════════════════════════════════════════════╝
        """);
        Console.ResetColor();
    }

    // ── Geração ───────────────────────────────────────────────────────────────

    public static void ShowGenerationResult(CodeGenerationResult result)
    {
        Console.WriteLine();
        PrintSection("📄 CÓDIGO GERADO", ConsoleColor.Green);

        if (!result.Success)
        {
            PrintError(result.ErrorMessage);
            return;
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(result.Code);
        Console.ResetColor();

        if (!string.IsNullOrWhiteSpace(result.Explanation))
        {
            Console.WriteLine();
            PrintSection("💬 EXPLICAÇÃO", ConsoleColor.Yellow);
            Console.WriteLine(result.Explanation);
        }
    }

    // ── Análise ───────────────────────────────────────────────────────────────

    public static void ShowAnalysisResult(CodeAnalysisResult analysis)
    {
        Console.WriteLine();
        PrintSection("🔍 ANÁLISE DO CÓDIGO", ConsoleColor.Magenta);

        // Score geral
        var scoreColor = analysis.OverallScore >= 80 ? ConsoleColor.Green
                       : analysis.OverallScore >= 60 ? ConsoleColor.Yellow
                       : ConsoleColor.Red;

        Console.Write("  Score Geral: ");
        Console.ForegroundColor = scoreColor;
        Console.WriteLine($"{analysis.OverallScore}/100 {ScoreBar(analysis.OverallScore)}");
        Console.ResetColor();

        // Sumário
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"\n  {analysis.Summary}");
        Console.ResetColor();

        // Qualidade
        Console.WriteLine();
        PrintSubSection($"✅  Qualidade  [{analysis.Quality.Score}/10]");
        Console.WriteLine($"  {analysis.Quality.Assessment}");
        PrintList("Sugestões", analysis.Quality.Suggestions);

        // Estrutura
        Console.WriteLine();
        PrintSubSection($"🏗   Estrutura  [{analysis.Structure.Score}/10]");
        Console.WriteLine($"  {analysis.Structure.Assessment}");
        PrintList("Sugestões", analysis.Structure.Suggestions);

        // Segurança
        Console.WriteLine();
        var riskColor = analysis.Security.RiskLevel switch
        {
            "Critical" => ConsoleColor.Red,
            "High" => ConsoleColor.DarkRed,
            "Medium" => ConsoleColor.Yellow,
            _ => ConsoleColor.Green
        };
        Console.Write($"  🔒  Segurança  [{analysis.Security.Score}/10]  Risco: ");
        Console.ForegroundColor = riskColor;
        Console.WriteLine(analysis.Security.RiskLevel);
        Console.ResetColor();
        Console.WriteLine($"  {analysis.Security.Assessment}");
        PrintList("Vulnerabilidades", analysis.Security.Vulnerabilities, ConsoleColor.Red);
        PrintList("Recomendações", analysis.Security.Recommendations, ConsoleColor.Yellow);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('─', 52));
        Console.ResetColor();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void PrintSection(string title, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(new string('─', 52));
        Console.WriteLine($"  {title}");
        Console.WriteLine(new string('─', 52));
        Console.ResetColor();
    }

    private static void PrintSubSection(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  {title}");
        Console.ResetColor();
    }

    private static void PrintList(string label, List<string> items,
        ConsoleColor color = ConsoleColor.DarkYellow)
    {
        if (items.Count == 0) return;
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  → {label}:");
        Console.ForegroundColor = color;
        foreach (var item in items)
            Console.WriteLine($"     • {item}");
        Console.ResetColor();
    }

    private static string ScoreBar(int score)
    {
        var filled = score / 10;
        return "[" + new string('█', filled) + new string('░', 10 - filled) + "]";
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ❌ {message}");
        Console.ResetColor();
    }

    public static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  ⏳ {message}");
        Console.ResetColor();
    }
}