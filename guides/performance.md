# ⚡ Guia de Performance & Velocidade — .NET MAUI

O que mais move o ponteiro em apps MAUI: **startup**, **rolagem fluida** e **uso de memória**. Em ordem de impacto.

## 1. Compiled Bindings — o maior ganho fácil

Bindings padrão usam reflection em runtime (lento). Declarar `x:DataType` ativa bindings **compilados**: resolvidos em build, sem reflection. Ganho de até ~8–20x no binding.

```xml
<!-- ✅ Sempre declare o tipo do binding -->
<ContentPage xmlns:vm="clr-namespace:App.ViewModels"
             x:DataType="vm:HomeViewModel">
  <Label Text="{Binding UserName}" />
</ContentPage>

<!-- dentro de DataTemplate, declare também -->
<DataTemplate x:DataType="models:Product">
  <Label Text="{Binding Name}" />
</DataTemplate>
```

> Dica: ative `<MauiEnableXamlCBindingWithSourceCompilation>true` e trate `XC` warnings de binding sem `x:DataType`.

## 2. CollectionView com reciclagem

Para listas, use `CollectionView` com a estratégia de reciclagem (padrão), que reaproveita os elementos visuais ao rolar em vez de recriá-los.

```xml
<CollectionView ItemsSource="{Binding Items}"
                ItemSizingStrategy="MeasureFirstItem">  <!-- mede 1x, aplica a todos -->
  <CollectionView.ItemTemplate>
    <DataTemplate x:DataType="models:Item">
      <!-- mantenha o template RASO e leve -->
      <Grid ColumnDefinitions="Auto,*" Padding="8">
        <Image Grid.Column="0" Source="{Binding Icon}" />
        <Label Grid.Column="1" Text="{Binding Title}" />
      </Grid>
    </DataTemplate>
  </CollectionView.ItemTemplate>
</CollectionView>
```

- `ItemSizingStrategy="MeasureFirstItem"` quando todos os itens têm a mesma altura — evita remedir cada um.
- Mantenha o `DataTemplate` simples: menos views por item = rolagem mais fluida.

## 3. Startup mais rápido

- **AOT / trimming** em Release: `<PublishTrimmed>true` e, no Android, habilite AOT — reduz tempo de JIT no boot.
- **Adie trabalho**: não carregue tudo no construtor da `App`/primeira página. Use lazy loading e carregue dados após a UI aparecer (`OnAppearing` + async).
- **DI enxuto**: registre serviços como `Singleton`/`Transient` conscientemente; evite instanciar serviços pesados no startup.
- **Shell**: use `Shell` para navegação — rotas registradas e páginas criadas sob demanda.

## 4. Imagens — causa nº 1 de memória alta

- Sirva imagens no tamanho de exibição (não jogue um JPG 4000px num avatar 48px).
- Use `Aspect="AspectFill"` + dimensões fixas para evitar remensuração.
- Para imagens remotas, use cache (o MAUI cacheia por padrão; controle com `CachingEnabled`/`CacheValidity`).
- Prefira **SVG/fonte de ícones** para ícones — escalam sem custo de bitmap.

## 5. Não bloqueie a UI thread

```csharp
// ❌ trava a interface
var data = httpClient.GetStringAsync(url).Result;

// ✅ assíncrono — UI continua responsiva
var data = await httpClient.GetStringAsync(url);
```

Trabalho pesado de CPU → `Task.Run`; atualize a UI de volta no thread principal (`MainThread.BeginInvokeOnMainThread`).

## 6. Handlers, não Renderers

No MAUI, customize controles via **Handlers** (arquitetura nova, mais leve) em vez dos Renderers do Xamarin. Use `mappers` para ajustes pontuais sem subclassar.

## 7. Checklist rápido

- [ ] `x:DataType` em todas as páginas e `DataTemplate`
- [ ] `CollectionView` (nunca `ListView`) com template raso
- [ ] Imagens no tamanho certo + ícones vetoriais
- [ ] Nada de `.Result`/`.Wait()` na UI thread
- [ ] Trabalho pesado fora do construtor da página
- [ ] Release com trimming/AOT habilitados
- [ ] Árvore visual rasa (ver [guia de layout](./layout.md))

---

## Como medir
- **Profiler do .NET** (`dotnet-trace`, `dotnet-counters`) para CPU/alocações.
- **Android Profiler / Instruments (iOS)** para memória e frames.
- Meça o **startup** com logs de timestamp no `App` e na primeira página — otimize com número, não com achismo.
