# 📐 Layout em .NET MAUI — por que sua tela está lenta (e a culpa é da árvore)

> Tem uma coisa que quase ninguém liga no começo e que decide se sua tela vai rolar lisa ou travada: a **profundidade da árvore visual**. Este guia é sobre entender isso de verdade, não decorar regras.

## O que acontece quando uma tela aparece

Pra desenhar qualquer tela, o MAUI faz dois passos em cada elemento: **medir** (quanto espaço você quer?) e **arranjar** (toma, esse é o seu espaço). Isso percorre a árvore inteira de elementos, de cima a baixo. Um `Label` dentro de um `StackLayout` dentro de outro `StackLayout` dentro de um `Grid` não é "um label" — é uma cadeia de medições aninhadas, e cada nível pode disparar uma remedição do nível de baixo.

Agora multiplica isso por uma lista com 200 itens, cada item com 6 níveis de aninhamento. São milhares de operações de medida a cada scroll. É exatamente aí que o app começa a "engasgar" ao rolar — não porque o celular é fraco, mas porque você pediu pra ele recalcular uma árvore profunda 60 vezes por segundo.

A regra de ouro do layout sai daí, e é uma só: **árvore visual rasa = renderização barata.** Tudo abaixo é consequência disso.

## `Grid` é seu melhor amigo (e o `StackLayout` aninhado, seu inimigo)

O reflexo de quem está aprendendo é empilhar `StackLayout`s. Quer um ícone à esquerda e dois textos à direita? Um `HorizontalStackLayout` com a imagem e um `VerticalStackLayout` dentro com os labels. Funciona, e cria quatro níveis de aninhamento pra um layout que é, na verdade, uma gradezinha 2x2.

```xml
<!-- ❌ Quatro níveis de profundidade pra um card simples -->
<VerticalStackLayout>
  <HorizontalStackLayout>
    <Image ... />
    <VerticalStackLayout>
      <Label Text="Título" />
      <Label Text="Subtítulo" />
    </VerticalStackLayout>
  </HorizontalStackLayout>
</VerticalStackLayout>

<!-- ✅ Um Grid. Um nível. Mesma aparência, fração do custo. -->
<Grid ColumnDefinitions="Auto,*" RowDefinitions="Auto,Auto" ColumnSpacing="12">
  <Image Grid.RowSpan="2" ... />
  <Label Grid.Column="1" Grid.Row="0" Text="Título" />
  <Label Grid.Column="1" Grid.Row="1" Text="Subtítulo" />
</Grid>
```

O `Grid` resolve em **um nível** o que os `StackLayout`s resolvem em quatro. Posicionamento por linha e coluna, span quando precisa atravessar. Sempre que você se pegar aninhando layouts, pare e pergunte: "isso não é uma grade?" Quase sempre é.

`StackLayout` ainda tem lugar: quando os elementos são genuinamente uma pilha linear e curta (um formulário de três campos, por exemplo). O problema nunca foi o `StackLayout` em si — é o *aninhamento* dele.

## Entender `Auto`, `*` e fixo é entender o custo

As três formas de dimensionar uma linha/coluna do `Grid` não são intercambiáveis — cada uma tem um preço:

- **Fixo** (`100`) — o mais barato. O MAUI não precisa perguntar nada a ninguém: o tamanho é aquele.
- **`*` (estrela)** — barato. "Divida o espaço que sobrou proporcionalmente." É um cálculo simples sobre o espaço restante.
- **`Auto`** — o caro. "Meça o conteúdo e me diga de quanto ele precisa." Isso força uma medição do filho, e numa lista grande isso vira o gargalo.

A lição prática: em listas, **evite `Auto`**. Se todos os itens têm a mesma altura, use tamanho fixo ou `*`. Reserve `Auto` pra onde o conteúdo realmente é imprevisível e raro.

## A lista certa pro trabalho certo

Esse é o erro que mais custa caro em performance percebida. MAUI te dá mais de uma forma de mostrar uma coleção, e usar a errada destrói a fluidez:

- **Lista grande, rolável, com dados que mudam → `CollectionView`.** Ponto final. Ele recicla os elementos visuais ao rolar (reaproveita os que saíram de tela em vez de criar novos), que é o que torna o scroll suave com milhares de itens.
- **Poucos itens estáticos (um menu de 5 opções) → `BindableLayout`** num StackLayout. Pra coisas pequenas e fixas, ele é mais leve que montar um CollectionView.
- **`ListView`? Nunca, em código novo.** É o controle legado do Xamarin. O `CollectionView` é mais rápido, mais flexível e é pra onde a plataforma aponta. Se você vê `ListView` num projeto, é dívida técnica esperando.

E quando usar `CollectionView`, lembre: **o template do item tem que ser raso.** De nada adianta o CollectionView reciclar elementos se cada item é uma árvore de 8 níveis. Card de lista = um `Grid` enxuto. Toda a teoria de "árvore rasa" vale em dobro dentro de um item de lista, porque ali ela é multiplicada por centenas.

## Margin, Padding e o espaçamento que você define no lugar errado

Confusão clássica que gera XAML bagunçado:
- **Padding** é espaço *interno* — entre a borda do container e o que tem dentro.
- **Margin** é espaço *externo* — entre o elemento e os vizinhos.

O anti-padrão é colocar `Margin` em cada filho pra criar espaçamento entre eles. Funciona, mas espalha o mesmo número por dez lugares — e mudar o espaçamento vira caça ao tesouro. Prefira definir o espaçamento **no container**: `Spacing` no StackLayout, `RowSpacing`/`ColumnSpacing` no Grid. Um lugar, uma fonte de verdade, e o layout respira igual sem repetição.

## Quando o layout é fluido: `FlexLayout`

Pra UIs que precisam se adaptar — cards que quebram pra próxima linha quando não cabem, distribuição que muda com o tamanho da tela — o `FlexLayout` faz o trabalho que você faria na mão (calcular quantos cabem por linha) automaticamente. É o `flexbox` do mundo web trazido pro MAUI. Use quando a quantidade de itens por linha depende do espaço disponível.

## O resumo honesto

Layout em MAUI se resume a uma disciplina: **mantenha a árvore rasa e escolha o container pela intenção, não pelo hábito.** `Grid` pra estrutura, `CollectionView` pra listas, espaçamento no container, `Auto` com parcimônia. Faça isso e suas telas rolam lisas mesmo nos aparelhos baratos — que é, no fim, onde a maioria dos seus usuários está.

➡️ Continua em: [performance além do layout](./performance.md) · [arquitetura](./architecture.md) · [testes](./testing.md)
