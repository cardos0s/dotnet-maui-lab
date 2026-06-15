# 🧪 Testes em .NET MAUI — testar o que dá medo, onde é barato

> A pergunta que importa não é "como escrever um teste". É "**por que a maioria dos apps mobile não tem testes**" — e como sair dessa armadilha. Os testes reais que sustentam este argumento estão em [reference-app/tests](../reference-app/tests).

## Por que apps mobile quase nunca têm testes decentes

Não é preguiça. É física. O instinto de quem está começando é testar a tela: abrir o app, clicar no botão, ver se a coisa certa aparece. Esse é o teste de UI ponta-a-ponta, e ele tem três problemas que se acumulam até o time desistir:

1. **É lento.** Subir um emulador, instalar o app, navegar até a tela, esperar animação, clicar. Cada teste leva dezenas de segundos. Uma suíte de 50 vira dez minutos. Ninguém roda dez minutos a cada commit.
2. **É frágil.** O botão mudou de lugar, a animação atrasou meio segundo, o emulador engasgou — e o teste fica vermelho sem nenhum bug real. Depois de o terceiro alarme falso, o time aprende a ignorar os vermelhos. E um time que ignora teste vermelho tem o pior dos mundos: o custo do teste sem o benefício.
3. **Cobre pouco por muito.** Um teste de UE2E gigante exercita um caminho feliz e dois campos. As 15 regras de cálculo por trás daquela tela continuam descobertas.

Então o time conclui "testar mobile é difícil, não compensa" — e segue sem rede de segurança. O erro não foi não testar. Foi **tentar testar no lugar mais caro possível.**

## A virada: a maior parte da sua lógica não é mobile

Lembra da [arquitetura](./architecture.md)? Aquela separação entre Core (cérebro) e Maui (cara) não era só por organização. Era **exatamente pra isto**.

A regra de cálculo de desconto, a decisão de quando habilitar o botão, o que fazer quando a API devolve 500, como ordenar a lista — nada disso é "mobile". É lógica C# comum, vivendo numa lib .NET pura. E lógica C# comum você testa do jeito mais barato, rápido e estável que existe: um teste unitário que roda em milissegundos, sem emulador, em qualquer máquina.

É por isso que no [reference-app](../reference-app) a ViewModel mora no `TaskApp.Core`. Os 6 testes dela rodam em **14 milissegundos**. Eu rodo eles a cada vez que salvo um arquivo. Esse é o feedback loop que muda a forma como você programa.

## A pirâmide, e por que ela tem essa forma

```
        ╱╲   UI / E2E (Appium)      ← poucos. Caros, frágeis. Só fluxos que NÃO podem quebrar (login, pagamento)
       ╱──╲
      ╱────╲  Integração            ← alguns. Repositório real contra um SQLite de verdade
     ╱──────╲
    ╱────────╲ Unit (ViewModels)    ← MUITOS. Rápidos, estáveis, baratos. É AQUI que mora sua confiança
   ╱──────────╲
```

A pirâmide não é uma regra estética. É uma consequência econômica: você concentra esforço onde o teste é barato e estável (a base), e usa os testes caros (o topo) com parcimônia cirúrgica, só nos fluxos que destruiriam o negócio se quebrassem. Quem inverte a pirâmide — muito E2E, pouco unitário — acaba com uma suíte lenta, instável e que mesmo assim não pega os bugs de lógica.

## Como um teste de ViewModel se parece

A beleza é que não tem mágica. Você constrói a ViewModel passando um fake, mexe nela como a UI mexeria, e verifica o resultado:

```csharp
[Fact]
public async Task AddTask_AddsToCollection_AndClearsInput()
{
    // Arrange — monta o cenário com um repositório em memória (sem banco, sem MAUI)
    var repo = new InMemoryTaskRepository();
    var sut  = new TaskListViewModel(repo);   // sut = "system under test"
    sut.NewTitle = "Comprar pneus";

    // Act — executa o comando EXATAMENTE como o botão faria
    await sut.AddTaskCommand.ExecuteAsync(null);

    // Assert — verifica o estado E o efeito colateral
    Assert.Single(sut.Tasks);
    Assert.Equal(string.Empty, sut.NewTitle);   // o input foi limpo? regra de UX testada.
}
```

Três coisas que valem ouro aqui:

- **Padrão AAA (Arrange, Act, Assert).** Não é firula — é o que torna o teste legível pra próxima pessoa em 5 segundos. Cenário, ação, verificação. Sempre nessa ordem, sempre separados.
- **Você invoca o comando, não o método privado.** `AddTaskCommand.ExecuteAsync(null)` é literalmente o que o botão dispara. Você testa a porta de entrada real, não os bastidores.
- **Você verifica o efeito colateral, não só o resultado óbvio.** Que a tarefa entrou na lista é o óbvio. Que o campo de texto foi limpo é a *regra de UX* — e é justamente o tipo de coisa que quebra sem ninguém perceber. Teste o que o usuário sentiria.

## O que vale testar (e o que não vale)

Não persiga 100% de cobertura. Cobertura cega faz você escrever teste pra getter trivial e ganhar um número bonito que não significa nada. Persiga **cobrir a lógica de decisão**:

- ✅ Comandos produzem o estado certo (adicionar, remover, alternar)
- ✅ `CanExecute` habilita e desabilita na hora certa — a regra do botão
- ✅ Propriedades calculadas (tipo `RemainingCount`) reagem quando a base muda
- ✅ Notificações (`PropertyChanged`, `CanExecuteChanged`) disparam quando deveriam
- ✅ **Os caminhos de erro** — o repositório lançou exceção, e aí? A ViewModel trata ou o app morre?

O último é o mais negligenciado e o mais valioso. Todo mundo testa o caminho feliz. Bug de produção quase sempre mora no caminho triste.

## Fakes ou Mocks?

- **Fake** (como o `InMemoryTaskRepository`) — uma implementação de verdade, simplificada, que *funciona*. Na maioria dos casos é tudo que você precisa, e deixa o teste limpo.
- **Mock** (Moq, NSubstitute) — quando você precisa **verificar a interação em si**: "o repositório foi chamado exatamente uma vez, com este argumento". Útil pra garantir que você não está, por exemplo, salvando duas vezes.

Comece com fakes. Vá pra mocks só quando a pergunta do teste for sobre *como* o colaborador foi usado, não sobre o resultado final.

## Integração e UI, no lugar certo

- **Integração** — aqui você troca o fake por um `SqliteTaskRepository` de verdade, apontando pra um banco em arquivo temporário. É o teste que pega o bug que o unitário não vê: a query SQL errada, o mapeamento de coluna trocado, a migration esquecida.
- **UI / E2E (Appium)** — reservado pros 2 ou 3 fluxos que, se quebrarem, é incidente: o login funciona? o pagamento completa? Caros e frágeis por natureza — então você paga esse preço só onde o risco justifica.

## E no CI, o fechamento do ciclo

```yaml
- run: dotnet test reference-app/tests/TaskApp.Core.Tests
```

Uma linha. Roda em qualquer runner Linux ou Windows, **sem instalar o workload do MAUI**, porque o Core não depende dele. Isso significa feedback em segundos a cada Pull Request. O time abre um PR, o CI roda a lógica inteira, e ninguém faz merge de uma regressão sem saber.

É esse o objetivo final de tudo: não "ter testes" como troféu, mas ter **um sinal rápido e confiável que te avisa antes do usuário**. Arquitetura boa é o que torna esse sinal barato o suficiente pra você de fato usar.

➡️ Continua em: [a arquitetura que viabiliza isto](./architecture.md) · [layout](./layout.md) · [performance](./performance.md)
