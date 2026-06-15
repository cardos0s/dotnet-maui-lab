using System.Text.Json;
using TaskApp.Core.Models;

namespace TaskApp.Core.Services;

/// <summary>
/// Segunda implementação de <see cref="ITaskRepository"/> — persiste em um
/// arquivo JSON. Existe pra PROVAR a promessa da arquitetura: trocar a
/// persistência do app é trocar UMA linha na DI, e nada mais — nem ViewModel,
/// nem View, nem testes da ViewModel mudam.
///
/// Repare que esta classe NÃO depende do MAUI. O caminho do arquivo é injetado,
/// então em produção vem de FileSystem.AppDataDirectory e nos testes vem de um
/// arquivo temporário — a classe não sabe (nem precisa saber) a diferença.
/// </summary>
public sealed class JsonFileTaskRepository : ITaskRepository
{
    private readonly string _path;
    private readonly SemaphoreSlim _gate = new(1, 1); // serializa o acesso ao arquivo

    public JsonFileTaskRepository(string filePath) => _path = filePath;

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken ct = default)
    {
        var all = await ReadAsync(ct);
        return all.OrderByDescending(t => t.CreatedAt).ToList();
    }

    public async Task AddAsync(TaskItem item, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            var all = await ReadUnlockedAsync(ct);
            all.Add(item);
            await WriteAsync(all, ct);
        }
        finally { _gate.Release(); }
    }

    public async Task UpdateAsync(TaskItem item, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            var all = await ReadUnlockedAsync(ct);
            var idx = all.FindIndex(t => t.Id == item.Id);
            if (idx >= 0) all[idx] = item;
            await WriteAsync(all, ct);
        }
        finally { _gate.Release(); }
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            var all = await ReadUnlockedAsync(ct);
            all.RemoveAll(t => t.Id == id);
            await WriteAsync(all, ct);
        }
        finally { _gate.Release(); }
    }

    private async Task<List<TaskItem>> ReadAsync(CancellationToken ct)
    {
        await _gate.WaitAsync(ct);
        try { return await ReadUnlockedAsync(ct); }
        finally { _gate.Release(); }
    }

    private async Task<List<TaskItem>> ReadUnlockedAsync(CancellationToken ct)
    {
        if (!File.Exists(_path)) return [];
        await using var stream = File.OpenRead(_path);
        var items = await JsonSerializer.DeserializeAsync<List<TaskItem>>(stream, cancellationToken: ct);
        return items ?? [];
    }

    private async Task WriteAsync(List<TaskItem> items, CancellationToken ct)
    {
        await using var stream = File.Create(_path);
        await JsonSerializer.SerializeAsync(stream, items, cancellationToken: ct);
    }
}
