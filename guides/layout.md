# 📐 Guia de Layout — .NET MAUI

Boas práticas para construir UIs limpas, flexíveis e baratas de renderizar. A regra de ouro: **árvore visual rasa = renderização rápida.**

## 1. Prefira `Grid` a `StackLayout` aninhado

Cada layout aninhado é mais um passo de medição/arranjo. Um `Grid` único resolve o que vários `StackLayout` aninhados fariam — com menos profundidade.

```xml
<!-- ❌ Evite: aninhamento profundo -->
<VerticalStackLayout>
  <HorizontalStackLayout>
    <Image ... />
    <VerticalStackLayout>
      <Label Text="Título" />
      <Label Text="Subtítulo" />
    </VerticalStackLayout>
  </HorizontalStackLayout>
</VerticalStackLayout>

<!-- ✅ Prefira: um Grid resolve tudo -->
<Grid ColumnDefinitions="Auto,*" RowDefinitions="Auto,Auto" ColumnSpacing="12">
  <Image Grid.RowSpan="2" ... />
  <Label Grid.Column="1" Grid.Row="0" Text="Título" />
  <Label Grid.Column="1" Grid.Row="1" Text="Subtítulo" />
</Grid>
```

## 2. Entenda o dimensionamento: `Auto`, `*` e fixo

- **`Auto`** — mede o conteúdo (custa um passo de medição). Use quando o tamanho depende do filho.
- **`*`** (estrela) — divide o espaço restante proporcionalmente. Mais barato que `Auto`.
- **Fixo** (ex.: `100`) — o mais barato, sem medição.

Evite `Auto` em listas grandes; prefira tamanhos fixos ou `*`.

## 3. Cuidado com `HorizontalOptions/VerticalOptions = "...AndExpand"`

`AndExpand` força recálculo de espaço extra. Na maioria dos casos um `Grid` com `*` expressa a mesma intenção sem o custo. No MAUI moderno, prefira definir o tamanho via `Grid`/`FlexLayout`.

## 4. Liste com o controle certo

| Cenário | Use |
|---|---|
| Lista grande / rolável / dados dinâmicos | **`CollectionView`** |
| Poucos itens estáticos (ex.: menu de 5 opções) | **`BindableLayout`** num StackLayout |
| Grade de itens | `CollectionView` com `GridItemsLayout` |

> Nunca use `ListView` em código novo — o `CollectionView` é mais rápido e flexível.

## 5. Não use um Layout quando uma View basta

Se há um único filho, não embrulhe num `StackLayout`. Um `ContentView` ou a própria View já resolve — menos um nó na árvore.

## 6. `Margin` vs `Padding`

- **Padding** — espaço **interno** (entre a borda do container e o conteúdo).
- **Margin** — espaço **externo** (entre a View e os vizinhos).

Defina espaçamento no container (`Spacing`, `RowSpacing`, `ColumnSpacing`) em vez de Margin em cada filho — mais limpo e barato.

## 7. `FlexLayout` para UIs fluidas

Para wrap automático e distribuição responsiva (cards que quebram linha), `FlexLayout` evita cálculos manuais de quantos cabem por linha.

---

➡️ Veja também o [guia de performance](./performance.md) para o que mais impacta velocidade em runtime.
