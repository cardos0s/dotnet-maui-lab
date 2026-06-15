# 💾 Armazenamento local & offline-first — o app que funciona no metrô

> Internet cai. O app não pode cair junto. Este guia é sobre guardar dados **no próprio aparelho** — desde uma simples preferência até um banco inteiro — e sobre a mentalidade *offline-first*, que é o que separa um app que frustra de um app que respeita o usuário.

## Cada coisa no seu lugar — as 4 opções

O MAUI te dá quatro formas de guardar dados localmente, e escolher a errada causa dor. A regra é simples: **tamanho e sensibilidade do dado decidem.**

| Você quer guardar | Use | Por quê |
|---|---|---|
| Uma configuração simples (tema, último filtro) | **Preferences** | Chave-valor leve, tipo um dicionário persistente |
| Algo sensível (token, senha) | **SecureStorage** | Criptografado pelo sistema (Keychain/Keystore) |
| Muitos registros, com consulta | **SQLite** | Um banco de verdade, com queries |
| Um arquivo (imagem, JSON, PDF) | **File System** | Arquivos na pasta do app |

O erro clássico é guardar um token de login em `Preferences` (que é texto puro, qualquer um lê) ou tentar guardar uma lista de 5.000 itens em `Preferences` (que não foi feito pra isso). Combine o dado com a ferramenta.

## Preferences — para configurações

A coisa mais simples que existe. Um dicionário que sobrevive ao app fechar:

```csharp
// guardar
Preferences.Set("tema", "escuro");
Preferences.Set("primeira_vez", false);

// ler (com valor padrão se não existir)
string tema = Preferences.Get("tema", "claro");
bool primeira = Preferences.Get("primeira_vez", true);

// remover
Preferences.Remove("tema");
```

Use pra: tema escolhido, último filtro, "já viu o onboarding?", id do último usuário. **Não** use pra: senhas, listas grandes, objetos complexos.

## SecureStorage — para segredos

Mesma cara do Preferences, mas **criptografado** pelo sistema operacional. É onde vai token de autenticação, refresh token, qualquer coisa que vazaria se alguém abrisse o arquivo do app:

```csharp
await SecureStorage.SetAsync("auth_token", token);
string? token = await SecureStorage.GetAsync("auth_token");
SecureStorage.Remove("auth_token");
```

Repara que é `async` (o Preferences não é) — porque acessar o cofre do sistema é uma operação mais pesada. **Regra inegociável:** token, senha e chave **nunca** em Preferences. Sempre SecureStorage.

## SQLite — para dados de verdade

Quando você tem *registros* — uma lista de tarefas, um cache de produtos, histórico — você quer um banco. **SQLite** é um banco completo que roda dentro do aparelho, sem servidor. A biblioteca `sqlite-net-pcl` deixa isso quase trivial:

```csharp
public class TarefaDb
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public bool Concluida { get; set; }
}

public class TarefaRepository
{
    private readonly SQLiteAsyncConnection _db;

    public TarefaRepository(string caminhoDoArquivo)
    {
        _db = new SQLiteAsyncConnection(caminhoDoArquivo);
        _db.CreateTableAsync<TarefaDb>();   // cria a tabela se não existir
    }

    public Task<List<TarefaDb>> GetAllAsync() => _db.Table<TarefaDb>().ToListAsync();
    public Task<int> AddAsync(TarefaDb t) => _db.InsertAsync(t);
    public Task<int> UpdateAsync(TarefaDb t) => _db.UpdateAsync(t);
    public Task<int> DeleteAsync(TarefaDb t) => _db.DeleteAsync(t);
}
```

O caminho do arquivo você monta com `FileSystem.AppDataDirectory` (a pasta privada do app):
```csharp
var caminho = Path.Combine(FileSystem.AppDataDirectory, "tarefas.db3");
```

E — voltando à [arquitetura](./architecture.md) — esse `TarefaRepository` é exatamente a implementação concreta que você troca pela `InMemory` com **uma linha** na DI. A promessa do reference-app vira realidade aqui.

## File System — para arquivos

Pra salvar um arquivo (um JSON exportado, uma foto baixada):
```csharp
var caminho = Path.Combine(FileSystem.AppDataDirectory, "backup.json");
await File.WriteAllTextAsync(caminho, json);
var lido = await File.ReadAllTextAsync(caminho);
```
`AppDataDirectory` é privado (só seu app vê). Pra arquivos que o usuário deve acessar por fora, há `FileSystem.CacheDirectory` e seletores de arquivo — mas comece pelo privado.

## A mentalidade offline-first

Agora a parte que é filosofia, não API. **Offline-first** significa projetar o app assumindo que a internet é um *bônus*, não um *pré-requisito*. O usuário no metrô, no avião, no elevador, no interior — todos eles merecem um app que funciona.

Na prática, a inversão é esta:

- **App online-first (o comum):** abre → mostra "carregando" → busca da API → se a rede falhar, tela de erro, usuário travado.
- **App offline-first:** abre → **mostra na hora o que está salvo localmente** → busca atualização da API em segundo plano → se vier, atualiza suave; se não vier, o usuário nem percebe, porque já tinha conteúdo.

O padrão central é o **cache local como fonte primária**:

```
1. Tela pede dados.
2. Repositório devolve IMEDIATAMENTE o que está no SQLite (rápido, sempre funciona).
3. Em paralelo, tenta buscar da API.
4. Deu certo? Atualiza o SQLite e a tela, suavemente.
5. Deu errado? Tudo bem — o usuário já está vendo os dados locais.
```

O usuário nunca olha pra uma tela em branco esperando a rede. A rede serve pra *manter atualizado*, não pra *funcionar*.

## Sincronização: o desafio honesto

Offline-first traz uma pergunta difícil: e se o usuário **editar offline** e o servidor também mudou? Isso é *sincronização*, e é genuinamente difícil. Estratégias comuns:
- **Last-write-wins** — a última edição ganha (simples, mas pode perder dados).
- **Fila de mudanças** — guarda localmente o que o usuário fez offline e reenvia quando a rede volta.
- **Timestamps/versão** — cada registro carrega quando mudou, e você resolve conflito comparando.

Não precisa resolver isso no primeiro app. Mas saber que o problema existe — e que "offline-first" é mais que "guardar um cache" — já te coloca à frente.

## O resumo

Escolha o cofre pelo dado: **Preferences** pra config, **SecureStorage** pra segredo, **SQLite** pra registros, **File** pra arquivos. E adote a inversão offline-first: **mostre o local primeiro, atualize com a rede depois.** O app fica mais rápido (não espera rede pra desenhar) e mais respeitoso (funciona onde o usuário está).

➡️ Relacionado: [consumindo APIs](./networking.md) — a outra metade da história · [arquitetura](./architecture.md) — onde o repositório se encaixa.
