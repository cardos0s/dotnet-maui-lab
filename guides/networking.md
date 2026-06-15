# 🌐 Consumindo APIs — falando com o mundo lá fora

> Quase todo app real busca dados de algum servidor. Este guia é sobre fazer isso direito: pegar JSON de uma API, transformar em objetos C#, e — o que separa app amador de profissional — **tratar o que dá errado**, porque a internet do usuário vai falhar.

## O fluxo, em três passos

Conversar com uma API REST é sempre a mesma dança:
1. **Pedir** — fazer uma requisição HTTP (GET, POST...) pra uma URL.
2. **Receber** — o servidor responde com um texto, quase sempre em **JSON**.
3. **Transformar** — converter esse JSON em objetos C# que seu app entende (isso se chama *desserializar*).

O MAUI já vem com tudo pra isso: `HttpClient` pra falar HTTP, e `System.Text.Json` pra converter JSON. Você não instala nada.

## O básico que funciona

Digamos que existe uma API que devolve produtos. Primeiro, a classe que representa um produto:

```csharp
public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public decimal Preco { get; set; }
}
```

E o serviço que busca:

```csharp
public class ProdutoApi
{
    private readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("https://api.exemplo.com/")
    };

    public async Task<List<Produto>> GetProdutosAsync()
    {
        // GetFromJsonAsync faz tudo: pede, recebe e desserializa de uma vez.
        var produtos = await _http.GetFromJsonAsync<List<Produto>>("produtos");
        return produtos ?? [];
    }
}
```

`GetFromJsonAsync<T>` é o atalho que junta os três passos. Você diz o tipo (`List<Produto>`), ele busca, lê o JSON e te devolve os objetos prontos. Repara no `await` — chamada de rede é **sempre** assíncrona (ver [performance](./performance.md), seção de UI thread).

## Mandando dados (POST)

Pra criar algo no servidor:
```csharp
public async Task CriarAsync(Produto novo)
{
    var resposta = await _http.PostAsJsonAsync("produtos", novo);
    resposta.EnsureSuccessStatusCode();   // lança se veio erro (4xx, 5xx)
}
```
`PostAsJsonAsync` serializa o objeto pra JSON e manda. `EnsureSuccessStatusCode` transforma um erro HTTP num exception que você pode capturar.

## A parte que separa profissional de amador: tratar erro

Aqui está o ponto. O código acima funciona **quando tudo dá certo**. Mas no mundo real:
- a internet do usuário cai no meio;
- o servidor está fora do ar;
- a resposta demora 30 segundos;
- veio um JSON diferente do esperado.

Um app amador ignora isso e **trava ou fecha** quando qualquer um acontece. Um app profissional **espera** que aconteça:

```csharp
public async Task<Resultado<List<Produto>>> GetProdutosAsync()
{
    try
    {
        var produtos = await _http.GetFromJsonAsync<List<Produto>>("produtos");
        return Resultado.Ok(produtos ?? []);
    }
    catch (HttpRequestException)   // sem internet, servidor fora, erro HTTP
    {
        return Resultado.Erro("Não foi possível conectar. Verifique sua internet.");
    }
    catch (TaskCanceledException)  // timeout — demorou demais
    {
        return Resultado.Erro("O servidor demorou a responder. Tente de novo.");
    }
}
```

O detalhe sênior: você captura **exceptions específicas** e devolve uma mensagem que o usuário entende, em vez de deixar o app morrer com um stack trace. A tela então reage a esse resultado mostrando a mensagem certa. Esse é o "estado de erro explícito" que o [guia de arquitetura](./architecture.md) menciona.

## Timeout — não deixe o usuário esperando pra sempre

Configure um tempo limite. Sem isso, uma conexão ruim deixa o app "carregando" eternamente:
```csharp
_http.Timeout = TimeSpan.FromSeconds(15);
```
Passou de 15 segundos? Vira `TaskCanceledException`, que você já trata acima.

## Onde isso encaixa na arquitetura

Não chame `HttpClient` direto da ViewModel. Esconda atrás de uma interface, igual fizemos com o repositório:

```csharp
public interface IProdutoRepository
{
    Task<IReadOnlyList<Produto>> GetAllAsync();
}

// uma implementação fala com a API; outra usa cache local; uma terceira é fake nos testes.
public class ApiProdutoRepository : IProdutoRepository { /* HttpClient aqui */ }
```

Assim a ViewModel não sabe (nem se importa) se os dados vêm de uma API, de um cache ou de um banco. Você troca a fonte sem tocar na tela, e testa a ViewModel com um fake — exatamente a ideia do [reference-app](../reference-app).

## Registre o HttpClient na DI (não crie na mão)

Criar um `new HttpClient()` por toda parte é um anti-padrão conhecido (esgota conexões). O jeito certo é registrar uma vez na DI:

```csharp
// em MauiProgram.cs
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<IProdutoRepository, ApiProdutoRepository>();
```

E a implementação recebe ele pelo construtor. Um `HttpClient` pro app, reaproveitado.

## Resiliência: o nível seguinte

Quando você estiver pronta pra subir o nível, a biblioteca **Polly** adiciona *retry* (tenta de novo automaticamente se falhar), *circuit breaker* (para de tentar um servidor que está claramente fora) e *backoff* (espera progressiva entre tentativas). Não precisa no primeiro app — mas saiba que existe, porque é o que torna o app resiliente a falhas momentâneas de rede sem você escrever tudo na mão.

## O resumo

Buscar dados é fácil; o difícil — e o que importa — é **assumir que vai falhar e tratar com elegância**. Captura específica, mensagem clara pro usuário, timeout configurado, e a chamada escondida atrás de uma interface. Faça isso e seu app sobrevive ao metrô, ao elevador e ao Wi-Fi de café.

➡️ Relacionado: [armazenamento local & offline-first](./local-storage.md) — pra quando você quer que o app funcione *sem* internet.
