# 📚 Guias — .NET MAUI

Boas práticas de UI e performance acumuladas construindo apps MAUI, com exemplos de código prontos para aplicar.

| Guia | Sobre |
|---|---|
| 🏛️ [Arquitetura](./architecture.md) | Clean Architecture, MVVM (CommunityToolkit), DI, navegação desacoplada, camadas |
| 🧪 [Testes](./testing.md) | Pirâmide de testes, testar ViewModels sem emulador, dublês, CI |
| 📐 [Layout](./layout.md) | Árvore visual rasa, `Grid` vs StackLayout aninhado, dimensionamento `Auto`/`*`, `CollectionView` vs `BindableLayout`, `FlexLayout` |
| ⚡ [Performance & Velocidade](./performance.md) | Compiled bindings (`x:DataType`), reciclagem em listas, startup, imagens, UI thread, handlers, checklist e como medir |

> 🔗 Os guias se reforçam: **arquitetura** dá testabilidade, **testes** garantem regressão zero, **layout** reduz custo de render e **performance** cuida do runtime. A implementação completa está no [reference-app](../reference-app).

## TL;DR — top 5 que mais importam
1. **`x:DataType`** em toda página/template → bindings compilados (ganho enorme).
2. **`CollectionView`** com template raso → rolagem fluida.
3. **Árvore visual rasa** → `Grid` no lugar de StackLayouts aninhados.
4. **Imagens no tamanho certo** + ícones vetoriais → memória sob controle.
5. **Nunca bloqueie a UI thread** (`await`, nada de `.Result`).
