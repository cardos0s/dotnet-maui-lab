using System.Collections.Concurrent;
using TaskApp.Core.Models;

namespace TaskApp.Core.Services;

/// <summary>
/// Implementação em memória, thread-safe. Serve como default para
/// desenvolvimento/demo e como referência da interface. Trocar por uma
/// implementação SQLite/HTTP é só registrar outra no container de DI.
/// </summary>
public sealed class InMemoryTaskRepository : ITaskRepository
{
    private readonly ConcurrentDictionary<Guid, TaskItem> _store = new();

    public Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyList<TaskItem> result = _store.Values
            .OrderByDescending(t => t.CreatedAt)
            .ToList();
        return Task.FromResult(result);
    }

    public Task AddAsync(TaskItem item, CancellationToken ct = default)
    {
        _store[item.Id] = item;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TaskItem item, CancellationToken ct = default)
    {
        _store[item.Id] = item;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        _store.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
