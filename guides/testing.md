# 🧪 Guia de Testes — .NET MAUI

A estratégia que dá o melhor retorno: **teste a lógica onde ela é barata de testar.** Veja os testes reais em [reference-app/tests](../reference-app/tests).

## A pirâmide, aplicada ao MAUI

```
        ╱╲   UI / E2E (Appium)        poucos, lentos, frágeis — fluxos críticos
       ╱──╲
      ╱────╲  Integração              repositórios reais (SQLite/HTTP)
     ╱──────╲
    ╱────────╲ Unit (ViewModels)      MUITOS, rápidos, determinísticos ← foco
   ╱──────────╲
```

O segredo: como as ViewModels vivem numa **lib pura** (ver [arquitetura](./architecture.md)), elas são testadas como qualquer código .NET — **sem emulador, em milissegundos, em qualquer CI**.

## Testando uma ViewModel

```csharp
[Fact]
public async Task AddTask_AddsToCollection_AndClearsInput()
{
    var repo = new InMemoryTaskRepository();      // fake/real em memória
    var sut = new TaskListViewModel(repo);        // injeta a dependência
    sut.NewTitle = "Comprar pneus";

    await sut.AddTaskCommand.ExecuteAsync(null);  // executa o comando

    Assert.Single(sut.Tasks);
    Assert.Equal(string.Empty, sut.NewTitle);     // efeito colateral verificado
}
```

Padrão **AAA** (Arrange, Act, Assert). Invoque comandos via `Command.ExecuteAsync(...)` / `Command.CanExecute(...)` — exatamente como a UI faria.

## O que vale testar nas ViewModels
- ✅ Comandos produzem o estado esperado (add/remove/toggle)
- ✅ `CanExecute` habilita/desabilita corretamente
- ✅ Propriedades computadas (ex.: `RemainingCount`) reagem a mudanças
- ✅ `INotifyPropertyChanged`/`CanExecuteChanged` disparam quando devem
- ✅ Caminhos de erro (repositório lança → ViewModel trata)

## Dublês de teste
- **Fake em memória** (como `InMemoryTaskRepository`) — simples e suficiente na maioria dos casos.
- **Mocks** (Moq/NSubstitute) — quando você precisa **verificar interação** ("o repositório foi chamado 1x com X").

## Integração e UI
- **Integração:** teste `SqliteTaskRepository` contra um banco real (em arquivo temporário) — pega bugs de mapeamento/SQL.
- **UI/E2E:** Appium para os 2–3 fluxos críticos (login, compra). Caros e frágeis — use com parcimônia, no topo da pirâmide.

## No CI
```yaml
- run: dotnet test reference-app/tests/TaskApp.Core.Tests
```
Roda em qualquer runner Linux/Windows **sem o workload MAUI** — porque o Core não depende dele. É isso que torna o feedback rápido em cada PR.

## Métrica que importa
Não persiga 100% de cobertura cega. Persiga **cobrir a lógica de decisão** (comandos, branches, cálculos). Getter trivial não precisa de teste; um `CanExecute` com regra, sim.
