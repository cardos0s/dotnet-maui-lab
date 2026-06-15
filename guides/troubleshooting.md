# 🩹 Erros comuns — e como sair deles sem chorar

> Todo iniciante em MAUI bate nas mesmas pedras. Aqui estão as mais comuns, o que a mensagem *realmente* quer dizer, e como resolver. Quando travar, procure seu sintoma aqui antes de entrar em pânico.

## "A tela abriu em branco / nada aparece"

**Causa mais provável:** você esqueceu (ou apagou) o `InitializeComponent()` no construtor da página.
```csharp
public MainPage()
{
    InitializeComponent();   // ← isto monta a tela a partir do XAML. Sem ela, tela vazia.
}
```
**Outras causas:** a página tem mais de um filho direto (uma `ContentPage` só aceita **um** — coloque um layout dentro e os elementos dentro do layout), ou o conteúdo está com `IsVisible="False"`, ou com cor de texto igual à cor de fundo (texto branco em fundo branco existe e engana muito).

## "O texto/dado não aparece, mas não dá erro"

Sintoma clássico de **binding quebrado**. O `{Binding Nome}` não achou `Nome` e, por padrão, falha **em silêncio** — não explode, só não mostra nada.

Cheque, nesta ordem:
1. O `BindingContext` está definido? (a página sabe de onde buscar os dados?)
2. O nome bate **exatamente**? `{Binding Nome}` ≠ `{Binding nome}` ≠ `{Binding Nomes}`. Maiúscula importa.
3. A propriedade é **pública**? Binding não enxerga propriedade privada.

**A prevenção definitiva:** adicione `x:DataType` na página (ver [data binding](./data-binding.md)). Aí um binding errado vira **erro de compilação** com o nome do culpado, em vez de tela muda.

## "InitializeComponent não existe" / erros no `.xaml.cs`

Geralmente o XAML tem um erro de sintaxe que impede a geração de código. Procure no XAML:
- Uma tag não fechada (`<Label>` sem `</Label>` ou sem `/>`).
- Um `x:Class` que não bate com o namespace/nome da classe no `.cs`.
- Um caractere especial solto (`&`, `<` dentro de texto — use `&amp;`, `&lt;`).

Corrija o XAML, e o `InitializeComponent` "volta a existir".

## "Esqueci o `partial`"

```
error: partial declarations must not specify different base classes
```
ou erros estranhos sobre a classe da página. A classe da página/ViewModel **precisa** ser `partial`, porque metade dela é gerada (do XAML, ou pelo CommunityToolkit.Mvvm). Adicione `partial`:
```csharp
public partial class MainPage : ContentPage   // ← partial
public partial class MeuViewModel : ObservableObject   // ← partial aqui também
```

## "Meu `[ObservableProperty]` / `[RelayCommand]` não gera nada"

Três checagens:
1. A classe é `partial`? (de novo — é o erro nº 1)
2. A classe herda de `ObservableObject`?
3. O pacote `CommunityToolkit.Mvvm` está instalado no projeto certo?

E lembre da convenção dos nomes: `[ObservableProperty] private string _nome;` gera a propriedade **`Nome`** (PascalCase, sem underscore). `[RelayCommand] Task SalvarAsync()` gera **`SalvarCommand`**. Você bind nos nomes gerados, não nos campos.

## "App não está respondendo" / o app congela

Você bloqueou a UI thread. Procure por `.Result` ou `.Wait()` em chamadas assíncronas:
```csharp
var dados = client.GetStringAsync(url).Result;   // ❌ congela aqui
var dados = await client.GetStringAsync(url);     // ✅
```
**Regra:** chamada assíncrona é sempre `await`. Nunca `.Result`/`.Wait()` na thread de UI. (Detalhes no [guia de performance](./performance.md).)

## "A lista não rola" ou "rola estranho"

Você provavelmente colocou um `CollectionView` (ou `ListView`) dentro de um `ScrollView`. Os dois querem controlar a rolagem e brigam. **A lista já rola sozinha** — tire o `ScrollView` de volta dela.

## "Imagem não aparece"

- O arquivo está em `Resources/Images/`?
- O nome em minúsculas, sem espaço nem acento? (`MinhaFoto.PNG` pode não funcionar; `minha_foto.png` sim.) O MAUI é exigente com nomes de imagem.
- Limpou e recompilou depois de adicionar? Imagens entram no build; às vezes precisa de `dotnet clean` + build.

## "Mudei o código e nada mudou no app"

O clássico build velho. Force uma limpeza:
```bash
dotnet clean
dotnet build
```
Se persistir, apague as pastas `bin/` e `obj/` e rebuilde. Soa grosseiro, mas resolve 90% dos "fantasmas".

## "Erro de build gigante e incompreensível"

Respire. Role o terminal **até o primeiro erro** (não o último — o primeiro). Erros em cascata enchem a tela, mas geralmente há **uma** causa raiz lá em cima, e os outros 40 são consequência. Conserte o de cima e quase sempre os de baixo somem juntos.

## A mentalidade que mais ajuda

Mensagem de erro não é o sistema te xingando — é ele tentando te dizer onde está o problema, do jeito truncado dele. **Leia a mensagem inteira, com calma, e procure o nome do arquivo e a linha.** Copie a parte central da mensagem e jogue no [Stack Overflow](https://stackoverflow.com/) ou na [doc da Microsoft](https://learn.microsoft.com/dotnet/maui/). Você quase nunca é a primeira pessoa a ver aquele erro — alguém já perguntou, alguém já respondeu.

➡️ Não entendeu um termo? Vá pro [glossário](./glossary.md).
