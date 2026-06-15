# 🏛️ Arquitetura — TaskApp (referência)

Um app .NET MAUI estruturado como um engenheiro sênior/tech lead estruturaria: **testável, desacoplado e pronto pra escalar de equipe**. O domínio é simples de propósito (uma lista de tarefas) — o valor está na **estrutura**, não no feature set.

## Princípio central: separe o que é testável do que precisa de UI

```
┌─────────────────────────────────────────────────────────┐
│  TaskApp.Maui  (head de UI — Views XAML, DI, navegação)  │
│  · depende do Core · roda só em device/emulador          │
└───────────────────────────┬─────────────────────────────┘
                            │ referencia ▼
┌─────────────────────────────────────────────────────────┐
│  TaskApp.Core  (lib .NET PURA — sem MAUI)                │
│  ├── Models/       entidades de domínio                  │
│  ├── Services/     interfaces + implementações           │
│  └── ViewModels/   lógica de apresentação (MVVM)         │
│  · 100% testável em qualquer runner (CI, console)        │
└───────────────────────────▲─────────────────────────────┘
                            │ testa ▼
┌─────────────────────────────────────────────────────────┐
│  TaskApp.Core.Tests  (xUnit — roda em CI sem emulador)   │
└─────────────────────────────────────────────────────────┘
```

> **Por que isso importa (visão tech lead):** UI tests em emulador são lentos e instáveis. Colocando ViewModels e regras numa lib pura, **a maior parte da lógica é coberta por testes rápidos e determinísticos** que rodam em segundos no CI — sem device farm.

## Decisões e trade-offs

| Decisão | Por quê | Trade-off |
|---|---|---|
| **MVVM com CommunityToolkit.Mvvm** | Source generators eliminam boilerplate de `INotifyPropertyChanged`/`ICommand` | Acoplamento ao toolkit (mínimo, é padrão de mercado) |
| **Core como lib separada** | Testabilidade sem MAUI; reuso (ex.: um head Blazor amanhã) | Mais um projeto pra gerenciar |
| **Dependency Injection** | Troca de implementação sem tocar consumidores; testes injetam fakes | Curva de aprendizado do container |
| **Repository por interface** | ViewModels não sabem se é InMemory/SQLite/HTTP (Dependency Inversion) | Indireção a mais |
| **Compiled bindings (`x:DataType`)** | Performance e erros de binding em build, não em runtime | Exige declarar o tipo em cada escopo |

## Fluxo de dependência (DI)

```
InMemoryTaskRepository ──(ITaskRepository)──► TaskListViewModel ──► TaskListPage ──► App
        registrado em MauiProgram.cs                resolvido pelo container
```

Trocar persistência = trocar **uma linha** em `MauiProgram.cs`:
```csharp
// de:
builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
// para:
builder.Services.AddSingleton<ITaskRepository, SqliteTaskRepository>();
```
Nenhuma ViewModel, View ou teste muda.

## Onde evoluiria num projeto real
- **Persistência:** `SqliteTaskRepository` (offline-first) + sync com backend
- **Navegação:** Shell com rotas registradas + `INavigationService` abstraído
- **Erros/estado:** um `Result<T>` ou estados de UI (Loading/Loaded/Error) explícitos
- **Observabilidade:** logging estruturado + telemetria de uso
- **CI:** `dotnet test` em cada PR (os testes do Core rodam sem emulador — ver guides/testing.md)
