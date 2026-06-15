# 🔤 Fontes & ícones — a identidade visual que parece profissional

> A fonte padrão do sistema e os ícones genéricos gritam "template". Trocar por uma tipografia própria e um conjunto de ícones consistente é um dos jeitos mais baratos de um app parecer caro. E é mais simples do que parece.

## Fontes customizadas

### Passo 1: colocar o arquivo no projeto
Os arquivos de fonte (`.ttf` ou `.otf`) vão na pasta `Resources/Fonts/`. Baixe de lugares como o [Google Fonts](https://fonts.google.com/) (gratuito e seguro pra uso comercial).

### Passo 2: registrar no `MauiProgram.cs`
Aqui você dá um **apelido** pra fonte, que vai usar no resto do app:

```csharp
.ConfigureFonts(fonts =>
{
    fonts.AddFont("Inter-Regular.ttf", "InterRegular");
    fonts.AddFont("Inter-Bold.ttf", "InterBold");
});
```

O primeiro argumento é o nome do arquivo; o segundo é o apelido. Esse apelido é o que importa daqui pra frente.

### Passo 3: usar
```xml
<Label Text="Olá" FontFamily="InterRegular" />
<Label Text="Título" FontFamily="InterBold" FontSize="28" />
```

### A jogada profissional: definir como padrão global
Você não quer escrever `FontFamily` em todo Label. Defina um estilo implícito no `App.xaml` (ver [estilo e temas](./styling-theming.md)) e o app inteiro herda a sua tipografia:

```xml
<Style TargetType="Label">
    <Setter Property="FontFamily" Value="InterRegular" />
</Style>
```

Pronto: todo texto do app nasce com a sua fonte, e você só especifica quando quer fugir do padrão (um título em bold, por exemplo). Tipografia consistente é metade da percepção de "app bem feito".

## Ícones — o jeito que escala

Aqui mora uma decisão importante. Você tem duas formas de colocar ícones, e a escolha afeta qualidade e manutenção.

### Opção A: imagens (PNG/SVG)
Cada ícone é um arquivo em `Resources/Images/`. Funciona, mas:
- PNG **pixeliza** quando ampliado e você precisa de vários tamanhos.
- SVG no MAUI escala bem, mas cada ícone é um arquivo, e mudar a cor exige editar o arquivo.

Bom pra ilustrações e logos. Pra ícones de interface (lixeira, engrenagem, lupa), tem coisa melhor.

### Opção B: fonte de ícones (icon font) — a escolha profissional
Uma **fonte de ícones** é um arquivo de fonte onde cada "letra" é um ícone (lixeira, coração, seta...). Bibliotecas famosas: **Font Awesome**, **Material Icons**. A vantagem é enorme:
- **Escala perfeita** em qualquer tamanho (é vetorial, como texto).
- **Muda de cor** com `TextColor`, como qualquer texto — sem editar arquivo.
- **Um arquivo só** pra centenas de ícones.

Você registra como uma fonte normal:
```csharp
fonts.AddFont("fa-solid-900.ttf", "FontAwesome");
```

E usa o ícone pelo seu código (cada ícone tem um código Unicode, que a biblioteca documenta):
```xml
<Label Text="&#xf2ed;"          
       FontFamily="FontAwesome"
       TextColor="{StaticResource Primaria}" />   <!-- ícone de lixeira, na cor da marca -->
```

A jogada de manutenção: em vez de espalhar `&#xf2ed;` pelo XAML (ilegível), defina os ícones como [recursos](./styling-theming.md) com nomes:
```xml
<x:String x:Key="IconeLixeira">&#xf2ed;</x:String>
```
```xml
<Label Text="{StaticResource IconeLixeira}" FontFamily="FontAwesome" />
```
Agora você lê "IconeLixeira" no código, não um hexadecimal misterioso.

> 💡 **Não esqueça da acessibilidade:** ícone sozinho não diz nada pra um leitor de tela. Todo ícone interativo precisa de `SemanticProperties.Description` (ver [acessibilidade](./accessibility.md)).

## Ícone do app e splash (a primeira impressão)

Diferente dos ícones de interface, o **ícone do app** (o que aparece na gaveta de apps) e a **splash screen** têm um lugar especial:
- Ícone → `Resources/AppIcon/` (um SVG; o MAUI gera todos os tamanhos automaticamente).
- Splash → `Resources/Splash/` (também SVG).

Você fornece um SVG de cada e o MAUI cuida de gerar as dezenas de variações que Android e iOS exigem. Detalhes na hora de publicar estão no [guia de publicação](./publishing.md).

## O resumo

Tipografia própria definida como **padrão global** + **fonte de ícones** (vetorial, colorível, um arquivo só) com os ícones nomeados como recursos = um app que parece desenhado por gente, não montado de template. É barato de fazer e é dos primeiros detalhes que diferenciam visualmente um app amador de um profissional.

➡️ Relacionado: [estilo e temas](./styling-theming.md) · [acessibilidade](./accessibility.md) · [publicação](./publishing.md).
