using TaskApp.Core.ViewModels;

namespace TaskApp.Maui.Views;

public partial class TaskListPage : ContentPage
{
    // A ViewModel é injetada pelo container (registrada em MauiProgram).
    // A View não sabe construir suas dependências — só recebe.
    public TaskListPage(TaskListViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Carrega os dados DEPOIS da UI aparecer (não bloqueia o startup).
        if (BindingContext is TaskListViewModel vm)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
