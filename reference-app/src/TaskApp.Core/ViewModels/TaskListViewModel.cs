using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskApp.Core.Models;
using TaskApp.Core.Services;

namespace TaskApp.Core.ViewModels;

/// <summary>
/// ViewModel da lista de tarefas. `partial` porque o CommunityToolkit.Mvvm
/// gera, em tempo de compilação, as propriedades observáveis e os comandos
/// a partir dos atributos — sem boilerplate de INotifyPropertyChanged.
///
/// Depende só de <see cref="ITaskRepository"/> (abstração), o que torna a
/// classe 100% testável sem MAUI/UI.
/// </summary>
public partial class TaskListViewModel : ObservableObject
{
    private readonly ITaskRepository _repo;

    public TaskListViewModel(ITaskRepository repo) => _repo = repo;

    public ObservableCollection<TaskItem> Tasks { get; } = [];

    /// <summary>Quantas tarefas ainda estão pendentes (binding na UI).</summary>
    public int RemainingCount => Tasks.Count(t => !t.IsCompleted);

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddTaskCommand))]
    private string _newTitle = string.Empty;

    /// <summary>Carrega as tarefas do repositório. Idempotente sob IsBusy.</summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            Tasks.Clear();
            foreach (var t in await _repo.GetAllAsync())
                Tasks.Add(t);
            OnPropertyChanged(nameof(RemainingCount));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanAddTask() => !string.IsNullOrWhiteSpace(NewTitle);

    /// <summary>Adiciona uma tarefa. O botão só habilita com título válido.</summary>
    [RelayCommand(CanExecute = nameof(CanAddTask))]
    private async Task AddTaskAsync()
    {
        var task = new TaskItem { Title = NewTitle.Trim() };
        await _repo.AddAsync(task);
        Tasks.Insert(0, task);
        NewTitle = string.Empty;
        OnPropertyChanged(nameof(RemainingCount));
    }

    [RelayCommand]
    private async Task ToggleAsync(TaskItem item)
    {
        item.IsCompleted = !item.IsCompleted;
        await _repo.UpdateAsync(item);
        OnPropertyChanged(nameof(RemainingCount));
    }

    [RelayCommand]
    private async Task DeleteAsync(TaskItem item)
    {
        await _repo.DeleteAsync(item.Id);
        Tasks.Remove(item);
        OnPropertyChanged(nameof(RemainingCount));
    }
}
