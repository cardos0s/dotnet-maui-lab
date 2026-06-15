# 🐞 Debugging — achar o bug sem perder a cabeça

> Programar é, em boa parte, consertar coisas que não funcionam. A diferença entre quem trava horas num bug e quem resolve em minutos quase nunca é inteligência — é **método e ferramenta**. Este guia é sobre os dois.

## A mentalidade primeiro: o bug não é pessoal

Quando algo não funciona, o instinto é mudar coisas no chute até "consertar". Isso é o caminho mais longo. Debugging bom é **investigação**, não tentativa e erro: você forma uma hipótese ("acho que a lista está vazia porque o load não rodou"), faz UM teste pra confirmar, e ajusta a hipótese com o que descobriu. Uma variável por vez. É chato e é rápido — o oposto de mudar cinco coisas e não saber qual resolveu.

## Hot Reload — seu melhor amigo no dia a dia

Antes de falar de bug, a ferramenta que mais economiza tempo: o **XAML Hot Reload**. Com o app rodando, você muda o XAML (uma cor, um espaçamento, um texto) e **a tela atualiza na hora**, sem recompilar nem perder o estado. Ajustar layout vira instantâneo em vez de "muda → build de 40s → navega de novo até a tela → olha".

Tem também o **.NET Hot Reload** pra mudanças em C# (mais limitado — mudanças estruturais ainda pedem rebuild, mas ajustes de método pegam ao vivo). Use os dois sem dó: é a diferença entre iterar 2 vezes por minuto e 2 vezes por hora.

## Breakpoints — congelar o tempo

O breakpoint é a ferramenta mais poderosa e a mais subutilizada por iniciante (que prefere encher o código de prints). Você clica na margem de uma linha, roda em debug, e quando a execução chega ali ela **para** — e você pode inspecionar **tudo**: o valor de cada variável naquele instante.

A partir do breakpoint, os controles que importam:
- **Step Over (F10)** — executa a linha atual e para na próxima.
- **Step Into (F11)** — entra dentro do método que está sendo chamado, pra ver o que acontece lá dentro.
- **Step Out (Shift+F11)** — termina o método atual e volta pra quem chamou.
- **Continue (F5)** — solta a execução até o próximo breakpoint.

O fluxo típico: você suspeita de um método, põe um breakpoint no começo dele, e vai de F10 em F10 vendo os valores mudarem — até a linha onde o valor fica *errado*. Achou a linha, achou o bug. Isso resolve a esmagadora maioria dos "por que esse número está zerado?".

### Breakpoints que pensam por você
- **Breakpoint condicional** — só para *quando uma condição é verdadeira* (`id == 42`). Salva sua vida num loop de 1.000 itens onde só o item 42 quebra. Clica com o botão direito no breakpoint → condição.
- **Janela de Watch** — fixa uma expressão (`tasks.Count`) pra acompanhar o valor dela a cada passo, sem ficar procurando.

## A janela de saída e os logs

Quando o breakpoint não cabe (um erro que só acontece raramente, ou em produção), você cai nos **logs**. O básico:

```csharp
System.Diagnostics.Debug.WriteLine($"Carregou {tasks.Count} tarefas");
```

Isso aparece na **janela de Output** do seu editor enquanto o app roda em debug. Diferente do `Console.WriteLine`, o `Debug.WriteLine` some no build de Release — então você não vaza log de debug pro app publicado.

Pra algo mais sério, configure **logging estruturado** (o `ILogger` da própria DI), que você pode ligar/desligar por nível e direcionar pra onde quiser.

## Lendo os logs nativos (quando o app fecha sozinho)

Tem um tipo de bug que assusta: o app **fecha sem mensagem** (crash nativo). O erro não aparece no seu editor porque aconteceu lá embaixo, na camada Android/iOS. A ferramenta pra isso:

- **Android:** `adb logcat` no terminal mostra o log do sistema em tempo real. Filtre pelo nome do seu app e procure por `FATAL EXCEPTION` — o stack trace nativo está ali.
- **iOS:** os logs aparecem no Console do Xcode / dispositivo.

Não precisa decorar — precisa saber que **todo crash deixa rastro no log nativo**, mesmo os que somem do seu editor. Quando o app "fecha do nada", é pra lá que você vai.

## A arte de isolar o problema

Quando um bug é cabeludo e você não sabe nem por onde começar, a técnica é **dividir pela metade**. O dado está errado na tela. Ele vem de uma API, passa por um serviço, vira ViewModel, vira binding. Em vez de olhar tudo, você corta no meio: põe um breakpoint **no serviço** e olha o dado ali. Certo no serviço? O bug está depois (ViewModel/binding). Errado no serviço? O bug está antes (API/parsing). Você acabou de eliminar metade do código suspeito com um teste. Repete, corta no meio de novo, e em poucos passos o bug não tem mais onde se esconder.

## O atalho que a arquitetura te dá

Lembra do [guia de arquitetura](./architecture.md)? Aqui ele paga de novo. Quando sua lógica está numa ViewModel testável, você nem precisa do app pra caçar o bug — você escreve um **teste** que reproduz o problema, e debuga *ele*, que roda em milissegundos e você repete à vontade. Um bug que você consegue capturar num teste é um bug que não volta, porque o teste fica lá vigiando pra sempre (ver [testes](./testing.md)).

## O resumo

Debugging é método + ferramenta. **Método:** uma hipótese por vez, divida pela metade, não chute. **Ferramentas:** Hot Reload pra iterar rápido, breakpoints (com condição e watch) pra congelar e inspecionar, `Debug.WriteLine`/logs pro que o breakpoint não pega, e o **logcat** pros crashes nativos que somem do editor. E, sempre que der, transforme o bug num teste — assim você conserta uma vez e ele nunca mais volta.

➡️ Relacionado: [erros comuns](./troubleshooting.md) (os sintomas mais frequentes) · [testes](./testing.md).
