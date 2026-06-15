using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskApp.Core.Models;
using TaskApp.Core.Services;

namespace TaskApp.Core.ViewModels;

/// <summary>
/// ViewModel da tela de EDIÇÃO de uma tarefa. Demonstra:
///  · uma segunda tela com seu próprio estado;
///  · carregar um item por Id (parâmetro de navegação);
///  · salvar e voltar — tudo via abstrações, então 100% testável.
/// </summary>
public partial class TaskEditViewModel : ObservableObject
{
    private readonly ITaskRepository _repo;
    private readonly INavigationService _nav;
    private TaskItem? _editing;

    public TaskEditViewModel(ITaskRepository repo, INavigationService nav)
    {
        _repo = repo;
        _nav = nav;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _title = string.Empty;

    [ObservableProperty]
    private bool _isCompleted;

    /// <summary>Carrega a tarefa a ser editada a partir do Id recebido na navegação.</summary>
    public async Task LoadAsync(Guid id)
    {
        var all = await _repo.GetAllAsync();
        _editing = all.FirstOrDefault(t => t.Id == id);
        if (_editing is not null)
        {
            Title = _editing.Title;
            IsCompleted = _editing.IsCompleted;
        }
    }

    private bool CanSave() => _editing is not null && !string.IsNullOrWhiteSpace(Title);

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (_editing is null) return;
        _editing.Title = Title.Trim();
        _editing.IsCompleted = IsCompleted;
        await _repo.UpdateAsync(_editing);
        await _nav.GoBackAsync();
    }

    [RelayCommand]
    private Task CancelAsync() => _nav.GoBackAsync();
}
