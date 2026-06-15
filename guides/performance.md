# ⚡ Performance em .NET MAUI — onde o tempo realmente vai

> Performance não é um truque que você aplica no fim. É um conjunto de decisões que você toma sabendo onde o tempo e a memória de um app mobile *de verdade* escorrem. Vou na ordem do que mais impacta, não na ordem do que é mais fácil de falar.

## Antes de otimizar, entenda os três inimigos

Num app MAUI, a percepção de "lentidão" vem de três lugares, e confundir eles é otimizar o que não importa:

1. **Startup** — o tempo entre tocar no ícone e a primeira tela útil aparecer. Primeira impressão, literalmente.
2. **Fluidez de scroll/interação** — o app responde ou engasga quando você rola e toca.
3. **Memória** — o app incha até o sistema matá-lo, ou trava o aparelho do usuário.

Cada um tem causas diferentes. Vamos por impacto.

## 1. Compiled bindings — o ganho mais barato que existe

Esse é o primeiro que eu aplicaria em qualquer projeto, porque o custo é quase zero e o ganho é enorme.

Por padrão, quando você escreve `{Binding UserName}` no XAML, o MAUI descobre *em tempo de execução*, via **reflection**, qual propriedade é essa e como ler. Reflection é flexível, mas é lenta — e numa lista que rola, isso é feito repetidamente, por item, por propriedade.

A correção é declarar o tipo do contexto com `x:DataType`. Aí o compilador resolve os bindings **em tempo de build**: vira acesso direto a propriedade, sem reflection nenhuma. O ganho documentado chega a ordens de grandeza, e o "custo" é literalmente uma linha por escopo:

```xml
<!-- Declare na página -->
<ContentPage xmlns:vm="clr-namespace:App.ViewModels"
             x:DataType="vm:HomeViewModel">
    <Label Text="{Binding UserName}" />
</ContentPage>

<!-- E DE NOVO dentro de cada DataTemplate — o escopo muda ali -->
<DataTemplate x:DataType="models:Product">
    <Label Text="{Binding Name}" />
</DataTemplate>
```

O detalhe que pega todo mundo: **o `x:DataType` precisa ser declarado de novo dentro de `DataTemplate`**, porque ali o contexto de binding deixa de ser a ViewModel e passa a ser o item da lista. Esqueceu? Aquele binding volta a ser reflection — justo o de dentro da lista, o que mais roda. Por isso vale tratar os avisos `XC` de "binding sem x:DataType" como erro: eles te apontam exatamente os bindings que ainda estão lentos.

## 2. Reciclagem em listas — fluidez não se finge

Já falei no [guia de layout](./layout.md) que `CollectionView` recicla elementos. Aqui vai o complemento de performance: a reciclagem só entrega o que promete se você não a sabotar.

