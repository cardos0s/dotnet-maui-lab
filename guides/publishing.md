# 🚀 Publicando na loja — do "funciona aqui" ao "está na App Store"

> Construir o app é metade do caminho. A outra metade — gerar um pacote de release assinado e fazer ele passar pela revisão das lojas — é onde muita gente trava, porque ninguém ensina. Este é o mapa.

## Debug vs Release: a virada de chave

Tudo que você rodou até agora foi um build de **Debug**: otimizado pra você desenvolver (hot reload, mensagens de erro completas, sem otimização pesada). O que vai pra loja é um build de **Release**, que é outro bicho:

- **Otimizado** — trimming (remove código não usado) e AOT (compila antes), pra abrir rápido e ocupar menos (ver [performance](./performance.md)).
- **Sem ferramentas de debug** — menor e mais seguro.
- **Assinado** — com um certificado que prova que o app é seu.

```bash
dotnet publish -f net9.0-android -c Release
dotnet publish -f net9.0-ios -c Release
```

A primeira vez que você roda em Release e algo quebra que funcionava em Debug, quase sempre é o **trimming** removendo código que ele *achou* que não era usado (comum com reflection/serialização). É um clássico — e mais um motivo pra usar compiled bindings e serialização com source generators, que o trimming entende.

## Assinatura: provando que o app é seu

As lojas não aceitam um app anônimo. Cada app é **assinado** com um certificado, que é o que diz "isto foi publicado por esta conta, e não foi adulterado depois".

- **Android:** você gera uma *keystore* (um arquivo com sua chave de assinatura) e assina o pacote com ela. **Guarde essa keystore como ouro** — se você perder, não consegue mais atualizar o app publicado (literalmente teria que publicar como um app novo). Faça backup em lugar seguro.
- **iOS:** a Apple controla tudo via *certificados* e *provisioning profiles*, gerados no portal de desenvolvedor. É mais burocrático, e **exige um Mac** pra compilar e assinar.

> 💡 Ferramentas modernas (e o próprio Visual Studio) automatizam boa parte disso. Mas entenda o conceito: **sem assinatura válida, não entra na loja.**

## Ícone e splash: o app precisa ter cara

Antes de publicar, o app precisa de identidade visual nos lugares certos:
- **Ícone do app** — vai em `Resources/AppIcon/`. O MAUI gera automaticamente todos os tamanhos (de aparelhos diferentes) a partir de um SVG. Você fornece um, ele faz o resto.
- **Splash screen** — a tela que aparece enquanto o app carrega, em `Resources/Splash/`. Também a partir de um SVG.

Não subestime: um app sem ícone decente ou com splash genérico grita "amador" antes de o usuário ver a primeira tela.

## Versionamento: dois números que importam

Toda submissão tem dois números, e confundi-los trava a publicação:
- **Versão de exibição** (`ApplicationDisplayVersion`, ex.: `1.2.0`) — o que o usuário vê na loja. Pra humanos.
- **Versão de build** (`ApplicationVersion`, ex.: `15`) — um número que **sempre aumenta** a cada submissão. As lojas recusam um upload com número igual ou menor que um já enviado.

Regra: a cada envio pra loja, **incremente o número de build**, sempre. Mesmo num hotfix mínimo.

## As lojas, e a parte que ninguém avisa: a revisão

- **Google Play** — você sobe o pacote (`.aab`, o formato preferido) no Play Console. A revisão costuma ser rápida (horas a um par de dias).
- **Apple App Store** — sobe pelo App Store Connect. A revisão é **mais rigorosa e mais lenta** (de um a vários dias), e a Apple **rejeita** por motivos que pegam todo mundo de surpresa.

Os motivos campeões de rejeição (vale ler antes de submeter):
- Permissão pedida sem justificativa clara no `Info.plist` (a tal frase do guia de [recursos do dispositivo](./device-features.md)).
- App que parece "incompleto" ou é só um site embrulhado.
- Faltando política de privacidade.
- Crash no fluxo que o revisor testou.

A lição: **leia as diretrizes da loja antes de submeter**, não depois de ser rejeitada. Cada rejeição custa dias.

## Distribuir sem a loja (pra testar)

Antes do mundo todo, você quer um grupo testando. As ferramentas pra isso:
- **Android:** mande o `.apk` direto, ou use a *Internal Testing* track do Play.
- **iOS:** **TestFlight** — a forma oficial de distribuir betas pra testadores, sem passar pela revisão completa da loja.

Esse passo é ouro: usuários reais em aparelhos reais acham bugs que nenhum emulador acha.

## Automatize quando doer (CI/CD)

Fazer release assinado na mão, toda vez, é tedioso e propenso a erro. Quando o app amadurecer, monte um pipeline (GitHub Actions, Azure DevOps) que builda, assina e sobe o pacote automaticamente a cada release. Os segredos (keystore, certificados) ficam guardados como *secrets* do CI, nunca no código. (É a mesma ideia do pipeline que existe no [devops-lab](https://github.com/cardos0s/devops-lab), aplicada a mobile.)

## O resumo

Publicar é: **build de Release** (otimizado, sem debug), **assinado** com seu certificado (guarde a keystore Android com a vida), com **ícone/splash** decentes e **número de build sempre crescente**, passando pela **revisão** de cada loja — Apple mais rígida que Google. Teste com gente real via TestFlight/Internal Testing antes, leia as diretrizes antes de submeter, e automatize quando o processo manual começar a doer.

Chegou aqui? Você não é mais alguém que "mexe com MAUI". Você publica apps. 🎉

➡️ Volte ao começo: [trilha completa de guias](./README.md) · ou veja a [arquitetura](./architecture.md) que sustenta um app pronto pra produção.
