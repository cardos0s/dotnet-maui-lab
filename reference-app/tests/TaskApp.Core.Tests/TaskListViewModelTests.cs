using TaskApp.Core.Models;
using TaskApp.Core.Services;
using TaskApp.Core.ViewModels;
using Xunit;

namespace TaskApp.Core.Tests;

/// <summary>
/// Testes do <see cref="TaskListViewModel"/> — provam que a lógica de UI
/// está correta SEM precisar do MAUI/emulador. É o ganho de separar o Core.
/// </summary>
public class TaskListViewModelTests
{
    private static TaskListViewModel CreateSut(out ITaskRepository repo)
    {
        repo = new InMemoryTaskRepository();
        return new TaskListViewModel(repo);
    }

    [Fact]
    public async Task AddTask_AddsToCollection_AndClearsInput()
    {
        var sut = CreateSut(out _);
        sut.NewTitle = "Comprar pneus";

        await sut.AddTaskCommand.ExecuteAsync(null);

        Assert.Single(sut.Tasks);
        Assert.Equal("Comprar pneus", sut.Tasks[0].Title);
        Assert.Equal(string.Empty, sut.NewTitle);   // input limpo após adicionar
    }

    [Fact]
    public void AddTask_CannotExecute_WhenTitleIsBlank()
    {
        var sut = CreateSut(out _);

        sut.NewTitle = "   ";
        Assert.False(sut.AddTaskCommand.CanExecute(null));

        sut.NewTitle = "Treino";
        Assert.True(sut.AddTaskCommand.CanExecute(null));
    }

    [Fact]
    public async Task Load_PopulatesFromRepository()
    {
        var sut = CreateSut(out var repo);
        await repo.AddAsync(new TaskItem { Title = "A" });
        await repo.AddAsync(new TaskItem { Title = "B" });

        await sut.LoadCommand.ExecuteAsync(null);

        Assert.Equal(2, sut.Tasks.Count);
    }

    [Fact]
    public async Task Toggle_FlipsCompleted_AndUpdatesRemainingCount()
    {
        var sut = CreateSut(out _);
        sut.NewTitle = "Calibrar kart";
        await sut.AddTaskCommand.ExecuteAsync(null);
        var task = sut.Tasks[0];

        Assert.Equal(1, sut.RemainingCount);

        await sut.ToggleCommand.ExecuteAsync(task);

        Assert.True(task.IsCompleted);
        Assert.Equal(0, sut.RemainingCount);
    }

    [Fact]
    public async Task Delete_RemovesTaskFromCollection()
    {
        var sut = CreateSut(out _);
        sut.NewTitle = "Tarefa temporária";
        await sut.AddTaskCommand.ExecuteAsync(null);
        var task = sut.Tasks[0];

        await sut.DeleteCommand.ExecuteAsync(task);

        Assert.Empty(sut.Tasks);
    }

    [Fact]
    public async Task NewTitle_RaisesCanExecuteChanged_OnAddCommand()
    {
        var sut = CreateSut(out _);
        bool raised = false;
        sut.AddTaskCommand.CanExecuteChanged += (_, _) => raised = true;

        sut.NewTitle = "qualquer";   // deve disparar reavaliação do CanExecute

        Assert.True(raised);
    }
}
