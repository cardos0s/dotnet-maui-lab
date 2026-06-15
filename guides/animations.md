# ✨ Animações & feedback visual — o app que parece vivo

> A diferença entre um app que parece "travado" e um que parece "fluido" raramente é velocidade real — é **feedback**. Um botão que reage ao toque, uma tela que transita suave, um item que aparece deslizando. Animação bem usada é comunicação, não enfeite.

## Por que animar (e por que NÃO exagerar)

Animação tem um propósito funcional antes de estético: ela **explica o que está acontecendo**. Quando um item desliza pra fora ao ser deletado, o usuário entende "foi embora". Quando um botão encolhe ao toque, ele sente "registrei seu clique". Quando uma tela entra pela direita, ele entende "avancei".

O erro do iniciante empolgado é animar tudo, o tempo todo, com 800ms de duração. O resultado é um app que parece **lento**, porque o usuário fica esperando a firula terminar pra fazer a próxima coisa. **Regra de ouro: animações curtas (150–300ms) e com propósito.** Se a animação não comunica nada, corte.

## O básico: animar uma propriedade

Quase todo elemento tem métodos de animação prontos. Os mais usados:

```csharp
await meuBotao.ScaleTo(0.95, 100);   // encolhe pra 95% em 100ms (feedback de toque)
await meuBotao.ScaleTo(1.0, 100);    // volta ao normal

await painel.FadeTo(0, 250);         // some (opacidade 0) em 250ms
await painel.FadeTo(1, 250);         // aparece

await card.TranslateTo(0, -50, 200); // move 50px pra cima em 200ms
await imagem.RotateTo(360, 500);     // gira uma volta
```

Todos são `async` — você dá `await` pra esperar terminar, ou encadeia. Combinar dois é o feijão-com-arroz do "aparecer suave":

```csharp
// elemento entra subindo e aparecendo ao mesmo tempo
card.Opacity = 0;
card.TranslateTo(0, 20, 0);          // começa 20px abaixo
await Task.WhenAll(
    card.FadeTo(1, 250),
    card.TranslateTo(0, 0, 250));    // sobe e aparece juntos
```

`Task.WhenAll` roda as duas ao mesmo tempo — porque animações sequenciais (uma depois da outra) somam duração e ficam lentas.

## Easing — o tempero que faz parecer natural

Movimento no mundo real não é linear: as coisas aceleram e desaceleram. O `Easing` é o que dá essa naturalidade. Comparado a um movimento robótico (linear), um com easing parece "vivo":

```csharp
await card.TranslateTo(0, 0, 300, Easing.CubicOut);   // desacelera no fim — suave
await bola.TranslateTo(0, 100, 400, Easing.BounceOut); // quica no fim
```

`CubicOut` (começa rápido, freia no fim) é o que você vai usar 80% das vezes — é o movimento "natural" que combina com quase tudo.

## Visual States — reagir a estados sem código

Pra coisas como "como o botão fica quando pressionado/desabilitado", o MAUI tem **VisualStateManager** — você declara o visual de cada estado no XAML, e o sistema troca sozinho:

```xml
<Button Text="Salvar">
    <VisualStateManager.VisualStateGroups>
        <VisualStateGroup x:Name="CommonStates">
            <VisualState x:Name="Normal">
                <VisualState.Setters>
                    <Setter Property="Scale" Value="1" />
                </VisualState.Setters>
            </VisualState>
            <VisualState x:Name="Pressed">
                <VisualState.Setters>
                    <Setter Property="Scale" Value="0.96" />
                </VisualState.Setters>
            </VisualState>
        </VisualStateGroup>
    </VisualStateManager.VisualStateGroups>
</Button>
```

Sem uma linha de C#, o botão encolhe ao ser pressionado e volta ao soltar. É declarativo, limpo, e é o jeito certo de fazer feedback de toque consistente no app inteiro (combinado com [estilos](./styling-theming.md)).

## Microfeedback: o detalhe que parece caro

As animações que mais impressionam são as menores e mais sutis:
- **Botão que encolhe ~4% ao toque** — o usuário *sente* o clique.
- **Item de lista que aparece com fade** ao carregar — em vez de "pipocar".
- **Pull-to-refresh** com a rodinha (o `RefreshView` já dá isso de graça).
- **Vibração leve** (`HapticFeedback.Perform(HapticFeedbackType.Click)`) ao completar uma ação importante.

Nenhuma dessas chama atenção pra si — elas só fazem o app *parecer* mais responsivo e caro. É o oposto da animação chamativa: o melhor microfeedback é o que o usuário sente sem perceber.

## Performance: animação também custa

Animação roda na UI thread. Animar muitas coisas ao mesmo tempo, ou animar propriedades caras (mudar layout repetidamente), pode causar engasgo — justo o oposto do que você queria. Prefira animar `Scale`, `Opacity`, `TranslationX/Y` e `Rotation`, que são baratas (a GPU lida bem). Evite animar coisas que forçam recálculo de layout a cada frame. (Conecta com o [guia de performance](./performance.md).)

## O resumo

Animação é comunicação: curta, com propósito, com easing natural. Use **VisualStates** pra feedback de toque consistente, **microfeedback** pra dar a sensação de polimento, e **anime só o barato** (Scale/Opacity/Translation) pra não trocar fluidez por firula. O alvo não é um app que chama atenção pelas animações — é um app que *parece certo* e ninguém sabe explicar por quê.

➡️ Relacionado: [estilo e temas](./styling-theming.md) · [performance](./performance.md).
