using MyPocketList.Core.Models;
using MyPocketList.Core.Persistence;
using MyPocketList.Core.Services;

namespace CoreTests;

public class ShoppingListServiceTests
{
    private sealed class FakeListStore : IListStore
    {
        public List<ShoppingList> Stored { get; private set; } = new();

        public Task SaveAsync(IEnumerable<ShoppingList> lists)
        {
            Stored = lists.ToList();
            return Task.CompletedTask;
        }

        public Task<List<ShoppingList>> LoadAsync() => Task.FromResult(Stored.ToList());
    }

    private static ShoppingListService CreateService(FakeListStore store) =>
        new ShoppingListService(store);

    [Fact]
    public async Task InitializeAsync_WhenNothingStored_HasNoLists()
    {
        var store = new FakeListStore();
        var service = CreateService(store);

        await service.InitializeAsync();

        Assert.Empty(service.Lists);
    }

    [Fact]
    public async Task InitializeAsync_LoadsStoredLists()
    {
        var store = new FakeListStore();
        store.Stored.Add(new ShoppingList { Id = Guid.NewGuid(), Name = "Groceries" });
        var service = CreateService(store);

        await service.InitializeAsync();

        Assert.Single(service.Lists);
        Assert.Equal("Groceries", service.Lists[0].Name);
    }

    [Fact]
    public async Task CreateList_WhenNameIsValid_AddsAndPersistsList()
    {
        var store = new FakeListStore();
        var service = CreateService(store);

        var result = await service.CreateListAsync("Weekly groceries");

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal("Weekly groceries", result.Value.Name);
        Assert.Single(service.Lists);
        Assert.Equal(service.Lists[0].Id, result.Value.Id);
        Assert.Single(store.Stored);
    }

    [Fact]
    public async Task CreateList_WhenNameIsEmpty_ReturnsFailure()
    {
        var store = new FakeListStore();
        var service = CreateService(store);

        var result = await service.CreateListAsync("   ");

        Assert.False(result.Succeeded);
        Assert.Equal("List name cannot be empty.", result.Message);
        Assert.Empty(service.Lists);
    }

    [Fact]
    public async Task CreateList_WhenNameIsDuplicate_ReturnsFailure()
    {
        var store = new FakeListStore();
        var service = CreateService(store);
        await service.CreateListAsync("Groceries");

        var result = await service.CreateListAsync("groceries");

        Assert.False(result.Succeeded);
        Assert.Equal("A list with the same name already exists.", result.Message);
        Assert.Single(service.Lists);
    }

    [Fact]
    public async Task RenameList_WhenValid_RenamesList()
    {
        var store = new FakeListStore();
        var service = CreateService(store);
        var list = (await service.CreateListAsync("Groceries")).Value!;

        var result = await service.RenameListAsync(list.Id, "Weekly groceries");

        Assert.True(result.Succeeded);
        Assert.Equal("Weekly groceries", list.Name);
        Assert.Equal("Weekly groceries", service.Lists.Single().Name);
        Assert.Equal("Weekly groceries", store.Stored.Single().Name);
    }

    [Fact]
    public async Task RenameList_WhenNameIsEmpty_ReturnsFailure()
    {
        var store = new FakeListStore();
        var service = CreateService(store);
        var list = (await service.CreateListAsync("Groceries")).Value!;

        var result = await service.RenameListAsync(list.Id, "");

        Assert.False(result.Succeeded);
        Assert.Equal("List name cannot be empty.", result.Message);
        Assert.Equal("Groceries", list.Name);
    }

    [Fact]
    public async Task RenameList_WhenListDoesNotExist_ReturnsFailure()
    {
        var store = new FakeListStore();
        var service = CreateService(store);

        var result = await service.RenameListAsync(Guid.NewGuid(), "New name");

        Assert.False(result.Succeeded);
        Assert.Equal("List not found.", result.Message);
    }

    [Fact]
    public async Task RenameList_WhenNameIsDuplicate_ReturnsFailure()
    {
        var store = new FakeListStore();
        var service = CreateService(store);
        var first = (await service.CreateListAsync("First")).Value!;
        await service.CreateListAsync("Second");

        var result = await service.RenameListAsync(first.Id, "second");

        Assert.False(result.Succeeded);
        Assert.Equal("A list with the same name already exists.", result.Message);
        Assert.Equal("First", first.Name);
    }

    [Fact]
    public async Task DeleteList_WhenListExists_RemovesIt()
    {
        var store = new FakeListStore();
        var service = CreateService(store);
        var list = (await service.CreateListAsync("Groceries")).Value!;

        var result = await service.DeleteListAsync(list.Id);

        Assert.True(result.Succeeded);
        Assert.Empty(service.Lists);
        Assert.Empty(store.Stored);
    }

    [Fact]
    public async Task DeleteList_WhenListDoesNotExist_ReturnsFailure()
    {
        var store = new FakeListStore();
        var service = CreateService(store);

        var result = await service.DeleteListAsync(Guid.NewGuid());

        Assert.False(result.Succeeded);
        Assert.Equal("List not found.", result.Message);
    }

    [Fact]
    public async Task SaveAsync_OverwritesStoreWithCurrentLists()
    {
        var store = new FakeListStore();
        var service = CreateService(store);
        await service.CreateListAsync("Groceries");

        store.Stored.Add(new ShoppingList { Id = Guid.NewGuid(), Name = "Hardware" });
        await service.SaveAsync();

        Assert.Single(store.Stored);
        Assert.Equal("Groceries", store.Stored[0].Name);
    }
}
