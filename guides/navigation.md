# 🧭 Navegação — andando entre telas

> Um app é mais de uma tela. Você precisa ir da lista pro detalhe, do detalhe pra edição, e voltar. Navegação é como você faz isso — e o MAUI te dá dois caminhos. Vou explicar os dois e dizer qual escolher.

## A ideia: uma pilha de telas

A metáfora que destrava navegação é a **pilha** (stack). Pense num maço de cartas na mesa. A tela que você vê é a carta do topo. Quando você abre uma nova tela, ela entra **por cima** (push). Quando você volta, a do topo sai (pop) e você vê a de baixo de novo. "Voltar" é literalmente tirar a carta de cima.

```
[ Detalhe ]   ← topo, é o que você vê
[  Lista  ]
[  Home   ]   ← base
```

Abrir Detalhe = empilhar (push). Voltar = desempilhar (pop). Simples assim.

## Caminho 1: `NavigationPage` (o jeito clássico)

Você embrulha sua primeira página numa `NavigationPage`. Ela dá a barra de título com o botão "voltar" e gerencia a pilha.

```csharp
// geralmente no App.xaml.cs ou MauiProgram
MainPage = new NavigationPage(new HomePage());
```

Pra ir pra uma nova tela:
```csharp
await Navigation.PushAsync(new DetalhePage());
```

Pra voltar (além do botão automático):
```csharp
await Navigation.PopAsync();
```

É direto e ótimo pra apps simples ou pra aprender. O `await` está aí porque navegação tem animação — você espera ela terminar.

## Caminho 2: Shell (o recomendado pra apps de verdade)

**Shell** é a forma moderna. Em vez de empilhar páginas na mão, você **registra rotas** (tipo URLs) e navega por nome. Ele também te dá de graça a estrutura de navegação comum: menu lateral (flyout) e abas (tabs).

Você descreve a casca do app no `AppShell.xaml`:
```xml
<Shell xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       x:Class="MeuApp.AppShell">

    <ShellContent Title="Início" ContentTemplate="{DataTemplate local:HomePage}" />
    <ShellContent Title="Perfil" ContentTemplate="{DataTemplate local:PerfilPage}" />
</Shell>
```
Isso sozinho já te dá abas entre "Início" e "Perfil".

Pra telas que não estão na casca (como um detalhe), você registra a rota:
```csharp
Routing.RegisterRoute("detalhe", typeof(DetalhePage));
```
E navega por string:
```csharp
await Shell.Current.GoToAsync("detalhe");
await Shell.Current.GoToAsync("..");   // ".." = voltar (igual subir um nível numa URL)
```

## Passar dados entre telas

Quase sempre você precisa levar uma informação ("abra o detalhe **do produto 42**"). Com Shell, o jeito limpo é por parâmetro de rota:

```csharp
await Shell.Current.GoToAsync($"detalhe?id={produto.Id}");
```

E a página de destino recebe via um atributo que o Shell preenche pra você:
```csharp
[QueryProperty(nameof(Id), "id")]
public partial class DetalhePage : ContentPage
{
    public string Id { get; set; }   // o Shell joga o "42" aqui automaticamente
}
```

Com `NavigationPage`, você passa pelo construtor mesmo: `new DetalhePage(produto)`. Mais simples, porém acopla mais.

## O erro que sabota sua arquitetura

Tem uma armadilha que vale avisar desde já. É tentador chamar a navegação direto de dentro da ViewModel:

```csharp
// ❌ A ViewModel agora depende do MAUI — e perdeu a testabilidade.
await Shell.Current.GoToAsync("detalhe");
```

No momento que a ViewModel chama `Shell.Current` ou `Navigation`, ela voltou a depender da UI, e você não consegue mais testá-la sem o app de pé. O jeito adulto é esconder a navegação atrás de uma interface — um `INavigationService` — que a ViewModel chama por intenção, e cuja implementação real (que mexe no Shell) vive na camada de UI. A ViewModel diz *para onde ir*; a UI sabe *como ir*. (O [guia de arquitetura](./architecture.md) destrincha isso.)

> 💡 Pra **aprender**, navegue direto, sem cerimônia. Quando o app crescer e você quiser testar as ViewModels, refatore pro `INavigationService`. Não trave o aprendizado tentando fazer tudo "certo" no primeiro dia.

## Qual escolher?

- **Aprendendo / app de 2-3 telas:** `NavigationPage`. Menos conceito, resultado rápido.
- **App de verdade, com abas/menu:** **Shell**. É pra onde a plataforma aponta, e a estrutura de navegação vem pronta.

➡️ Próximo: [06 · Estilo e temas](./styling-theming.md) — deixar o app bonito e consistente.
