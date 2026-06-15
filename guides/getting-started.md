# 🚀 Começando do zero — seu primeiro app .NET MAUI

> Se você nunca abriu o MAUI na vida, é por aqui. Vou assumir que você sabe *alguma* coisa de C# (variável, método, classe) e nada de MAUI. No fim deste guia você vai ter um app rodando na sua tela e vai entender cada arquivo que apareceu.

## O que é .NET MAUI, em uma frase honesta

**MAUI** (Multi-platform App UI) é o jeito da Microsoft de você escrever **um código C# e XAML e rodar ele como app nativo no Android, iOS, Windows e macOS.** Em vez de aprender Kotlin pro Android e Swift pro iPhone, você escreve uma vez. É o sucessor do Xamarin.Forms — se você ouvir falar de Xamarin, pense "MAUI antigo".

A palavra "nativo" importa: não é um site embrulhado num app. Os botões são botões de verdade do sistema, a performance é de app nativo. Você só não precisa escrever duas vezes.

## O que você precisa instalar

Você precisa de três coisas. Não pule nenhuma.

1. **O .NET SDK** (versão 8 ou mais nova). É o kit que compila e roda código .NET. Baixe em [dot.net](https://dotnet.microsoft.com/download).

2. **O workload do MAUI.** O SDK vem "pelado"; o workload é o pacote extra que ensina ele a fazer apps MAUI. Depois de instalar o SDK, abra o terminal e rode:
   ```bash
   dotnet workload install maui
   ```
   Isso baixa um bom tanto de coisa (os SDKs de Android, etc.) — pode demorar. É normal.

3. **Um editor.** Suas opções:
   - **Visual Studio** (Windows/Mac) — o caminho mais fácil pra MAUI, já vem com tudo integrado.
   - **VS Code** com a extensão **.NET MAUI** — mais leve, funciona em qualquer SO.
   - **Rider** (JetBrains) — ótimo, pago.

4. **Um lugar pra rodar o app:**
   - **Android:** um emulador (vem com o workload) ou seu celular no modo desenvolvedor.
   - **Windows:** roda direto na sua máquina, sem emulador. (É o jeito mais rápido de testar visualmente.)
   - **iOS/Mac:** precisa de um Mac. iPhone precisa de Mac + conta Apple.

> 💡 **Dica de iniciante:** comece testando no **Windows** (ou no Mac Catalyst, se você está no Mac). Emulador de Android é lento e some o foco. Faça o app funcionar na máquina primeiro, depois leve pro celular.

## Confirmando que deu certo

Antes de criar qualquer coisa, confirme que o ambiente está de pé:

```bash
dotnet --version          # deve mostrar a versão do SDK
dotnet workload list      # deve listar maui-android, maui-ios, etc.
```

Se os dois respondem, você está pronto. Se `workload list` vem vazio, volte no passo 2.

## Criando o primeiro app

```bash
dotnet new maui -n MeuPrimeiroApp
cd MeuPrimeiroApp
```

O comando `dotnet new maui` cria um projeto a partir de um template — o esqueleto de um app que já compila e roda. O `-n` é o nome. Pronto, você tem um app.

## Rodando

```bash
# no Windows:
dotnet build -t:Run -f net9.0-windows10.0.19041.0

# no Mac:
dotnet build -t:Run -f net9.0-maccatalyst

# no Android (com emulador aberto):
dotnet build -t:Run -f net9.0-android
```

O `-f` (framework) diz pra qual plataforma você quer rodar. A primeira vez demora (ele compila tudo); depois é mais rápido. Vai aparecer uma tela com um botão "Click me" e um contador. Clica nele. Funcionou? Você é oficialmente uma dev MAUI.

## Os arquivos que apareceram — o tour

Abrir um projeto MAUI pela primeira vez assusta pela quantidade de arquivo. Calma, a maioria você quase nunca toca. Os que importam:

| Arquivo/pasta | O que é |
|---|---|
| **`MauiProgram.cs`** | O ponto de partida do app. Aqui você registra serviços (DI), fontes, etc. É o "liga tudo". |
| **`App.xaml` / `App.xaml.cs`** | A aplicação em si. Define recursos globais (cores, estilos) e qual janela/página abre primeiro. |
| **`AppShell.xaml`** | A "casca" de navegação — define a estrutura de telas e menus. |
| **`MainPage.xaml`** | Uma página (tela). O `.xaml` é o layout; o `.xaml.cs` (code-behind) é o C# daquela tela. |
| **`Resources/`** | Imagens, fontes, ícones, cores e estilos do app. |
| **`Platforms/`** | Código específico de cada sistema (Android, iOS...). Você raramente mexe aqui no começo. |

A dupla `.xaml` + `.xaml.cs` é o padrão que você vai ver o tempo todo: **um arquivo descreve a aparência (XAML), o outro descreve o comportamento (C#).** Eles são a mesma página, partidos em dois. Entender isso é o primeiro grande "click".

## Próximo passo

Agora que você tem um app rodando, o próximo passo é entender a linguagem das telas — o **XAML**. É pra lá que a gente vai: [02 · XAML do zero](./xaml-basics.md).

> 🗺️ Perdida na ordem das coisas? O [índice de guias](./README.md) tem a trilha completa, do iniciante ao avançado.
