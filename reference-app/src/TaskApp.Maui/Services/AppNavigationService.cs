using TaskApp.Core.Services;
using TaskApp.Maui.Views;

namespace TaskApp.Maui.Services;

/// <summary>
/// Implementação REAL do <see cref="INavigationService"/> — esta é a única peça
/// que conhece o MAUI/Navigation. As ViewModels chamam a interface; o "como"
/// navegar mora aqui, na camada de UI. Resolve a página da DI (já com sua
/// ViewModel) e empilha.
/// </summary>
public sealed class AppNavigationService : INavigationService
{
    private readonly IServiceProvider _services;

    public AppNavigationService(IServiceProvider services) => _services = services;

    private static INavigation Nav =>
        Application.Current!.Windows[0].Page!.Navigation;

    public async Task GoToEditAsync(Guid taskId)
    {
        var page = _services.GetRequiredService<TaskEditPage>();
        await page.InitializeAsync(taskId);   // carrega a tarefa na ViewModel
        await Nav.PushAsync(page);
    }

    public Task GoBackAsync() => Nav.PopAsync();
}
