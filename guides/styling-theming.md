# 🎨 Estilo e temas — deixando o app bonito (e consistente)

> Dá pra colorir cada botão na mão. Mas aí você muda a cor da marca e tem que editar 80 lugares. Estilo bem feito é sobre **definir uma vez, usar em todo lugar** — e ainda ganhar modo escuro de brinde.

## O problema dos valores espalhados

Veja este XAML inocente:
```xml
<Button BackgroundColor="#8A2BE2" TextColor="White" CornerRadius="8" />
<Button BackgroundColor="#8A2BE2" TextColor="White" CornerRadius="8" />
<Button BackgroundColor="#8A2BE2" TextColor="White" CornerRadius="8" />
```

Três botões iguais, com o mesmo roxo `#8A2BE2` copiado três vezes. Agora multiplica por um app inteiro. Quando o design mudar o roxo (e vai mudar), você vai caçar `#8A2BE2` em dezenas de arquivos e rezar pra não esquecer nenhum. Isso é dívida visual.

A solução tem dois níveis: **recursos** (valores nomeados) e **estilos** (conjuntos de propriedades nomeados).

## Nível 1: recursos — dê nome aos valores

Em vez de espalhar `#8A2BE2`, defina ele uma vez com um nome, num `ResourceDictionary`:

```xml
<ContentPage.Resources>
    <ResourceDictionary>
        <Color x:Key="Primaria">#8A2BE2</Color>
        <Color x:Key="Fundo">#0D1117</Color>
        <x:Double x:Key="EspacoPadrao">16</x:Double>
    </ResourceDictionary>
</ContentPage.Resources>
```

E use pelo nome com `{StaticResource}`:
```xml
<Button BackgroundColor="{StaticResource Primaria}" />
<Label Padding="{StaticResource EspacoPadrao}" />
```

Agora o roxo mora num lugar só. Mudou ali, mudou no app inteiro. `x:Key` é o nome que você dá; `{StaticResource Nome}` é como você usa.

## Recursos globais: `App.xaml`

Recursos numa página valem só naquela página. Pra valerem no app **inteiro**, coloque no `App.xaml` — é o dicionário de recursos global:

```xml
<Application.Resources>
    <ResourceDictionary>
        <Color x:Key="Primaria">#8A2BE2</Color>
    </ResourceDictionary>
</Application.Resources>
```

Regra mental: cor da marca, espaçamentos padrão, tamanhos de fonte — tudo isso é global, vai no `App.xaml`. Recurso específico de uma tela fica na tela.

## Nível 2: estilos — agrupe propriedades repetidas

Recurso resolve o valor único. Mas e quando **um conjunto** de propriedades se repete (todo botão primário é roxo + branco + arredondado)? Aí você cria um **`Style`**:

```xml
<Style x:Key="BotaoPrimario" TargetType="Button">
    <Setter Property="BackgroundColor" Value="{StaticResource Primaria}" />
    <Setter Property="TextColor" Value="White" />
    <Setter Property="CornerRadius" Value="8" />
    <Setter Property="HeightRequest" Value="48" />
</Style>
```

E aplica:
```xml
<Button Text="Salvar" Style="{StaticResource BotaoPrimario}" />
```

Um botão, uma linha de estilo, e ele herda as quatro propriedades. Mudou o padrão de botão? Muda o `Style`, muda todos.

## Estilos implícitos: sem precisar nomear

Se você quer que **todos** os controles de um tipo tenham um visual padrão (todo `Label` cinza, por exemplo), crie um `Style` **sem `x:Key`**. Ele se aplica automaticamente a todos daquele tipo:

```xml
<Style TargetType="Label">
    <Setter Property="TextColor" Value="#E6EDF3" />
    <Setter Property="FontFamily" Value="OpenSansRegular" />
</Style>
```

Agora todo `Label` do app já nasce com essa cor e fonte, sem você fazer nada. É o "tema base" da sua UI.

## Modo escuro — quase de graça

Aqui o estilo bem feito te paga de volta. O MAUI tem suporte nativo a tema claro/escuro com `AppThemeBinding`: você define os dois valores, e ele troca conforme o sistema do usuário.

```xml
<Label TextColor="{AppThemeBinding Light=Black, Dark=White}" />

<!-- melhor ainda: combine com recursos -->
<Color x:Key="Texto">{AppThemeBinding Light=#1A1A1A, Dark=#E6EDF3}</Color>
```

Se você centralizou suas cores em recursos (nível 1), adicionar modo escuro vira ajustar **um dicionário** em vez de cada tela. Esse é o retorno de ter feito a lição de casa do estilo: o modo escuro deixa de ser um projeto e vira um detalhe.

## A hierarquia, pra fixar

```
App.xaml (global)  →  vale no app inteiro
   └── Page.Resources  →  vale só na página
          └── propriedade no elemento  →  vale só naquele elemento
```

O mais específico ganha. Um `Button` com `BackgroundColor` próprio ignora o estilo. Use isso a seu favor: defina o padrão global, sobrescreva pontualmente só onde precisa fugir do padrão.

## A regra de ouro

**Nenhum valor visual importante deve aparecer mais de uma vez no código.** Cor de marca, espaçamento padrão, raio de canto, tamanho de fonte — tudo vira recurso nomeado. Não é firula de organização: é o que torna mudar o visual do app uma tarefa de minutos em vez de uma caça ao tesouro.

➡️ Voltou a empacar? Veja [erros comuns e como resolver](./troubleshooting.md) · ou o [glossário](./glossary.md) pra um termo que não entendeu.
