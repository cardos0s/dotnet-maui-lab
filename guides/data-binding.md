# 🔗 Data Binding — ligando a tela aos seus dados

> Este é o conceito que separa "mexer no MAUI" de "entender o MAUI". É também o que mais confunde no começo, porque é meio mágico. Vou desfazer a mágica devagar.

## O problema que o binding resolve

Imagine uma tela com o nome do usuário. Sem binding, pra mostrar o nome você faria assim no code-behind:

```csharp
nomeLabel.Text = usuario.Nome;
```

E se o nome mudar depois? Você teria que lembrar de rodar essa linha de novo. E se ele aparece em três lugares? Três linhas, toda vez. E se o usuário digita num campo e você quer ler de volta? Mais código manual, nos dois sentidos. Telas reais têm dezenas desses, e manter tudo sincronizado na mão é onde os bugs nascem.

**Data binding é uma conexão automática entre uma propriedade da tela e uma propriedade de um objeto.** Você liga uma vez, e o MAUI mantém os dois em sincronia sozinho. O nome mudou no objeto? O Label atualiza. O usuário digitou no campo? O objeto atualiza. Você para de carregar dado na mão.

## A sintaxe: `{Binding}`

```xml
<Label Text="{Binding Nome}" />
```

Aquele `{Binding Nome}` quer dizer: *"o texto deste Label vem da propriedade `Nome` do meu contexto de dados."* Não é o texto literal — é uma instrução de "vá buscar `Nome` lá".

A pergunta óbvia: **buscar `Nome` de onde?** É aí que entra a peça que falta.

## O `BindingContext` — o "de onde"

Todo elemento tem um **`BindingContext`**: o objeto de onde os bindings dele buscam os dados. Quando você escreve `{Binding Nome}`, o MAUI vai no `BindingContext` daquele elemento e procura uma propriedade `Nome`.

Você define o `BindingContext` geralmente no code-behind da página:

```csharp
public MainPage()
{
    InitializeComponent();
    BindingContext = new Usuario { Nome = "Julia" };
}
```

Agora, na tela:
```xml
<Label Text="{Binding Nome}" />   <!-- mostra "Julia" -->
```

O Label foi no `BindingContext` (o objeto `Usuario`), achou a propriedade `Nome`, e mostrou "Julia". **Essa é a mágica inteira:** `{Binding X}` = "pegue `X` do meu BindingContext".

Um detalhe que economiza horas: o `BindingContext` é **herdado pelos filhos**. Se você define no `ContentPage`, todos os Labels, Buttons e Entries dentro dela usam o mesmo contexto. Você define uma vez, a tela toda enxerga.

## O elo que falta: como a tela "sabe" que mudou

Aqui mora a parte que quebra a cabeça de todo iniciante. Você faz isto:

```csharp
BindingContext = usuario;
// ... mais tarde ...
usuario.Nome = "Maria";   // mudei o nome... mas o Label NÃO atualizou. Por quê?!
```

O Label não atualiza porque o objeto **não avisou ninguém** que mudou. O binding é automático, mas ele precisa de um "alô, mudei". Esse "alô" é uma interface chamada **`INotifyPropertyChanged`**. Um objeto que a implementa dispara um evento toda vez que uma propriedade muda, e o binding está ouvindo esse evento.

Implementar isso na mão é chato (um monte de boilerplate). Por isso, na prática, **a gente nunca implementa na mão** — usa o `CommunityToolkit.Mvvm`, que gera tudo:

```csharp
public partial class UsuarioViewModel : ObservableObject
{
    [ObservableProperty]      // ← isto gera a propriedade Nome JÁ com o "alô" embutido
    private string _nome = "Julia";
}
```

Agora sim: mudou `Nome`, o Label atualiza sozinho. Esse objeto — que segura os dados da tela e avisa quando mudam — é a **ViewModel**, e é a base do padrão MVVM (que o [guia de arquitetura](./architecture.md) aprofunda).

## Mão dupla: `Mode`

Bindings têm direção:

- **`OneWay`** (padrão da maioria) — dados vão do objeto → tela. Um Label só mostra; ele não muda o objeto.
- **`TwoWay`** — vai e volta. Um `Entry` precisa disso: o que o usuário digita volta pro objeto.

```xml
<Entry Text="{Binding Nome, Mode=TwoWay}" />
```
Na prática, controles de input (`Entry`, `Switch`, `Slider`) já usam `TwoWay` por padrão — então você raramente escreve `Mode` à mão. Mas é bom saber que existe, pra quando precisar forçar uma direção.

## Comandos: o clique, do jeito adulto

Lembra do `Clicked="OnButton"` no code-behind do [guia de XAML](./xaml-basics.md)? O jeito MVVM de fazer a mesma coisa é **bindar um comando**:

```xml
<Button Text="Salvar" Command="{Binding SalvarCommand}" />
```

E na ViewModel:
```csharp
[RelayCommand]
private async Task SalvarAsync()
{
    // a lógica do salvar — aqui, na ViewModel testável, não no code-behind
}
```

A diferença não é cosmética: agora a lógica do botão está na ViewModel, que você **testa sem abrir o app** (ver [guia de testes](./testing.md)). O `[RelayCommand]` gera o `SalvarCommand` a partir do método `SalvarAsync`. (Convenção: método `XxxAsync` vira `XxxCommand`.)

## Compiled bindings: o `x:DataType` (não pule isto)

Por padrão, o binding descobre as propriedades em tempo de execução, usando reflection — que é flexível mas lento, e some os erros pra produção. Você corrige declarando o tipo do contexto com `x:DataType`:

```xml
<ContentPage xmlns:vm="clr-namespace:MeuApp.ViewModels"
             x:DataType="vm:UsuarioViewModel">
    <Label Text="{Binding Nome}" />
</ContentPage>
```

Com isso, o binding é resolvido em tempo de compilação: mais rápido, e se você escrever `{Binding Nonme}` (com erro de digitação), o **build falha** em vez de a tela vir vazia em silêncio. É a diferença entre achar o erro em 2 segundos e caçar ele por uma hora. Pegue esse hábito cedo — o [guia de performance](./performance.md) explica o ganho em detalhe.

## Recapitulando a corrente toda

```
ViewModel (com [ObservableProperty])  ──avisa quando muda──►  Binding  ──atualiza──►  Tela
       ▲                                                                                  │
       └──────────────── usuário digita / clica (TwoWay / Command) ◄──────────────────────┘
```

Quando essa imagem faz sentido — objeto que avisa, binding que escuta, tela que reage, e tudo de volta — você "entendeu o MAUI". O resto da sua jornada é aplicar isso com capricho.

➡️ Próximo: [05 · Navegação entre telas](./navigation.md).
