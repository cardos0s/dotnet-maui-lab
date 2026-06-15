# 🧩 XAML do zero — a linguagem das telas

> XAML parece complicado porque é cheio de `<`, `>` e palavras compridas. Mas a ideia por trás é simples, e quando ela cai a ficha o resto vira detalhe. Vamos com calma.

## O que é XAML, de verdade

**XAML** (lê-se "zâmel") é uma forma de **descrever uma tela usando marcação**, parecida com HTML. Em vez de escrever em C# "crie um botão, defina o texto, defina a cor, adicione na tela", você *descreve* como a tela é:

```xml
<Button Text="Clica aqui" TextColor="White" />
```

Cada elemento XAML (`<Button>`, `<Label>`...) corresponde a uma classe C# de verdade (a classe `Button`, a classe `Label`). XAML é só um jeito mais visual e declarativo de criar e configurar esses objetos. Você *poderia* fazer tudo em C# puro — mas pra descrever telas, marcação é mais legível.

A regra mental: **XAML = "o quê" (a aparência). C# code-behind = "o que acontece" (o comportamento).**

## Anatomia de um elemento

```xml
<Label Text="Olá, mundo!" FontSize="24" TextColor="Purple" />
```

- `<Label ... />` — o **elemento**. Equivale a `new Label()`.
- `Text`, `FontSize`, `TextColor` — **propriedades** (ou "atributos"). Equivalem a `label.Text = "..."`.
- O `/>` no fim fecha o elemento. Quando ele não tem filhos, fecha assim, com a barra.

Quando um elemento **tem filhos** (outros elementos dentro), ele abre e fecha separado:

```xml
<VerticalStackLayout>
    <Label Text="Primeira linha" />
    <Label Text="Segunda linha" />
</VerticalStackLayout>
```

Aqui o `VerticalStackLayout` é um **container** — ele empilha o que está dentro, de cima pra baixo. Os dois `Label` são filhos dele.

## A estrutura de uma página

Toda página começa parecida com isto:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MeuApp.MainPage">

    <VerticalStackLayout Padding="20" Spacing="10">
        <Label Text="Bem-vinda!" FontSize="32" />
        <Button Text="Começar" />
    </VerticalStackLayout>

</ContentPage>
```

Vamos por partes, porque essas primeiras linhas assustam:

- **`<ContentPage>`** — é a tela. Quase toda tela é uma `ContentPage`. Repara que ela só tem **um filho direto** (o `VerticalStackLayout`). Isso é uma regra: uma página tem um conteúdo só. Pra ter vários elementos, você coloca um container dentro, e os elementos dentro do container.
- **`xmlns=...`** — são os **namespaces**. Aquele monte de URL parece importante e assustador, mas é só o XAML dizendo "de onde vêm esses elementos". O `xmlns` padrão traz `Button`, `Label`, etc. O `xmlns:x` traz coisas utilitárias (como o `x:Class`). **Você copia e cola e esquece** — só vai mexer aqui quando precisar de um namespace extra (e o guia de [data binding](./data-binding.md) mostra quando).
- **`x:Class="MeuApp.MainPage"`** — liga este XAML ao arquivo C# (o code-behind) `MainPage.xaml.cs`. É a costura entre o "o quê" e o "o que acontece".

## O code-behind: o outro lado da página

Pra cada `MinhaPagina.xaml` existe um `MinhaPagina.xaml.cs`. É o C# daquela tela:

```csharp
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();   // ← lê o XAML e monta a tela. NUNCA apague isto.
    }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        // o que acontece quando o botão é clicado
    }
}
```

Duas coisas pra gravar:
- **`partial`** — a classe é "partida": metade vem do que você escreve, metade é gerada a partir do XAML. Por isso `partial`.
- **`InitializeComponent()`** — é a linha que lê o XAML e constrói a tela de fato. Se você apagar, a tela vem em branco. Não apague.

## Ligando um clique (o jeito iniciante)

No XAML, você diz qual método chamar quando o botão é clicado:

```xml
<Button Text="Clica" Clicked="OnButtonClicked" />
```

E no code-behind, você escreve esse método:

```csharp
private void OnButtonClicked(object sender, EventArgs e)
{
    DisplayAlert("Oi", "Você clicou!", "OK");
}
```

Isso funciona, e é perfeito pra aprender. **Mas guarde uma ressalva pro futuro:** colocar lógica no code-behind do clique é o jeito iniciante. Quando o app cresce, isso vira o "1.200 linhas de code-behind impossível de testar" que o [guia de arquitetura](./architecture.md) descreve. O caminho adulto é **MVVM com data binding** — mas uma coisa de cada vez. Aprenda o clique primeiro; troque por binding quando o clique não doer mais.

## Comentários em XAML

Vai precisar:

```xml
<!-- isto é um comentário em XAML -->
```

## O "click" que você precisa ter

XAML não é uma linguagem nova e estranha — é só **objetos C# descritos com marcação**. `<Button Text="X"/>` é `new Button { Text = "X" }`. Quando você lê XAML pensando "isso é só um objeto sendo criado e configurado", o medo passa. O resto é vocabulário: aprender quais elementos existem e quais propriedades cada um tem — e é exatamente isso que o próximo guia cobre.

➡️ Próximo: [03 · Os controles essenciais](./controls.md) — o vocabulário das telas, com exemplos.
