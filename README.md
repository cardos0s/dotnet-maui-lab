<img width="100%" src="https://capsule-render.vercel.app/api?type=waving&color=0:512BD4,100:8A2BE2&height=160&section=header&text=.NET%20MAUI%20Lab&fontSize=46&fontColor=FFFFFF&fontAlignY=35&desc=Arquitetura%20%C2%B7%20Testes%20%C2%B7%20Performance%20%C2%B7%20UI%20cross-platform&descSize=15&descAlignY=58&descColor=FFFFFF"/>

<p align="center">
  Engenharia <b>.NET MAUI</b> de ponta a ponta — uma <b>trilha completa do zero ao nível sênior</b>:<br/>
  do "nunca abri o MAUI" até Clean Architecture, MVVM e testes, com tudo explicado a fundo.
</p>

<p align="center">
  <img src="https://skillicons.dev/icons?i=cs,dotnet&perline=9" />
</p>

---

## 🎓 Está começando? Comece por aqui

Uma **trilha de aprendizado em ordem**, escrita pra explicar o *porquê* de cada coisa — não só o como:

| # | Guia | O que você aprende |
|---|---|---|
| 1 | [Começando do zero](./guides/getting-started.md) | Instalar, configurar e rodar o primeiro app |
| 2 | [XAML do zero](./guides/xaml-basics.md) | A linguagem das telas |
| 3 | [Controles essenciais](./guides/controls.md) | O vocabulário: botões, listas, layouts |
| 4 | [Data binding](./guides/data-binding.md) | Ligar a tela aos dados — o conceito-chave |
| 5 | [Navegação](./guides/navigation.md) | Andar entre telas |
| 6 | [Estilo e temas](./guides/styling-theming.md) | Deixar bonito + modo escuro |

🆘 [Erros comuns](./guides/troubleshooting.md) · 📖 [Glossário](./guides/glossary.md) · 🗺️ [Trilha completa (iniciante → sênior)](./guides/README.md)

## 🔵 Apps de verdade — "minha tela funciona, e agora?"

O que leva um exercício até a loja:

| # | Guia | O que você aprende |
|---|---|---|
| 7 | [Consumindo APIs](./guides/networking.md) | Buscar dados e tratar a rede que falha |
| 8 | [Armazenamento & offline-first](./guides/local-storage.md) | Guardar no aparelho, funcionar sem internet |
| 9 | [Recursos do dispositivo](./guides/device-features.md) | GPS, câmera, sensores, permissões |
| 10 | [Acessibilidade](./guides/accessibility.md) | Um app que todo mundo usa |
| 11 | [Animações & feedback](./guides/animations.md) | O app que parece vivo |
| 12 | [Código por plataforma](./guides/platform-specifics.md) | Quando "um código só" precisa de exceções |
| 13 | [Publicando na loja](./guides/publishing.md) | Release, assinatura, App Store / Play |

---

## ⭐ App de referência — destaque

O **[reference-app (TaskApp)](./reference-app)** mostra como estruturar um app MAUI de verdade:

- 🏛️ **Clean Architecture** — Core (`.NET` puro, testável) separado do head de UI
- 🧩 **MVVM** com CommunityToolkit.Mvvm (source generators)
- 💉 **Dependency Injection** — cadeia Repository → ViewModel → Page → App
- ✅ **Testes unitários** das ViewModels rodando **sem emulador** — `11/11 verdes`, executados de verdade
- ⚡ Compiled bindings, `CollectionView` reciclado, layout raso

> O domínio é simples de propósito (lista de tarefas) — o valor está na **engenharia**, não no feature set. Veja a [ARCHITECTURE.md](./reference-app/ARCHITECTURE.md).

## 📚 Guias avançados — "já faço telas, quero fazer profissional"

| Guia | Sobre |
|---|---|
| 🏛️ [Arquitetura](./guides/architecture.md) | Clean Architecture, MVVM, DI, navegação, camadas |
| 🧪 [Testes](./guides/testing.md) | Pirâmide, testar ViewModels sem emulador, dublês, CI |
| 📐 [Layout](./guides/layout.md) | Árvore visual rasa, `Grid`, `CollectionView`, `FlexLayout` |
| ⚡ [Performance](./guides/performance.md) | Compiled bindings, reciclagem, startup, imagens, UI thread |

## 🎨 Demos de UI

| Demo | Tema | Destaque |
|---|---|---|
| 🍔 [Cardoso's Burguer](./cardosos-burguer) | Detalhes de produto | Layout de detalhe com imagem e avaliação |
| 🎄 [Christmas Catalog](./christmas-catalog) | Catálogo de Natal | Múltiplas telas, cabeçalho fixo + lista rolável |

---

## 🗂️ Estrutura

```
dotnet-maui-lab/
├── reference-app/       # ⭐ App de referência: Clean Arch + MVVM + testes
│   ├── src/TaskApp.Core/      (lib testável, sem MAUI)
│   ├── src/TaskApp.Maui/      (head de UI)
│   └── tests/                 (xUnit — 11/11 verdes)
├── guides/              # Trilha do zero ao sênior (17 guias + glossário)
├── cardosos-burguer/    # Demo de UI
└── christmas-catalog/   # Demo de UI
```

## ▶️ Rodando

```bash
# testes do app de referência — sem workload MAUI nem emulador
cd reference-app && dotnet test          # ✅ 11/11

# qualquer demo / o head MAUI (requer workload MAUI)
dotnet build -t:Run -f net10.0-android
```

Para um projeto MAUI completo em produção (mapas, gravação, MVVM), veja o **[Strava_Maui](https://github.com/cardos0s/Strava_Maui)**.

---

<p align="center">
  <b>Feito por <a href="https://github.com/cardos0s">Julia Cardoso</a></b>
</p>
