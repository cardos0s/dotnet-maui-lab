namespace TaskApp.Core.Services;

/// <summary>
/// Abstração de navegação. As ViewModels navegam por INTENÇÃO ("vá editar a
/// tarefa X"), sem nunca tocar em Shell, NavigationPage ou qualquer API do MAUI.
///
/// Por quê: se a ViewModel chamasse Shell.Current.GoToAsync diretamente, ela
/// voltaria a depender do MAUI e perderia a testabilidade. Com esta interface,
/// a implementação real (que mexe no Shell) vive na camada de UI, e os testes
/// injetam um fake que só registra "pra onde a ViewModel pediu pra ir".
///
/// É o padrão que o guides/architecture.md prega — aqui implementado de verdade.
/// </summary>
public interface INavigationService
{
    Task GoToEditAsync(Guid taskId);
    Task GoBackAsync();
}
