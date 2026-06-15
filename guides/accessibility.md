# ♿ Acessibilidade — fazer um app que todo mundo consegue usar

> Acessibilidade não é um recurso extra pra "quando sobrar tempo". É a diferença entre um app que serve a todos e um que exclui milhões de pessoas — pessoas cegas, com baixa visão, com dificuldade motora, daltônicas. E a boa notícia: 80% do trabalho são pequenos cuidados que cabem na sua rotina normal.

## Por que isso importa (de verdade, não da boca pra fora)

Cerca de 1 em cada 6 pessoas no mundo vive com alguma deficiência. Quando seu app não é acessível, você não está deixando de "atender um nicho" — está fechando a porta na cara de uma fração enorme dos seus usuários. Além disso, em muitos lugares é **exigência legal**, e as lojas (Apple, Google) cada vez olham mais pra isso. Mas o argumento que importa é o primeiro: software bom é software que as pessoas conseguem usar.

E tem um bônus honesto: quase tudo que melhora acessibilidade melhora a experiência pra **todo mundo**. Contraste bom ajuda quem está no sol. Alvos de toque grandes ajudam quem está no ônibus tremendo. Legendas ajudam quem está no silêncio.

## 1. Leitores de tela — o `SemanticProperties`

A pessoa cega navega o app por um **leitor de tela** (TalkBack no Android, VoiceOver no iOS), que **lê em voz alta** o que está na tela conforme ela toca. Pra isso funcionar, seus elementos precisam ter descrições com sentido.

```xml
<!-- ❌ Um ícone de lixeira sem descrição: o leitor fala "botão". Botão de quê?! -->
<ImageButton Source="lixeira.png" />

<!-- ✅ Agora o leitor fala "Excluir tarefa" -->
<ImageButton Source="lixeira.png"
             SemanticProperties.Description="Excluir tarefa" />
```

- **`SemanticProperties.Description`** — o que o elemento *é/faz*. Essencial em ícones, imagens e botões sem texto.
- **`SemanticProperties.Hint`** — uma dica extra do que vai acontecer ("toque duplo para abrir").
- **`SemanticProperties.HeadingLevel`** — marca títulos, pra pessoa navegar "de título em título" como faria numa página web.

A regra prática: **todo elemento interativo sem texto visível precisa de uma `Description`.** Um botão escrito "Salvar" já se descreve sozinho; um ícone de engrenagem, não.

## 2. Contraste — a leitura que não cansa

Texto cinza-claro sobre fundo branco é bonito no Figma e ilegível pra quem tem baixa visão (e pra você no sol). O padrão **WCAG** define uma régua: texto normal precisa de contraste de pelo menos **4.5:1** com o fundo.

Não precisa decorar a matemática — use um verificador de contraste (tem vários online, e o próprio sistema de design tokens pode validar). A regra de bolso: se você precisa apertar os olhos pra ler, está ruim. Centralize suas cores em [recursos](./styling-theming.md) e teste o contraste delas uma vez; depois é só reusar.

## 3. Não comunique só por cor

Erro clássico: "campos em vermelho estão errados". E quem é daltônico? Pra ~8% dos homens, vermelho e verde são a mesma coisa. **Cor nunca deve ser a única forma de passar uma informação.** Acompanhe sempre de um ícone, um texto ou um símbolo:

```
❌ só a borda vermelha          ✅ borda vermelha + ícone ⚠ + texto "E-mail inválido"
```

## 4. Alvos de toque grandes o suficiente

Um botão de 20x20 pixels é um inferno pra quem tem tremor, artrite ou dedos grandes. A recomendação é **no mínimo 44x44 pontos** de área tocável. Se o ícone é pequeno, aumente a área clicável com `Padding` em volta — a área de toque cresce mesmo o desenho ficando pequeno.

## 5. Tamanho de fonte que respeita o usuário

Muita gente aumenta a fonte do sistema porque precisa. Se você fixa tudo em pixels rígidos, quebra o layout dessas pessoas. Prefira deixar o texto escalar com a preferência do sistema, e teste seu app com a fonte do celular no tamanho grande — é revelador quanta coisa quebra.

## Como testar (e isto é o pulo do gato)

Ler sobre acessibilidade não substitui **sentir** a barreira. Faça isto pelo menos uma vez:

1. **Ligue o leitor de tela** (TalkBack/VoiceOver) e tente usar seu app **de olhos fechados**, só ouvindo. Você vai descobrir, em 30 segundos, todos os botões que falam "botão, botão, botão" sem dizer o quê.
2. **Aumente a fonte do sistema** ao máximo e veja o que quebra.
3. **Use um simulador de daltonismo** e veja se suas telas ainda fazem sentido.

Esses três testes pegam a esmagadora maioria dos problemas, e nenhum custa dinheiro.

## Um checklist honesto

- [ ] Todo ícone/imagem interativo tem `SemanticProperties.Description`
- [ ] Contraste de texto ≥ 4.5:1
- [ ] Nenhuma informação passada *só* por cor
- [ ] Alvos de toque ≥ 44x44
- [ ] Testei navegando com leitor de tela de olhos fechados
- [ ] Testei com a fonte do sistema no grande

## O resumo

Acessibilidade é, na prática, um punhado de hábitos: descrever ícones, garantir contraste, não confiar só na cor, alvos grandes, e **testar com os olhos fechados pelo menos uma vez**. Você não precisa virar especialista — precisa parar de assumir que todo usuário enxerga, ouve e toca como você. Esse é, aliás, o espírito de projetos como o [Aurora](https://github.com/cardos0s): tecnologia que inclui em vez de excluir.

➡️ Relacionado: [estilo e temas](./styling-theming.md) (onde centralizar cores e contraste) · [controles](./controls.md).
