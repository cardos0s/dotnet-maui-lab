using TaskApp.Core.Models;
using TaskApp.Core.Services;
using TaskApp.Core.ViewModels;
using Xunit;

namespace TaskApp.Core.Tests;

/// <summary>
/// Testes da tela de edição. Mostram como testar uma ViewModel que carrega por
/// parâmetro, salva e navega de volta — tudo sem abrir o app, usando o fake de
/// navegação pra verificar o "voltar".
/// </summary>
public class TaskEditViewModelTests
{
    private static async Task<(TaskEditViewModel vm, ITaskRepository repo, FakeNavigationService nav, Guid id)>
        SetupWithOneTask(string title = "Original")
    {
        var repo = new InMemoryTaskRepository();
        var item = new TaskItem { Title = title };
        await repo.AddAsync(item);
        var nav = new FakeNavigationService();
        var vm = new TaskEditViewModel(repo, nav);
        return (vm, repo, nav, item.Id);
    }

    [Fact]
    public async Task Load_FillsFieldsFromTheTask()
    {
        var (vm, _, _, id) = await SetupWithOneTask("Trocar o óleo");

        await vm.LoadAsync(id);

        Assert.Equal("Trocar o óleo", vm.Title);
        Assert.False(vm.IsCompleted);
    }

    [Fact]
    public async Task Save_PersistsChanges_AndNavigatesBack()
    {
        var (vm, repo, nav, id) = await SetupWithOneTask();
        await vm.LoadAsync(id);

        vm.Title = "Título editado";
        vm.IsCompleted = true;
        await vm.SaveCommand.ExecuteAsync(null);

        var saved = (await repo.GetAllAsync()).Single();
        Assert.Equal("Título editado", saved.Title);
        Assert.True(saved.IsCompleted);
        Assert.Equal(1, nav.GoBackCount);          // voltou pra tela anterior
    }

    [Fact]
    public async Task Save_CannotExecute_WhenTitleIsBlank()
    {
        var (vm, _, _, id) = await SetupWithOneTask();
        await vm.LoadAsync(id);

        vm.Title = "   ";
        Assert.False(vm.SaveCommand.CanExecute(null));

        vm.Title = "Válido";
        Assert.True(vm.SaveCommand.CanExecute(null));
    }

    [Fact]
    public async Task Cancel_NavigatesBack_WithoutSaving()
    {
        var (vm, repo, nav, id) = await SetupWithOneTask("Não muda");
        await vm.LoadAsync(id);

        vm.Title = "Mudei mas vou cancelar";
        await vm.CancelCommand.ExecuteAsync(null);

        var unchanged = (await repo.GetAllAsync()).Single();
        Assert.Equal("Não muda", unchanged.Title);   // não persistiu
        Assert.Equal(1, nav.GoBackCount);
    }
}
