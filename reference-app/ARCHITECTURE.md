# 🏛️ Arquitetura do TaskApp — as decisões e o porquê de cada uma

> Um Architecture Decision Record (ADR) informal. O app é uma lista de tarefas — simples de propósito. O que vale a pena ler aqui não é *o que* ele faz, mas **por que ele é estruturado assim**, porque essas mesmas decisões escalam pra um app de verdade com 40 telas.

## A tese central

Existe uma única ideia que organiza tudo neste projeto, e se você levar só ela embora já valeu: **separe o código que precisa de UI do código que não precisa, e teste o segundo até não sobrar dúvida.**

Num app mobile típico, a lógica de negócio fica grudada na tela. O cálculo mora no evento de clique, a decisão mora no code-behind, e o resultado é que pra testar qualquer coisa você precisa do app inteiro de pé num emulador. Aqui a gente faz o oposto: a lógica vive numa biblioteca .NET pura, sem nenhuma linha de MAUI, e a tela é uma camada fina por cima.

```
┌─────────────────────────────────────────────────────────┐
│  TaskApp.Maui  (a CARA — Views XAML, DI, navegação)      │
│  · depende do Core · só roda em device/emulador          │
└───────────────────────────┬─────────────────────────────┘
                            │ referencia ▼
┌─────────────────────────────────────────────────────────┐
│  TaskApp.Core  (o CÉREBRO — lib .NET pura, zero MAUI)    │
│  ├── Models/       entidades de domínio                  │
│  ├── Services/     interfaces + implementações           │
│  └── ViewModels/   estado e comandos da tela             │
│  · roda em qualquer runner — CI, console, o que for      │
└───────────────────────────▲─────────────────────────────┘
                            │ testa em milissegundos ▼
┌─────────────────────────────────────────────────────────┐
│  TaskApp.Core.Tests  (xUnit — 6/6 verdes, sem emulador)  │
└─────────────────────────────────────────────────────────┘
```

O efeito disso não é estético. É que **a maior parte da lógica do app fica coberta por testes que rodam em 14 milissegundos**, em qualquer máquina, sem device farm. Quando o feedback é tão barato, você de fato testa — e a qualidade para de depender de heroísmo manual.

## As decisões, uma a uma

### MVVM com CommunityToolkit.Mvvm
A alternativa seria escrever `INotifyPropertyChanged` na mão em cada ViewModel — dezenas de linhas de boilerplate que não dizem nada sobre o seu domínio. O toolkit gera tudo isso em tempo de compilação a partir de atributos (`[ObservableProperty]`, `[RelayCommand]`). O custo é um acoplamento a uma biblioteca — mas é a biblioteca oficial da Microsoft pra isso, padrão de mercado, então é um acoplamento que eu assino embaixo sem hesitar. **Trade-off aceito conscientemente.**

### Core como projeto separado do MAUI
Poderia ser tudo num projeto só, com pastas. Seria menos arquivo pra gerenciar. Mas aí o "Core" voltaria a depender do MAUI por tabela, e eu perderia a capacidade de testá-lo isolado e de reaproveitá-lo (um head Blazor amanhã usa o mesmo Core sem tocar numa linha). O preço — um projeto a mais — é baixíssimo perto do que ele compra. **Vale cada centavo.**

### Repository atrás de interface
A `TaskListViewModel` não conhece `InMemoryTaskRepository`. Ela conhece `ITaskRepository`, um contrato. Quem decide a implementação concreta é o container de DI, num único lugar. Isso é Inversão de Dependência, e o retorno é direto: trocar persistência (memória → SQLite → API) é mudar **uma linha**, e os testes injetam um fake sem nenhuma cerimônia. O custo é uma indireção a mais pra quem lê o código pela primeira vez — um preço pequeno por um desacoplamento grande.

### Dependency Injection de verdade
A cadeia inteira é montada por injeção: o repositório entra na ViewModel, a ViewModel entra na Page, a Page entra na App. Nenhuma camada constrói suas próprias dependências — todas recebem. É isso que mantém cada peça substituível e testável em isolamento.

### Compiled bindings (`x:DataType`)
Bindings resolvidos em tempo de build, não por reflection em runtime. Mais rápido, e — talvez mais importante — erros de binding viram **erro de compilação** em vez de tela quebrada em produção. O custo é declarar o tipo do contexto em cada escopo de XAML. Disciplina barata, retorno alto.

## A cadeia de dependência, na prática

```
InMemoryTaskRepository ──(ITaskRepository)──► TaskListViewModel ──► TaskListPage ──► App
         registrado em MauiProgram.cs                     resolvido pelo container de DI
```

E é por isso que trocar o banco do app inteiro é isto, e nada mais:

```csharp
// de:
builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
// para:
builder.Services.AddSingleton<ITaskRepository, SqliteTaskRepository>();
```

Uma linha em `MauiProgram.cs`. Nenhuma ViewModel muda. Nenhuma View muda. Nenhum teste muda. Esse é o sinal de que as fronteiras estão no lugar certo: uma mudança grande de comportamento cabe num ponto pequeno do código.

## Onde isso evoluiria num produto real

Este app é deliberadamente enxuto pra deixar a estrutura visível. Num projeto de produção, os próximos passos naturais seriam:

- **Persistência de verdade:** já há um `JsonFileTaskRepository` (persiste em arquivo) provando a troca; o próximo seria um `SqliteTaskRepository` com estratégia offline-first (a especialidade de quem escreve isto) e sincronização com um backend — sem que a ViewModel precise saber que a sincronização existe.
- **Navegação abstraída:** ✅ já implementada — o `INavigationService` deixa a `TaskListViewModel` abrir a tela de edição por intenção (`OpenEditCommand`), sem tocar em Shell/Navigation. Os testes verificam a navegação com um fake, sem UI.
- **Estado explícito:** trocar o `IsBusy` booleano por estados nomeados (`Loading / Loaded / Empty / Error`), eliminando combinações impossíveis.
- **Observabilidade:** logging estruturado e telemetria de uso, pra decidir com dado em vez de achismo.
- **CI:** `dotnet test` em cada PR. Os testes do Core rodam sem emulador, então o sinal é rápido o suficiente pra ninguém mergear regressão. Ver [../guides/testing.md](../guides/testing.md).

Cada um desses encaixa **sem reescrever o que já existe** — porque as fronteiras já estão desenhadas. Esse é o ponto inteiro de pensar arquitetura cedo: não é fazer mais agora, é fazer o agora de um jeito que o depois caiba.
