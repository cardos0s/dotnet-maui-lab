namespace TaskApp.Core.Models;

/// <summary>
/// Entidade de domínio. Imutável onde faz sentido (Id/CreatedAt via init).
/// Sem nenhuma dependência de UI ou framework — domínio puro.
/// </summary>
public sealed class TaskItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Title { get; set; }
    public bool IsCompleted { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
