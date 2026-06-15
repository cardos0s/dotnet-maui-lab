using Microsoft.Extensions.Logging;
using TaskApp.Core.Services;
using TaskApp.Core.ViewModels;
using TaskApp.Maui.Services;
using TaskApp.Maui.Views;

namespace TaskApp.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ---------------------------------------------------------------
        // Dependency Injection — o coração da testabilidade e do desacoplamento.
        //
        // A ViewModel pede ITaskRepository; o container resolve a implementação
        // registrada aqui. Trocar InMemory por SQLite/HTTP é UMA linha, sem
        // tocar em ViewModel, Page ou testes.
        // ---------------------------------------------------------------
        builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
        builder.Services.AddSingleton<INavigationService, AppNavigationService>();

        // Tela da lista: singleton (uma só, mantém estado).
        builder.Services.AddSingleton<TaskListViewModel>();
        builder.Services.AddSingleton<TaskListPage>();

        // Tela de edição: transient (cada edição começa limpa, com a tarefa carregada).
        builder.Services.AddTransient<TaskEditViewModel>();
        builder.Services.AddTransient<TaskEditPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
