using AICodeAssistantAnalyzer.Models;
using AICodeAssistantAnalyzer.Services;

// ── Configuração ──────────────────────────────────────────────────────────────

// Carrega a API Key (variável de ambiente ou argumento de linha de comando)
var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY")
          ?? (args.Length > 0 ? args[0] : null);

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌  API Key do Gemini não encontrada.");
    Console.WriteLine("    Defina a variável de ambiente GEMINI_API_KEY ou passe como argumento:");
    Console.WriteLine("    > AICodeAssistantAnalyzer <sua-api-key>");
    Console.ResetColor();
    return;
}

// ── Inicialização dos serviços ────────────────────────────────────────────────

var gemini = new GeminiService(apiKey);
var generator = new CodeGeneratorService(gemini);
var analyzer = new CodeAnalyzerService(gemini);

DisplayService.ShowBanner();

// ── Loop principal ────────────────────────────────────────────────────────────

while (true)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("  O que você deseja? (ou 'sair' para encerrar)\n  > ");
    Console.ForegroundColor = ConsoleColor.White;

    var input = Console.ReadLine()?.Trim();
    Console.ResetColor();

    if (string.IsNullOrWhiteSpace(input)) continue;
    if (input.Equals("sair", StringComparison.OrdinalIgnoreCase)) break;

    // ── Etapa 1: Captura da solicitação ──────────────────────────────────────

    var request = new CodeRequest { UserPrompt = input };

    // ── Etapa 2: Geração de código ───────────────────────────────────────────

    DisplayService.PrintInfo("Gerando código com Gemini...");
    var generationResult = await generator.GenerateAsync(request);
    DisplayService.ShowGenerationResult(generationResult);

    if (!generationResult.Success) continue;

    // ── Etapa 3: Análise do código ───────────────────────────────────────────

    DisplayService.PrintInfo("Analisando qualidade, estrutura e segurança...");
    var analysisResult = await analyzer.AnalyzeAsync(generationResult.Code, input);
    DisplayService.ShowAnalysisResult(analysisResult);
}

Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine("\n  Até logo! 👋\n");
Console.ResetColor();