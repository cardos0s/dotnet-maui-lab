# 🔄 CI/CD para MAUI — build, teste e entrega no automático

> Fazer build, rodar testes e gerar o pacote da loja **na mão**, toda vez, é lento e propenso a erro humano ("esqueci de rodar os testes", "assinei com a chave errada"). CI/CD automatiza isso: a máquina faz, sempre igual, a cada mudança. Este guia conecta o que você aprendeu com o mundo da entrega contínua.

## O que significam as siglas

- **CI (Continuous Integration / Integração Contínua):** a cada push ou Pull Request, uma máquina **builda o código e roda os testes** automaticamente. Se algo quebrou, você sabe em minutos — antes de virar problema de todo mundo.
- **CD (Continuous Delivery / Deployment):** a etapa seguinte — a mesma automação **empacota e entrega** o app (pra testadores ou pra loja).

O valor é simples: **nenhuma regressão passa despercebida, e ninguém depende da memória de uma pessoa** pra fazer o release certo.

## O insight que o reference-app já te deu

Lembra por que o [reference-app](../reference-app) separa o `TaskApp.Core` do MAUI? Aqui está mais um motivo, e talvez o melhor: **os testes do Core rodam no CI sem precisar do workload MAUI, sem emulador, sem nada de mobile.**

```yaml
- run: dotnet test reference-app/tests/TaskApp.Core.Tests
```

Uma linha, num runner Linux comum, em segundos. Toda a sua lógica de negócio é verificada a cada PR de graça. Se você tivesse enfiado a lógica no code-behind das telas, precisaria de um emulador no CI — lento, caro e instável. A arquitetura que você montou **paga o aluguel** justo aqui.

## Um pipeline de CI mínimo (GitHub Actions)

Um arquivo em `.github/workflows/ci.yml` já te dá feedback automático a cada push:

```yaml
name: ci
on:
  push:
    branches: [main]
  pull_request:

jobs:
  build-test:
    runs-on: ubuntu-latest        # Linux basta pra testar o Core
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      - name: Restore + Build do Core
        run: dotnet build reference-app/src/TaskApp.Core
      - name: Testes
        run: dotnet test reference-app/tests/TaskApp.Core.Tests
```

A partir de agora, todo Pull Request mostra um ✅ ou ❌ automático. Ninguém faz merge de código que quebrou os testes sem ver o vermelho gritando.

## Buildar o app mobile no CI (a parte mais pesada)

Testar o Core é leve. **Compilar o app MAUI** é outra história: precisa do workload, do SDK Android (e o build de iOS **exige um runner macOS**). Um job de build de app fica assim:

```yaml
  build-android:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      - run: dotnet workload install maui-android
      - run: dotnet publish -f net9.0-android -c Release
```

É mais lento, então um padrão comum é: **rodar os testes do Core em todo PR** (rápido, sempre), e **buildar o app só quando vai gerar uma versão** (em tags/releases). Você não precisa compilar o Android inteiro a cada vírgula.

## Os segredos: a parte sensível do CD

Pra **assinar** o app (ver [publicação](./publishing.md)), o CI precisa da sua keystore (Android) ou certificados (iOS). E aqui mora uma regra inegociável: **esses segredos NUNCA vão pro repositório.** Eles ficam guardados como **secrets** da plataforma de CI (em GitHub Actions, `Settings → Secrets`), criptografados, e o pipeline lê de lá em runtime:

```yaml
      - name: Assinar
        env:
          KEYSTORE_PASSWORD: ${{ secrets.KEYSTORE_PASSWORD }}
        run: # ... usa a variável, nunca o valor literal
```

É exatamente a mesma disciplina de segredos que aplicamos no [neuromind](https://github.com/cardos0s) e no [devops-lab](https://github.com/cardos0s/devops-lab): segredo é injetado por variável de ambiente, nunca commitado. Vazar uma keystore Android é catastrófico — quem a tem pode publicar updates falsos do seu app.

## O fluxo completo, de ponta a ponta

```
push / PR ──► CI builda o Core ──► roda os testes ──► ✅ ou ❌
                                                        │
                        (em uma tag de release) ────────┘
                                │
                        builda o app (Android/iOS) ──► assina (secrets) ──► sobe pra TestFlight/Play
```

Você abre um PR, o CI te diz na hora se quebrou algo. Quando marca uma release, a esteira gera o pacote assinado e entrega pros testadores — sem ninguém digitar um comando. O erro humano sai da equação.

## Onde ver isso de verdade

O **[devops-lab](https://github.com/cardos0s/devops-lab)** tem um pipeline CI/CD completo (build → teste → imagem → deploy) com a mesma anatomia — matriz de jobs, segredos como secrets, deploy condicional. A diferença é só o alvo (containers lá, app mobile aqui); os princípios são idênticos.

## O resumo

CI/CD troca "lembrar de fazer certo" por "a máquina faz certo, sempre". Comece pequeno: um job que **builda o Core e roda os testes a cada PR** — barato, rápido, e já elimina a regressão silenciosa (graças à arquitetura testável). Evolua pra buildar e assinar o app nos releases, com os segredos guardados como secrets do CI, nunca no código. É o último elo entre "eu programo" e "eu entrego software com confiança".

➡️ Relacionado: [testes](./testing.md) · [publicação](./publishing.md) · [arquitetura](./architecture.md).
