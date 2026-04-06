# Smart Gallery Viewer — .NET MAUI Client

[![.NET](https://img.shields.io/badge/.NET_MAUI-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/apps/maui)
[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Android%20%7C%20iOS%20%7C%20macOS-blue)]()
[![Tests](https://img.shields.io/badge/Tests-10%20passing-brightgreen)]()
[![Backend](https://img.shields.io/badge/🔗_Backend-smart--gallery--aws-FF9900?logo=amazonaws)](https://github.com/MarcosSilva-Dev/smart-gallery-aws)

> **Client cross-platform** para o [Smart Gallery AWS](https://github.com/MarcosSilva-Dev/smart-gallery-aws) — galeria de imagens serverless com auto-tagging via Amazon Rekognition.

---

## 🏗️ Parte de um Ecossistema

Este projeto faz parte de uma arquitetura de dois repositórios:

| Repositório | Stack | Responsabilidade |
|---|---|---|
| [**smart-gallery-aws**](https://github.com/MarcosSilva-Dev/smart-gallery-aws) | .NET 8 · Lambda · S3 · DynamoDB · Rekognition | API serverless + IA auto-tagging |
| **smart-gallery-maui** (este) | .NET MAUI 10 · MVVM · CommunityToolkit | Client cross-platform |

```
┌─────────────────────┐         ┌──────────────────────────────┐
│  Smart Gallery MAUI │ ──────► │     Smart Gallery AWS        │
│  (.NET MAUI 10)     │  HTTPS  │  (Lambda + API Gateway)      │
│                     │         │         │                    │
│  📱 Windows         │         │    ┌────┴────┐               │
│  📱 Android         │         │    │ Rekognition (IA tags)   │
│  📱 iOS             │         │    │ S3 (imagens)            │
│  📱 macOS           │         │    │ DynamoDB (metadados)    │
└─────────────────────┘         └────┴─────────────────────────┘
```

---

## ✨ Funcionalidades

### 📷 Galeria de Imagens
- Grid responsivo com thumbnails
- Busca por tags ou título
- Paginação automática (infinite scroll)

### 🚀 Upload com IA
- Seleção de imagens da galeria do dispositivo
- Captura direta da câmera
- Tags manuais + **auto-tagging via Amazon Rekognition**
- Feedback visual dos tags detectados pela IA

### 🔍 Visualização Detalhada
- Imagem em alta resolução
- Metadados completos: resolução, tamanho, formato
- Lista de todas as tags (manuais + IA)
- Exclusão com confirmação

### 📊 Dashboard Analytics
- KPIs: total de imagens, armazenamento, formatos
- Distribuição por formato (barras visuais)
- Ranking de tags mais populares

### ⚙️ Configurações
- URL da API configurável
- Teste de conexão integrado
- Persistência local via `Preferences`

---

## 🛠️ Stack Tecnológica

| Camada | Tecnologia |
|---|---|
| Framework | .NET MAUI 10 |
| Arquitetura | MVVM (Model-View-ViewModel) |
| Source Generators | CommunityToolkit.Mvvm 8.4 |
| HTTP | HttpClient + System.Text.Json |
| Navegação | Shell (TabBar + rotas) |
| Testes | xUnit + MockHttpHandler (10 testes) |

### Por que .NET MAUI?
- **Uma codebase** → Windows, Android, iOS, macOS
- **Ecossistema unificado** com o backend .NET
- **MVVM nativo** com source generators (zero boilerplate)
- **Performance nativa** (não é WebView)

---

## 🚀 Como Usar

### Pré-requisitos
- .NET 10 SDK com workload MAUI
- Backend [smart-gallery-aws](https://github.com/MarcosSilva-Dev/smart-gallery-aws) deployado

### Executar (Windows)
```powershell
cd src/SmartGallery.Maui
dotnet build -f net10.0-windows10.0.19041.0
dotnet run -f net10.0-windows10.0.19041.0
```

### Configurar API
1. Abra a aba **Config** no app
2. Cole a URL da API (ex: `https://xxxxx.execute-api.us-east-1.amazonaws.com/prod`)
3. Clique em **Testar** para validar
4. **Salvar** e reiniciar o app

### Testes
```powershell
cd tests/SmartGallery.Maui.Tests
dotnet test
```

---

## 📁 Estrutura

```
smart-gallery-maui/
├── src/SmartGallery.Maui/
│   ├── Models/           # DTOs (espelho da API)
│   ├── Services/         # GalleryApiService, SettingsService
│   ├── ViewModels/       # MVVM com CommunityToolkit
│   ├── Views/            # XAML Pages (Gallery, Upload, Detail, Dashboard, Settings)
│   ├── Converters/       # Value converters
│   ├── MauiProgram.cs    # DI + configuração
│   └── AppShell.xaml     # Navegação (TabBar)
├── tests/SmartGallery.Maui.Tests/
│   └── GalleryApiServiceTests.cs  # 10 testes
├── .gitignore
└── README.md
```

---

## 📈 Métricas

| Métrica | Valor |
|---|---|
| Testes | 10 |
| Páginas | 5 (Gallery, Upload, Detail, Dashboard, Settings) |
| ViewModels | 5 |
| Plataformas | 4 (Windows, Android, iOS, macOS) |
| Dependências | 2 (CommunityToolkit.Mvvm, Microsoft.Maui) |

---

## 🔗 Projetos Relacionados

- 🔙 **Backend**: [smart-gallery-aws](https://github.com/MarcosSilva-Dev/smart-gallery-aws) — API serverless .NET 8 + Lambda + Rekognition

---

## 📄 Licença

MIT
