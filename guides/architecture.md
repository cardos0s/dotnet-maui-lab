# 🏛️ Arquitetura em .NET MAUI — um guia de quem já apanhou

> Este não é um guia de "como fazer um Hello World". É sobre como estruturar um app que **três pessoas vão tocar ao mesmo tempo daqui a um ano**, sem que cada feature nova vire uma cirurgia de risco. A implementação completa do que descrevo aqui está no [reference-app](../reference-app).

## O problema que ninguém te conta no começo

Todo app MAUI começa lindo. Você arrasta um `Button`, escreve o `Clicked` no code-behind da página, chama a API ali mesmo, atualiza um `Label`. Funciona. Você entrega. Todo mundo feliz.

O problema aparece no terceiro mês. A tela de checkout agora tem 1.200 linhas de code-behind. Tem lógica de cálculo de frete misturada com `await DisplayAlert`. Quando o QA acha um bug no cálculo do desconto, você não consegue escrever um teste pra ele — porque o cálculo está enterrado dentro de um handler de clique que depende de um `Entry.Text` e de um pop-up nativo. Pra testar, você teria que **subir o app inteiro no emulador, navegar até a tela, digitar nos campos e clicar**. Ninguém faz isso. Então ninguém testa. Então o bug volta.

Arquitetura não é sobre "ser elegante". É sobre **manter a velocidade de entrega constante ao longo do tempo** em vez de ver ela despencar a cada feature. Tudo que vem a seguir existe pra resolver essa dor específica.

## A decisão que paga mais: separar o cérebro da cara

A regra mais valiosa que existe em MAUI — e a que mais gente ignora — é esta: **a lógica do seu app não pode depender do MAUI.**

Pensa assim. O MAUI é a *cara* do app: botões, listas, animações, navegação. Mas o *cérebro* — "o que acontece quando o usuário adiciona uma tarefa", "como eu calculo quantas faltam", "o que fazer quando a API falha" — isso não tem nada a ver com botão nenhum. Esse cérebro poderia rodar num app de console, num site Blazor, num teste automatizado. Ele só não sabe disso porque você o algemou dentro de um `ContentPage`.

Então a gente separa fisicamente, em dois projetos:

```
App.Core   →  o cérebro. Uma lib .NET PURA. Models, Services, ViewModels.
              NÃO referencia o MAUI. Roda em qualquer lugar.
App.Maui   →  a cara. Views em XAML, navegação, injeção de dependência.
              Referencia o Core. Só roda em device/emulador.
```

"Mas Julia, por que dois projetos pra um app pequeno? Não é over-engineering?"

É a pergunta certa, e a resposta é o coração de tudo: **porque o `App.Core` você consegue testar em milissegundos, e o `App.Maui` você não.** Build de MAUI precisa de workload, de SDK de Android, de emulador. Build de uma lib .NET pura precisa de... nada. Roda no seu CI, num runner Linux pelado, em 3 segundos. Quando 80% da sua lógica vive no Core, 80% do seu app fica coberto por testes rápidos e confiáveis. Esse é o jogo inteiro.

E tem um bônus: no dia em que o produto quiser um portal web, seu Core inteiro — regras, ViewModels, serviços — é reaproveitado por um head Blazor. Você não reescreve nada. Você só desenha telas novas.

## MVVM: o padrão, sem o sofrimento

MVVM (Model-View-ViewModel) assusta as pessoas porque a primeira versão que todo mundo aprende é cheia de cerimônia: implementar `INotifyPropertyChanged` na mão, escrever `RaisePropertyChanged` em cada setter, criar uma classe `RelayCommand` do zero. É chato e ninguém merece.

A boa notícia: **o `CommunityToolkit.Mvvm` mata 90% desse boilerplate** usando geração de código em tempo de compilação. Você escreve um campo com um atributo, e o compilador escreve a propriedade observável pra você. Olha:

```csharp
public partial class TaskListViewModel : ObservableObject
{
    [ObservableProperty]                            // ← isto gera a propriedade pública NewTitle,
    [NotifyCanExecuteChangedFor(nameof(AddTaskCommand))]  //   com notificação de mudança,
    private string _newTitle = string.Empty;        //   e ainda reavalia o CanExecute do comando

    [RelayCommand(CanExecute = nameof(CanAdd))]     // ← isto gera um AddTaskCommand completo
    private async Task AddTaskAsync() { /* ... */ }

    private bool CanAdd() => !string.IsNullOrWhiteSpace(NewTitle);
}
```

Repara em três coisas que importam:

1. **A classe é `partial`.** Não é opcional. O gerador de código precisa "completar" sua classe com o que ele escreveu. Esqueceu o `partial`? O build quebra com uma mensagem que parece bug, mas é só isso.

2. **`[ObservableProperty]` vai no campo `_newTitle`, e gera a propriedade `NewTitle`.** Convenção: campo privado com underscore vira propriedade pública PascalCase. Você bind na `NewTitle` no XAML, mexe no `NewTitle` no código.

