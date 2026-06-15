# 📱 Recursos do dispositivo — GPS, câmera, sensores e permissões

> Um app mobile que não usa nada do celular é só um site. A graça é acessar a câmera, a localização, os contatos, o sensor de movimento. O MAUI te dá isso de um jeito unificado — e este guia mostra como, sem esquecer da parte que todo mundo esquece: **pedir permissão**.

## A API unificada: o que era "MAUI Essentials"

O MAUI traz dezenas de funcionalidades de dispositivo numa API só, igual em todas as plataformas. Você chama o mesmo código C# e ele fala com o GPS do Android e do iPhone por baixo. Os mais usados:

| Quero... | Uso |
|---|---|
| Saber a localização | `Geolocation` |
| Tirar/escolher foto | `MediaPicker` |
| Saber se tem internet | `Connectivity` |
| Vibrar / dar feedback tátil | `Vibration` / `HapticFeedback` |
| Ler sensores (acelerômetro, giroscópio) | `Accelerometer`, `Gyroscope`... |
| Abrir um link no navegador | `Browser` |
| Compartilhar texto/arquivo | `Share` |
| Copiar/colar | `Clipboard` |
| Saber sobre a bateria | `Battery` |
| Discar, mandar SMS, e-mail | `PhoneDialer`, `Sms`, `Email` |

A beleza é a uniformidade: aprendeu um, o padrão dos outros é igual.

## Exemplo: pegar a localização

```csharp
var local = await Geolocation.GetLocationAsync(new GeolocationRequest
{
    DesiredAccuracy = GeolocationAccuracy.Medium,
    Timeout = TimeSpan.FromSeconds(10)
});

if (local != null)
    Console.WriteLine($"Você está em {local.Latitude}, {local.Longitude}");
```

## A parte que todo mundo esquece: permissões

Aqui está o detalhe que faz o app crashar em produção mas funcionar no seu teste. **Recursos sensíveis exigem permissão do usuário** — localização, câmera, microfone, contatos. E permissão tem duas camadas, as duas obrigatórias:

### Camada 1: declarar no manifesto
Você precisa dizer ao sistema, **no arquivo de configuração de cada plataforma**, que o app usa aquele recurso. Localização no Android, por exemplo, vai no `Platforms/Android/AndroidManifest.xml`:
```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```
No iOS, vai no `Platforms/iOS/Info.plist`, e o iOS ainda exige uma **frase explicando o porquê** (que aparece pro usuário):
```xml
<key>NSLocationWhenInUseUsageDescription</key>
<string>Usamos sua localização para mostrar o kart mais próximo.</string>
```
Esqueceu de declarar? O app é **rejeitado pela loja** ou crasha ao pedir. Esse é um dos motivos nº 1 de "funciona na minha máquina, quebra na do usuário".

### Camada 2: pedir em runtime
Declarar não basta — você precisa **perguntar ao usuário** na hora, e ele pode dizer não:
```csharp
var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
if (status != PermissionStatus.Granted)
{
    await DisplayAlert("Permissão necessária",
        "Precisamos da localização pra essa função.", "OK");
    return;   // o usuário negou — trate com elegância, não force
}
// só chegou aqui se ele permitiu
```

A regra de ouro de UX: **peça a permissão no momento que faz sentido**, não tudo de cara na abertura. Vai usar o mapa? Aí peça localização. O usuário entende "estou clicando no mapa, faz sentido pedir GPS" — e nega muito menos do que quando você bombardeia ele com 5 pop-ups antes de ele ver o app.

## Sempre trate o "não"

Câmera negada, GPS desligado, sem internet — todos são estados **normais**, não exceções raras. Um app maduro tem um plano pra cada:
```csharp
if (Connectivity.NetworkAccess != NetworkAccess.Internet)
{
    // mostra os dados em cache (ver guia de offline-first), não uma tela de erro
}
```

O usuário no modo avião não é um bug pra você logar — é uma situação que você projeta. Cada recurso de dispositivo tem o caminho do "deu certo" e o caminho do "não rolou", e os dois precisam existir.

## Onde isso encaixa na arquitetura

Igual à rede e ao banco: **esconda atrás de uma interface.** A ViewModel não deve chamar `Geolocation` direto (de novo, vira código não-testável amarrado à plataforma). Crie um `ILocationService`, a implementação real usa `Geolocation`, e nos testes você injeta um fake que devolve uma coordenada fixa. Assim você testa "o que a tela faz quando recebe a localização" sem precisar de um GPS de verdade.

## O resumo

Os recursos do dispositivo são o que tornam seu app *mobile* de verdade, e a API unificada do MAUI deixa isso fácil. O que não é opcional: **declarar a permissão no manifesto E pedir em runtime, no momento certo, tratando o "não" com dignidade.** Faça isso e seu app usa o poder do aparelho sem virar aquele que pede tudo e crasha quando você recusa.

➡️ Relacionado: [offline-first](./local-storage.md) (pro `Connectivity`) · [acessibilidade](./accessibility.md) (todo mundo merece usar seu app).
