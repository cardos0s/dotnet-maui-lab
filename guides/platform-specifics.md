# 🧬 Código por plataforma — quando "um código só" precisa de exceções

> A promessa do MAUI é escrever uma vez e rodar em todo lugar. E é verdade — em 95% do código. Mas vai chegar o dia em que você precisa de algo *diferente* no Android e no iOS, ou de uma API nativa que o MAUI não embrulha. Este guia é sobre esses 5%, sem perder a sanidade.

## Primeiro: você provavelmente NÃO precisa disto

Antes de escrever código por plataforma, desconfie. A maioria das diferenças visuais se resolve com ferramentas que já existem e são mais limpas:

- **`OnPlatform`** no XAML — um valor diferente por plataforma, declarativo:
  ```xml
  <Label FontSize="{OnPlatform iOS=18, Android=16, Default=16}" />

  <!-- útil pra espaçamento: iOS tem notch, Android não -->
  <ContentPage Padding="{OnPlatform iOS='0,40,0,0', Default='0,16,0,0'}" />
  ```
- **`OnIdiom`** — diferente por *tipo de aparelho* (celular vs tablet):
  ```xml
  <Grid ColumnDefinitions="{OnIdiom Phone='*', Tablet='*,*'}" />
  ```

Se a sua necessidade é "um número diferente por plataforma", `OnPlatform` resolve sem você escrever nenhum `if`. Comece sempre por aqui.

## Quando você precisa de C# diferente: `#if`

Pra blocos de código que só existem numa plataforma, há a **compilação condicional**. O `#if` inclui o código só quando compilando pra aquela plataforma:

```csharp
#if ANDROID
    // este bloco só existe no build Android
    var contexto = Platform.CurrentActivity;
#elif IOS
    // este só no iOS
    var janela = UIKit.UIApplication.SharedApplication.KeyWindow;
#endif
```

Os símbolos (`ANDROID`, `IOS`, `WINDOWS`, `MACCATALYST`) o MAUI define automaticamente conforme o alvo. É poderoso, mas **use com parcimônia** — `#if` espalhado vira código difícil de ler e testar (cada plataforma é um caminho diferente que você precisa verificar). Se você está escrevendo muito `#if`, provavelmente é hora do próximo padrão.

## O jeito limpo: classes parciais por plataforma

Quando a diferença é grande (uma implementação inteira muda por plataforma), o padrão elegante é uma **classe parcial** com um arquivo por plataforma. Você declara o contrato uma vez e implementa em arquivos separados que o MAUI compila só na plataforma certa:

```
Services/
├── DeviceInfoService.cs              # a parte comum / a assinatura
├── DeviceInfoService.Android.cs      # implementação Android (compila só no Android)
└── DeviceInfoService.iOS.cs          # implementação iOS
```

O MAUI tem uma convenção: arquivos numa pasta `Platforms/Android/` ou com sufixo `.Android.cs` entram **só** no build do Android. Assim cada implementação é um arquivo limpo, sem `#if`, e a parte comum não sabe que existem variações. É a versão organizada do `#if`.

## Acessando API nativa direto

Às vezes você precisa de algo que nem o MAUI nem o Essentials embrulham — uma API específica do Android, por exemplo. O MAUI permite chamar a API nativa direto de dentro do bloco condicional, porque no build Android você tem acesso a todo o SDK do Android em C#:

```csharp
#if ANDROID
    var vibrador = (Android.OS.Vibrator)Platform.CurrentActivity
        .GetSystemService(Android.Content.Context.VibratorService);
#endif
```

É o "escape hatch": quando a abstração não cobre, você desce ao nativo. Raro, mas bom saber que a porta existe — você nunca fica preso por uma limitação da abstração.

## Handlers: customizar um controle por plataforma

Quer que o `Entry` não tenha aquela linha embaixo no Android? Isso é customização de baixo nível, e o caminho é o **Handler** (a peça que liga o controle MAUI ao nativo). Você ajusta o controle nativo via *mapper*, sem subclassar:

```csharp
// em MauiProgram.cs, remove a borda inferior do Entry no Android
Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("SemBorda", (handler, view) =>
{
#if ANDROID
    handler.PlatformView.Background = null;
#endif
});
```

(Se você vem do Xamarin: isto substitui os antigos *Custom Renderers*, e é bem mais leve.)

## A disciplina que mantém isso são

O risco do código por plataforma é ele virar um cipoal de exceções que ninguém entende. Três regras pra evitar:

1. **Tente `OnPlatform`/`OnIdiom` primeiro.** Resolve a maioria sem ramificar lógica.
2. **Isole a diferença atrás de uma interface.** A ViewModel pede `IDeviceInfoService`; ela não sabe que existem três implementações. O resto do app continua plataforma-agnóstico e testável.
3. **Concentre o `#if` em poucos arquivos.** Não espalhe pela base — um `#if` num serviço isolado é controlável; cem `#if` espalhados pelas ViewModels é caos.

## O resumo

"Um código só" não é uma mentira — é um padrão com escapes bem definidos. Comece por `OnPlatform`/`OnIdiom` (declarativo, limpo). Suba pra classes parciais por plataforma quando a diferença é grande. Use `#if` e API nativa como último recurso, sempre isolado atrás de uma interface. Assim você aproveita a portabilidade do MAUI **e** acessa todo o poder nativo quando precisa — sem transformar o app num labirinto.

➡️ Relacionado: [recursos do dispositivo](./device-features.md) · [arquitetura](./architecture.md) (esconder a diferença atrás de interface).
