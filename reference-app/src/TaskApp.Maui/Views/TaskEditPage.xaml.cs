using TaskApp.Core.ViewModels;

namespace TaskApp.Maui.Views;

public partial class TaskEditPage : ContentPage
{
    private readonly TaskEditViewModel _vm;

    public TaskEditPage(TaskEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    /// <summary>Chamado pela navegação antes de empilhar — carrega a tarefa pelo Id.</summary>
    public Task InitializeAsync(Guid taskId) => _vm.LoadAsync(taskId);
}