3. **`[NotifyCanExecuteChangedFor]` é o detalhe sênior.** Sem ele, o botão "Adicionar" não saberia que precisa reabilitar quando o usuário digita. Com ele, toda vez que `NewTitle` muda, o `AddTaskCommand` reavalia se pode executar. É a diferença entre uma UI que "responde" e uma que parece travada.

O que era 40 linhas de boilerplate viram 6 linhas de intenção pura. Esse é o ganho.

## Injeção de Dependência: o cano que liga tudo

Aqui mora a parte que separa código júnior de código sênior. A pergunta é: **quando a `TaskListViewModel` precisa de um repositório pra salvar tarefas, quem cria esse repositório?**

A resposta júnior é: a própria ViewModel. `var repo = new SqliteTaskRepository();` lá dentro. Funciona, mas amarra a ViewModel a uma implementação concreta pra sempre. Quer trocar SQLite por uma API? Edita a ViewModel. Quer testar a ViewModel sem um banco real? Não dá — ela cria o banco sozinha.

A resposta sênior é: **a ViewModel não cria nada. Ela pede.** No construtor, ela declara "eu preciso de um `ITaskRepository`" — uma *interface*, não uma classe. E um terceiro, o container de DI, decide qual implementação entregar:

```csharp
// Em MauiProgram.cs — o único lugar que conhece as implementações concretas:
builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
builder.Services.AddSingleton<TaskListViewModel>();
builder.Services.AddSingleton<TaskListPage>();
```

Isso é **Inversão de Dependência** (o "D" do SOLID), e o efeito prático é lindo: trocar a persistência do app inteiro é mudar **uma linha**. De `InMemoryTaskRepository` pra `SqliteTaskRepository`. A ViewModel não percebe. A View não percebe. Os testes não percebem. Você mexe num lugar, o app inteiro muda de comportamento.

E nos testes? Você simplesmente passa um fake na mão: `new TaskListViewModel(new InMemoryTaskRepository())`. Sem container, sem mágica. A ViewModel aceita qualquer coisa que cumpra o contrato `ITaskRepository`.

Uma palavra sobre **ciclo de vida**, que pega gente experiente:
- **Singleton** — uma instância pro app todo. Bom pra serviços com estado/cache (um repositório, um cliente HTTP).
- **Transient** — uma instância nova a cada pedido. Bom pra ViewModels de telas que reabrem e devem começar limpas.

Registrar uma ViewModel com estado como Singleton e depois se perguntar "por que a tela voltou com os dados velhos?" é um clássico. Pense no ciclo de vida como parte do design, não como detalhe.

## Navegação: o vazamento silencioso

Tem um erro que quase todo app comete e que sabota a separação que você tanto cuidou: **navegar de dentro da ViewModel com código de UI.**

```csharp
// ❌ Isto vaza o MAUI pra dentro do seu cérebro testável:
await Navigation.PushAsync(new DetailPage());
```

No momento em que a ViewModel chama `Navigation.PushAsync` ou instancia uma `Page`, ela voltou a depender do MAUI — e perdeu a testabilidade que era o objetivo. A correção é abstrair a navegação atrás de uma interface (`INavigationService`) que a ViewModel chama por intenção ("vá para os detalhes da tarefa X"), ou usar o **Shell** com rotas registradas e navegar por string. A View resolve *como* navegar; a ViewModel só diz *para onde*.

## Estado de UI: pare de mentir com booleanos

`IsBusy = true/false` é o começo, mas é uma simplificação perigosa. Uma tela real tem mais estados: está carregando? carregou e tem dados? carregou e está vazia? deu erro? Cada um pede uma UI diferente, e tentar representar isso com dois ou três booleanos soltos (`IsBusy`, `IsEmpty`, `HasError`) gera combinações impossíveis — o que acontece quando `IsBusy` e `HasError` são ambos `true`?

O jeito sênior é modelar o estado como **uma coisa só e explícita**: um enum `Loading / Loaded / Empty / Error`, ou um `Result<T>`. A UI reage a um estado bem definido por vez, e os testes ficam triviais ("dado que o repositório falhou, o estado vira `Error`"). Estado explícito é menos bug e menos `if` aninhado.

## O mapa, pra fechar

| Camada | Responsabilidade | Conhece o MAUI? |
|---|---|---|
| **Models** | Entidades e regras de domínio | ❌ |
| **Services** | Persistência, rede, dispositivo — sempre atrás de interface | ❌ |
| **ViewModels** | Estado e comandos da tela | ❌ |
| **Views (XAML)** | Layout e binding, nada de regra de negócio | ✅ |
| **DI / App** | Composição — o único lugar que amarra concreto com abstrato | ✅ |

A linha que divide o ❌ do ✅ é a fronteira mais importante do seu app. Tudo acima dela você testa em segundos. Tudo abaixo você desenha e olha no olho. Mantenha essa linha nítida e o resto se resolve.

➡️ Continua em: [como testar tudo isso](./testing.md) · [layout que não trava](./layout.md) · [performance de verdade](./performance.md)
