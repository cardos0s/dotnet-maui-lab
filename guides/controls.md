# 🎛️ Os controles essenciais — o vocabulário das telas

> Aprender MAUI é, em boa parte, aprender quais "peças" existem e pra que serve cada uma. Este é seu catálogo de consulta: os controles que você vai usar em 95% das telas, com exemplo e quando usar. Não precisa decorar — volta aqui quando precisar.

## Mostrar texto e imagens

### `Label` — texto na tela
```xml
<Label Text="Olá!" FontSize="20" TextColor="Gray" FontAttributes="Bold" />
```
O mais básico de todos. `FontAttributes` aceita `Bold`, `Italic`, `None`. Pra texto com partes de formatação diferente (uma palavra em negrito no meio da frase), existe `FormattedText`, mas deixe isso pra depois.

### `Image` — imagens
```xml
<Image Source="logo.png" HeightRequest="80" Aspect="AspectFit" />
```
`Source` aponta pra um arquivo em `Resources/Images/`. O `Aspect` controla como a imagem preenche o espaço:
- `AspectFit` — cabe inteira, pode sobrar borda.
- `AspectFill` — preenche tudo, pode cortar.
- `Fill` — estica (geralmente feio, evite).

> 💡 Use sempre `HeightRequest`/`WidthRequest` em imagens. Imagem sem tamanho definido é a causa nº 1 de memória estourada (ver [performance](./performance.md)).

## Receber input do usuário

### `Button` — botão
```xml
<Button Text="Salvar" Clicked="OnSalvar" BackgroundColor="Purple" TextColor="White" />
```

### `Entry` — campo de texto de uma linha
```xml
<Entry Placeholder="Seu nome" Text="{Binding Nome}" Keyboard="Default" />
```
`Placeholder` é o texto cinza que some quando você digita. `Keyboard` muda o teclado: `Email`, `Numeric`, `Telephone`. Pra senha, use `IsPassword="True"`.

### `Editor` — campo de texto de várias linhas
```xml
<Editor Placeholder="Escreva um comentário..." HeightRequest="100" />
```
É o `Entry` para textos longos (caixa que cresce).

### `CheckBox` e `Switch` — ligado/desligado
```xml
<CheckBox IsChecked="{Binding Aceito}" />
<Switch IsToggled="{Binding NotificacoesAtivas}" />
```
`CheckBox` é o quadradinho com "✓"; `Switch` é o botão deslizante. Mesma função, visual diferente — escolha pelo contexto (Switch para configurações, CheckBox para listas/termos).

### `Slider` — escolher um número arrastando
```xml
<Slider Minimum="0" Maximum="100" Value="{Binding Volume}" />
```

### `DatePicker` e `Picker` — escolher data ou item de lista
```xml
<DatePicker Date="{Binding DataNascimento}" />

<Picker Title="Escolha um estado" ItemsSource="{Binding Estados}" />
```
O `Picker` é o "dropdown" — uma lista de opções pra escolher uma.

## Organizar a tela (layouts)

Os layouts não aparecem na tela — eles **posicionam** os outros elementos. Os três que importam:

### `Grid` — linhas e colunas (o mais poderoso)
```xml
<Grid RowDefinitions="Auto,*" ColumnDefinitions="*,*" RowSpacing="8">
    <Label Grid.Row="0" Grid.Column="0" Text="A" />
    <Label Grid.Row="0" Grid.Column="1" Text="B" />
    <Button Grid.Row="1" Grid.ColumnSpan="2" Text="Ocupa as duas colunas" />
</Grid>
```
`Auto` = do tamanho do conteúdo. `*` = divide o espaço que sobra. É o layout que você vai usar mais — entenda ele bem no [guia de layout](./layout.md).

### `VerticalStackLayout` / `HorizontalStackLayout` — empilhar
```xml
<VerticalStackLayout Spacing="10" Padding="20">
    <Label Text="Um" />
    <Label Text="Dois" />
</VerticalStackLayout>
```
Empilha na vertical ou horizontal. Simples e ótimo pra poucos itens. **Cuidado com aninhar muitos** — vira lentidão (de novo, [layout](./layout.md)).

### `ScrollView` — fazer o conteúdo rolar
```xml
<ScrollView>
    <VerticalStackLayout>
        <!-- conteúdo grande que não cabe na tela -->
    </VerticalStackLayout>
</ScrollView>
```
Envolve o conteúdo que pode passar do tamanho da tela. **Nunca** coloque uma lista (`CollectionView`) dentro de um `ScrollView` — elas brigam pela rolagem.

## Listas

### `CollectionView` — listas de dados
```xml
<CollectionView ItemsSource="{Binding Produtos}">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Label Text="{Binding Nome}" Padding="10" />
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```
O controle pra mostrar coleções. `ItemsSource` é a lista de dados; o `ItemTemplate` é o "molde" de como cada item aparece. É um conceito que junta listas + [data binding](./data-binding.md) — o próximo grande passo.

## Visual e feedback

### `Border` — moldura, cantos arredondados, "card"
```xml
<Border Stroke="Gray" StrokeThickness="1" Padding="16"
        StrokeShape="RoundRectangle 12">
    <Label Text="Conteúdo dentro de um card" />
</Border>
```
O `Border` é como você faz "cards". `StrokeShape="RoundRectangle 12"` arredonda os cantos. (Você pode ver `Frame` em código antigo — é o predecessor; prefira `Border`.)

### `ActivityIndicator` — "carregando..."
```xml
<ActivityIndicator IsRunning="{Binding IsBusy}" />
```
A rodinha de loading. `IsRunning="True"` mostra e gira; `False` esconde.

## Propriedades que TODO controle tem

Independente do controle, estas você usa o tempo todo:

| Propriedade | Pra quê |
|---|---|
| `Margin` | Espaço por **fora** do elemento |
| `Padding` | Espaço por **dentro** (em containers) |
| `HorizontalOptions` / `VerticalOptions` | Alinhamento: `Start`, `Center`, `End`, `Fill` |
| `IsVisible` | `True`/`False` — mostra ou esconde |
| `IsEnabled` | `True`/`False` — habilita ou desabilita |
| `BackgroundColor` | Cor de fundo |
| `WidthRequest` / `HeightRequest` | Tamanho sugerido |

## Como descobrir o resto

Este catálogo cobre o essencial, mas existem mais controles. Quando precisar de algo específico ("como faço uma aba?", "como faço um mapa?"), a [documentação oficial de controles](https://learn.microsoft.com/dotnet/maui/user-interface/controls/) tem a lista completa. O segredo é: você não precisa saber todos — precisa saber que eles existem e onde procurar.

➡️ Próximo: [04 · Data binding](./data-binding.md) — como ligar a tela aos seus dados (o conceito que muda tudo).
