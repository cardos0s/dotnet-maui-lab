# 🏛️ Guia de Arquitetura — .NET MAUI

Como estruturar um app MAUI que **escala de equipe e sobrevive a mudanças**. Veja o [reference-app](../reference-app) para a implementação completa e testada.

## 1. Separe o Core do head de UI

A regra que mais paga: **lógica não-visual numa lib .NET pura**, sem dependência do MAUI.

```
App.Core   (Models, Services, ViewModels)   ← testável em qualquer runner
App.Maui   (Views, DI, navegação)           ← depende do Core
App.Core.Tests                              ← roda em CI sem emulador
```

Benefícios: testes rápidos, build de CI sem workload de UI, e reuso (um head Blazor/console amanhã reaproveita o mesmo Core).

## 2. MVVM com CommunityToolkit.Mvvm

Use os **source generators** — eles eliminam o boilerplate de `INotifyPropertyChanged` e `ICommand`.

```csharp
public partial class TaskListViewModel : ObservableObject
{
    [ObservableProperty]                       // gera a propriedade NewTitle
    [NotifyCanExecuteChangedFor(nameof(AddTaskCommand))]
    private string _newTitle = string.Empty;

    [RelayCommand(CanExecute = nameof(CanAdd))] // gera AddTaskCommand
    private async Task AddTaskAsync() { /* ... */ }

    private bool CanAdd() => !string.IsNullOrWhiteSpace(NewTitle);
}
```

- `partial` é obrigatório (o gerador completa a classe).
- `[RelayCommand]` em método `XxxAsync` gera `XxxCommand` (`IAsyncRelayCommand`).

## 3. Dependency Injection

Registre serviços, ViewModels e Pages no container. A View recebe a ViewModel por construtor — não a constrói.

```csharp
builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
builder.Services.AddSingleton<TaskListViewModel>();
builder.Services.AddSingleton<TaskListPage>();
```

**Singleton vs Transient:** serviços de estado/cache → Singleton; ViewModels de telas reabertas com estado novo → Transient. Pense no ciclo de vida.

## 4. Dependa de abstrações (Dependency Inversion)

A ViewModel depende de `ITaskRepository`, nunca da implementação. Isso permite trocar InMemory → SQLite → HTTP sem tocar em ViewModel/View/testes, e injetar fakes nos testes.

## 5. Navegação desacoplada

Evite `Navigation.PushAsync(new Page())` espalhado nas ViewModels (acopla à UI). Abstraia num `INavigationService` que a ViewModel chama, ou use **Shell** com rotas registradas (`Routing.RegisterRoute`) e navegação por string/rota.

## 6. Estado de UI explícito

Em vez de só `IsBusy`, modele os estados reais: `Loading / Loaded / Empty / Error`. Deixa a UI previsível e os testes diretos.

## Camadas, em resumo

| Camada | Responsabilidade | Conhece o MAUI? |
|---|---|---|
| **Models** | Entidades e regras de domínio | ❌ |
| **Services** | Persistência, rede, dispositivo (por interface) | ❌ |
| **ViewModels** | Estado e comandos da tela | ❌ |
| **Views (XAML)** | Layout e binding | ✅ |
| **DI / App** | Composição (amarra tudo) | ✅ |

➡️ Veja também: [testes](./testing.md) · [layout](./layout.md) · [performance](./performance.md)