```xml
<CollectionView ItemsSource="{Binding Items}"
                ItemSizingStrategy="MeasureFirstItem">
    <CollectionView.ItemTemplate>
        <DataTemplate x:DataType="models:Item">
            <!-- template RASO. Cada nível aqui é multiplicado por centenas de itens. -->
            <Grid ColumnDefinitions="Auto,*" Padding="8">
                <Image Grid.Column="0" Source="{Binding Icon}" />
                <Label Grid.Column="1" Text="{Binding Title}" />
            </Grid>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

Dois ajustes que importam:

- **`ItemSizingStrategy="MeasureFirstItem"`** — quando todos os itens têm a mesma altura, isso diz ao MAUI "mede um, aplica em todos" em vez de remedir cada um. Numa lista grande de itens uniformes, é um ganho real. (Só não use se os itens têm alturas variáveis — aí ele mediria errado.)
- **Template raso.** Repito porque é o que mais gente erra: o custo do item é multiplicado pela quantidade de itens. Um nível a mais no template é centenas de medições a mais no scroll.

## 3. Startup — a primeira impressão que você controla

Ninguém perdoa um app que demora 4 segundos pra abrir. Três frentes:

- **AOT e trimming no Release.** Em desenvolvimento o app é compilado sob demanda (JIT), o que adiciona tempo no boot. No build de Release, habilite trimming (`<PublishTrimmed>true`) e, no Android, AOT — o código já vem compilado, e o boot encurta de verdade.
- **Não faça trabalho pesado no construtor.** O pecado clássico é carregar dados, abrir banco, chamar API tudo no construtor da `App` ou da primeira página — e o usuário fica olhando pra uma tela em branco. **Mostre a UI primeiro, carregue depois.** No reference-app, o `OnAppearing` dispara o carregamento *depois* que a página apareceu; a tela surge na hora e os dados entram em seguida.
- **DI enxuta.** Cada serviço pesado instanciado no startup é tempo no boot. Registre com consciência; deixe o que é caro pra ser criado sob demanda (Transient/lazy) em vez de tudo Singleton no arranque.

## 4. Imagens — a causa número um de memória estourada

Se o seu app está consumindo memória demais, aposto na imagem antes de qualquer outra coisa.

O erro é jogar um JPG de 4000 pixels num avatar de 48 pixels. O arquivo é pequeno no disco, mas **descomprimido na memória** ele ocupa o tamanho real, em bitmap. Multiplique por uma lista de avatares e você tem centenas de MB de RAM em imagens que aparecem do tamanho de uma moeda.

- Sirva a imagem **no tamanho de exibição**, não no tamanho original.
- Use `Aspect="AspectFill"` com dimensões fixas pra evitar remedição.
- Pra imagens remotas, aproveite o cache (o MAUI cacheia por padrão; ajuste `CachingEnabled`/`CacheValidity`).
- Pra ícones, prefira **SVG ou fonte de ícones** — escalam pra qualquer tamanho sem custo de bitmap nem pixelização.

## 5. Nunca, jamais, bloqueie a UI thread

Existe uma thread, e só uma, que desenha a tela e responde ao toque. Se você a bloquear, o app *congela* — não devagar, congela mesmo, e o Android mostra o temido "App não está respondendo".

```csharp
// ❌ .Result e .Wait() bloqueiam a UI thread. O app trava aqui.
var data = httpClient.GetStringAsync(url).Result;

// ✅ await libera a thread pra desenhar enquanto a rede trabalha.
var data = await httpClient.GetStringAsync(url);
```

Regra sem exceção: **I/O é sempre `await`**. E trabalho pesado de CPU (processar uma imagem, parsear um arquivo gigante) vai pra `Task.Run`, com a atualização da UI voltando pro thread principal via `MainThread.BeginInvokeOnMainThread`. A UI thread existe pra desenhar, não pra esperar.

## 6. Handlers, não Renderers

Detalhe de quem vem do Xamarin: pra customizar um controle no MAUI, o caminho é **Handler**, não o `Renderer` antigo. A arquitetura de handlers é mais leve e desacoplada, e pra ajustes pontuais você nem precisa subclassar — mexe num `mapper`. Se você está portando código com `CustomRenderer`, isso é dívida pra migrar.

## O checklist que eu colaria no PR

- [ ] `x:DataType` em toda página **e** em todo `DataTemplate`
- [ ] `CollectionView` (nunca `ListView`), com template raso
- [ ] Imagens no tamanho de exibição + ícones vetoriais
- [ ] Zero `.Result`/`.Wait()` na UI thread — `await` sempre
- [ ] Nada de trabalho pesado no construtor; carregue no `OnAppearing`
- [ ] Release com trimming/AOT
- [ ] Árvore visual rasa (ver [layout](./layout.md))

## E, por favor, meça antes de otimizar

A frase mais cara em performance é "eu acho que é isso aqui". Quase sempre não é. Antes de reescrever, **meça**:

- **`dotnet-trace` e `dotnet-counters`** pra CPU e alocações de memória.
- **Android Profiler / Instruments (iOS)** pra memória e taxa de frames.
- Pro startup, coloque timestamps no `App` e na primeira página e veja o número subir a cada coisa que você carrega.

Otimização sem medição é superstição. Você gasta um dia "melhorando" o que custava 2ms e ignora o que custava 800ms. Mede, acha o gargalo de verdade, ataca ele, mede de novo. É menos heróico e muito mais eficaz.

➡️ Continua em: [layout, a base de tudo](./layout.md) · [arquitetura](./architecture.md) · [testes](./testing.md)
