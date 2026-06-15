using TaskApp.Core.Services;

namespace TaskApp.Core.Tests;

/// <summary>
/// Dublê de navegação pros testes. Não navega de verdade (não há UI) — apenas
/// REGISTRA o que a ViewModel pediu, pra gente verificar "a ViewModel mandou
/// navegar pra edição da tarefa certa?". É o padrão de testar navegação sem app.
/// </summary>
public sealed class FakeNavigationService : INavigationService
{
    public Guid? NavigatedToEditId { get; private set; }
    public int GoBackCount { get; private set; }

    public Task GoToEditAsync(Guid taskId)
    {
        NavigatedToEditId = taskId;
        return Task.CompletedTask;
    }

    public Task GoBackAsync()
    {
        GoBackCount++;
        return Task.CompletedTask;
    }
}
