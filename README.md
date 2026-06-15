<img width="100%" src="https://capsule-render.vercel.app/api?type=waving&color=0:512BD4,100:8A2BE2&height=160&section=header&text=.NET%20MAUI%20Lab&fontSize=46&fontColor=FFFFFF&fontAlignY=35&desc=Demos%20de%20UI%20e%20layout%20cross-platform&descSize=16&descAlignY=58&descColor=FFFFFF"/>

<p align="center">
  Coleção de demos em <b>.NET MAUI</b> focadas em <b>UI, layout e design cross-platform</b>.<br/>
  Cada projeto explora a construção de interfaces ricas com controles nativos (Android · iOS · Windows · macOS).
</p>

<p align="center">
  <img src="https://skillicons.dev/icons?i=cs,dotnet&perline=9" />
</p>

---

## 🎨 Projetos

| Demo | Tema | Destaque de UI |
|---|---|---|
| 🍔 [Cardoso's Burguer](./cardosos-burguer) | Página de detalhes de hambúrguer | Layout de detalhe com imagem, classificação e informações do produto |
| 🎄 [Christmas Catalog](./christmas-catalog) | Catálogo de presentes de Natal | Múltiplas telas (catálogo, opções, detalhes) com cabeçalho fixo + lista rolável |

## 📚 Guias

Boas práticas extraídas da construção dos apps — com snippets prontos:

| Guia | Sobre |
|---|---|
| 📐 [Layout](./guides/layout.md) | Árvore visual rasa, `Grid` vs StackLayout aninhado, `CollectionView`, `FlexLayout` |
| ⚡ [Performance & Velocidade](./guides/performance.md) | Compiled bindings, reciclagem em listas, startup, imagens, UI thread, checklist |

---

## 💡 Sobre

Estas são demos de **craft de interface** — o foco é demonstrar a criação de layouts complexos e temáticos em .NET MAUI usando XAML e controles nativos, com a mesma base de código rodando em todas as plataformas.

Para um projeto MAUI completo de produção (Clean Architecture, MVVM, mapas e fluxo de gravação), veja o **[Strava_Maui](https://github.com/cardos0s/Strava_Maui)**.

```
dotnet-maui-lab/
├── cardosos-burguer/    # Demo: página de detalhes de produto
├── christmas-catalog/   # Demo: catálogo temático multi-tela
└── guides/              # Boas práticas de layout e performance
```

## ▶️ Rodando qualquer demo
```bash
cd <pasta-da-demo>
dotnet build
dotnet build -t:Run -f net10.0-android   # ou -f net10.0-ios / -windows
```
> Requer o workload do .NET MAUI instalado (`dotnet workload install maui`).

---

<p align="center">
  <b>Feito por <a href="https://github.com/cardos0s">Julia Cardoso</a></b>
</p>
