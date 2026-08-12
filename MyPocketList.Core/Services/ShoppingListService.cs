using MyPocketList.Core.Models;
using MyPocketList.Core.Persistence;
using MyPocketList.Core.Results;

namespace MyPocketList.Core.Services;

public interface IShoppingListService
{
    IReadOnlyList<ShoppingList> Lists { get; }
    Task InitializeAsync();
    Task<Result<ShoppingList>> CreateListAsync(string name);
    Task<Result<ShoppingList>> RenameListAsync(Guid id, string newName);
    Task<Result> DeleteListAsync(Guid id);
    Task SaveAsync();
}

public class ShoppingListService : IShoppingListService
{
    private readonly IListStore _store;
    private List<ShoppingList> _lists = new();
    private bool _initialized;

    public ShoppingListService(IListStore store)
    {
        _store = store;
    }

    public IReadOnlyList<ShoppingList> Lists => _lists;

    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        _lists = await _store.LoadAsync();
        _initialized = true;
    }

    public async Task<Result<ShoppingList>> CreateListAsync(string name)
    {
        var trimmed = name?.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return Result<ShoppingList>.Fail("List name cannot be empty.");
        }

        if (_lists.Any(l => l.Name.Equals(trimmed, StringComparison.OrdinalIgnoreCase)))
        {
            return Result<ShoppingList>.Fail("A list with the same name already exists.");
        }

        var list = new ShoppingList
        {
            Id = Guid.NewGuid(),
            Name = trimmed
        };

        _lists.Add(list);
        await SaveAsync();
        return Result<ShoppingList>.Ok(list);
    }

    public async Task<Result<ShoppingList>> RenameListAsync(Guid id, string newName)
    {
        var trimmed = newName?.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return Result<ShoppingList>.Fail("List name cannot be empty.");
        }

        var list = _lists.FirstOrDefault(l => l.Id == id);
        if (list == null)
        {
            return Result<ShoppingList>.Fail("List not found.");
        }

        if (_lists.Any(l => l.Id != id && l.Name.Equals(trimmed, StringComparison.OrdinalIgnoreCase)))
        {
            return Result<ShoppingList>.Fail("A list with the same name already exists.");
        }

        list.Name = trimmed;
        await SaveAsync();
        return Result<ShoppingList>.Ok(list);
    }

    public async Task<Result> DeleteListAsync(Guid id)
    {
        var list = _lists.FirstOrDefault(l => l.Id == id);
        if (list == null)
        {
            return Result.Fail("List not found.");
        }

        _lists.Remove(list);
        await SaveAsync();
        return Result.Ok();
    }

    public async Task SaveAsync()
    {
        await _store.SaveAsync(_lists);
    }
}
