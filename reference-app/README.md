# 🧱 Reference App — TaskApp

App .NET MAUI de **referência arquitetural**: como estruturar um app testável, desacoplado e escalável. O domínio (lista de tarefas) é simples de propósito — o foco é a **engenharia**.

## ⭐ O que este projeto demonstra
- **Clean Architecture** — Core (domínio + lógica) separado do head de UI
- **MVVM** com CommunityToolkit.Mvvm (source generators, zero boilerplate)
- **Dependency Injection** — cadeia Repository → ViewModel → Page → App
- **Testes** rodando **sem emulador** — 11 no total (unitários da ViewModel + integração de persistência)
- **Duas implementações de repositório** (memória e arquivo JSON) provando que trocar persistência = 1 linha
- **Compiled bindings**, `CollectionView` com reciclagem, layout raso (as boas práticas dos [guides](../guides))

## 📂 Estrutura

```
reference-app/
├── TaskApp.slnx                    # solution (formato novo, .NET 10)
├── ARCHITECTURE.md                 # decisões, diagrama e trade-offs
├── src/
│   ├── TaskApp.Core/               # lib .NET PURA (testável sem MAUI)
│   │   ├── Models/TaskItem.cs
│   │   ├── Services/               # ITaskRepository + InMemory + JsonFile
│   │   └── ViewModels/TaskListViewModel.cs
│   └── TaskApp.Maui/               # head de UI (Views XAML + DI)
│       ├── MauiProgram.cs          # registro de DI
│       ├── App.xaml(.cs)
│       └── Views/TaskListPage.xaml(.cs)
└── tests/
    └── TaskApp.Core.Tests/         # xUnit — 6 testes do ViewModel
```

## ▶️ Rodando

```bash
# testes do Core — NÃO precisa do workload MAUI nem de emulador
cd reference-app
dotnet test                         # ✅ 11/11 passando (unit + integração)

# app completo (precisa do workload MAUI + device/emulador)
dotnet build src/TaskApp.Maui -f net10.0-android
```

> ✅ Os testes do `TaskApp.Core` foram **compilados e executados** (6/6 verdes). O head MAUI segue o template oficial e é a camada fina de UI sobre o Core testado.

## 📖 Leia também
- [ARCHITECTURE.md](./ARCHITECTURE.md) — o porquê de cada decisão
- [../guides/architecture.md](../guides/architecture.md) — guia de arquitetura MAUI
- [../guides/testing.md](../guides/testing.md) — estratégia de testes
