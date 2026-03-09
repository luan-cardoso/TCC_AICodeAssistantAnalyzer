namespace AICodeAssistantAnalyzer.Models;

public class CodeRequest
{
    public string UserPrompt { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;  // detectado ou informado
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class CodeGenerationResult
{
    public bool Success { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

public class CodeAnalysisResult
{
    public QualityAnalysis Quality { get; set; } = new();
    public StructureAnalysis Structure { get; set; } = new();
    public SecurityAnalysis Security { get; set; } = new();
    public int OverallScore { get; set; }   // 0–100
    public string Summary { get; set; } = string.Empty;
}

public class QualityAnalysis
{
    public int Score { get; set; }   // 0–10
    public string Assessment { get; set; } = string.Empty;
    public List<string> Suggestions { get; set; } = [];
}

public class StructureAnalysis
{
    public int Score { get; set; }
    public string Assessment { get; set; } = string.Empty;
    public List<string> Suggestions { get; set; } = [];
}

public class SecurityAnalysis
{
    public int Score { get; set; }
    public string RiskLevel { get; set; } = string.Empty;  // Low / Medium / High / Critical
    public string Assessment { get; set; } = string.Empty;
    public List<string> Vulnerabilities { get; set; } = [];
    public List<string> Recommendations { get; set; } = [];
}