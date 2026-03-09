## 🚀 Como rodar

### Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) instalado
- Uma API Key do Google Gemini → [aistudio.google.com](https://aistudio.google.com)

---

### 1. Instalar dependências
```bash
dotnet add package Newtonsoft.Json
```

### 2. Configurar a API Key

**Linux/macOS**
```bash
export GEMINI_API_KEY="sua-chave-aqui"
```

**Windows (PowerShell)**
```powershell
$env:GEMINI_API_KEY="sua-chave-aqui"
```

### 3. Rodar o projeto
```bash
dotnet run
```
