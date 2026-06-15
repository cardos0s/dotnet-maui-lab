# 📚 Guias .NET MAUI — uma trilha do zero ao sênior

> Não é uma pilha de documentos soltos. É um **caminho**, pensado pra você seguir em ordem se está começando, ou pular direto no que precisa se já tem estrada. Cada guia é escrito pra explicar o *porquê*, não só o *como*.

## 🟢 Trilha iniciante — "nunca abri o MAUI"

Comece aqui e siga na ordem. No fim, você consegue construir uma tela funcional e entender cada linha.

1. **[Começando do zero](./getting-started.md)** — instalar, configurar e rodar seu primeiro app. O tour dos arquivos.
2. **[XAML do zero](./xaml-basics.md)** — a linguagem das telas. O que é, como funciona, a dupla XAML + code-behind.
3. **[Os controles essenciais](./controls.md)** — o vocabulário: Label, Button, Entry, Image, listas, layouts. Catálogo de consulta.
4. **[Data binding](./data-binding.md)** — ligar a tela aos dados. **O conceito que muda tudo** — leia com calma.
5. **[Navegação](./navigation.md)** — andar entre telas (NavigationPage e Shell).
6. **[Estilo e temas](./styling-theming.md)** — deixar bonito, consistente, com modo escuro.

🆘 Travou? **[Erros comuns](./troubleshooting.md)** tem o seu sintoma. Termo estranho? **[Glossário](./glossary.md)**.

## 🟡 Trilha intermediária — "já faço telas, quero fazer direito"

Você já constrói coisas, mas o código começou a embolar. Hora de estruturar.

- **[Arquitetura](./architecture.md)** — Clean Architecture, MVVM de verdade, DI, navegação desacoplada. O salto de "funciona" pra "escala".
- **[Layout (a fundo)](./layout.md)** — por que sua tela trava e como a profundidade da árvore visual decide isso.

## 🔴 Trilha avançada — "quero nível profissional"

- **[Testes](./testing.md)** — por que apps mobile não têm testes e como sair dessa; testar ViewModels sem emulador.
- **[Performance](./performance.md)** — startup, scroll, memória: onde o tempo realmente vai, e como medir antes de otimizar.

## ⭐ E veja funcionando de verdade

Toda a teoria das trilhas intermediária e avançada está **implementada e testada** no **[reference-app (TaskApp)](../reference-app)** — Clean Architecture, MVVM, DI e testes unitários rodando (6/6 verdes). Leia o código junto com os guias: é a ponte entre entender e fazer.

---

## TL;DR — os 5 hábitos que mais importam
Se você levar só cinco coisas destes guias:
1. **`x:DataType`** em toda página e template → bindings rápidos e erros no build.
2. **Lógica na ViewModel, não no code-behind** → testável e reutilizável.
3. **`CollectionView` com template raso** → listas que rolam lisas.
4. **Árvore visual rasa** (`Grid` > StackLayouts aninhados) → render barata.
5. **`await` sempre, nunca `.Result`** → app que não congela.
