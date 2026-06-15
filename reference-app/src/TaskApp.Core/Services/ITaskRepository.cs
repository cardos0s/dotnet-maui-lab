using TaskApp.Core.Models;

namespace TaskApp.Core.Services;

/// <summary>
/// Abstração de persistência. As ViewModels dependem DESTA interface,
/// nunca de uma implementação concreta (Dependency Inversion). Em produção
/// pode ser SQLite/Supabase/HTTP; nos testes, um fake em memória.
/// </summary>
public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(TaskItem item, CancellationToken ct = default);
    Task UpdateAsync(TaskItem item, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
