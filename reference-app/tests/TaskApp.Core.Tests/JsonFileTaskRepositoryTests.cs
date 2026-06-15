using TaskApp.Core.Models;
using TaskApp.Core.Services;
using TaskApp.Core.ViewModels;
using Xunit;

namespace TaskApp.Core.Tests;

/// <summary>
/// Testes de INTEGRAÇÃO do <see cref="JsonFileTaskRepository"/> — diferente dos
/// testes da ViewModel (que usam um fake), aqui a gente exercita a persistência
/// REAL contra um arquivo de verdade, num caminho temporário. É o nível "Integração"
/// da pirâmide de testes (ver guides/testing.md).
///
/// Implementa IDisposable pra limpar o arquivo no fim de cada teste.
/// </summary>
public sealed class JsonFileTaskRepositoryTests : IDisposable
{
    private readonly string _tempFile = Path.Combine(
        Path.GetTempPath(), $"taskapp-test-{Guid.NewGuid():N}.json");

    private JsonFileTaskRepository NewRepo() => new(_tempFile);

    [Fact]
    public async Task Add_ThenGet_PersistsToFile()
    {
        var repo = NewRepo();
        await repo.AddAsync(new TaskItem { Title = "Persistir isto" });

        var all = await repo.GetAllAsync();

        Assert.Single(all);
        Assert.Equal("Persistir isto", all[0].Title);
        Assert.True(File.Exists(_tempFile));   // escreveu no disco de verdade
    }

    [Fact]
    public async Task Data_SurvivesAcrossInstances()
    {
        // Uma instância escreve...
        await NewRepo().AddAsync(new TaskItem { Title = "Sobrevive ao restart" });

        // ...e uma instância NOVA (simulando reabrir o app) lê do mesmo arquivo.
        var outraInstancia = NewRepo();
        var all = await outraInstancia.GetAllAsync();

        Assert.Single(all);
        Assert.Equal("Sobrevive ao restart", all[0].Title);
    }

    [Fact]
    public async Task Update_ModifiesPersistedItem()
    {
        var repo = NewRepo();
        var item = new TaskItem { Title = "Antes" };
        await repo.AddAsync(item);

        item.IsCompleted = true;
        await repo.UpdateAsync(item);

        var reloaded = (await NewRepo().GetAllAsync()).Single();
        Assert.True(reloaded.IsCompleted);
    }

    [Fact]
    public async Task Delete_RemovesFromFile()
    {
        var repo = NewRepo();
        var item = new TaskItem { Title = "Temporário" };
        await repo.AddAsync(item);

        await repo.DeleteAsync(item.Id);

        Assert.Empty(await repo.GetAllAsync());
    }

    [Fact]
    public async Task ViewModel_WorksWithFileRepo_ExactlyLikeWithInMemory()
    {
        // A PROVA da arquitetura: a MESMA ViewModel, sem UMA linha de mudança,
        // funciona com a implementação de arquivo. Trocar persistência não afeta
        // a lógica de apresentação — é o que o ARCHITECTURE.md promete.
        var vm = new TaskListViewModel(NewRepo(), new FakeNavigationService());
        vm.NewTitle = "Tarefa via arquivo";

        await vm.AddTaskCommand.ExecuteAsync(null);
        await vm.LoadCommand.ExecuteAsync(null);

        Assert.Single(vm.Tasks);
        Assert.Equal("Tarefa via arquivo", vm.Tasks[0].Title);
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile)) File.Delete(_tempFile);
    }
}
