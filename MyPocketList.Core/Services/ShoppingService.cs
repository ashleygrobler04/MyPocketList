using MyPocketList.Core.Enums;
using MyPocketList.Core.Models;
using MyPocketList.Core.Results;

namespace MyPocketList.Core.Services;

public class ShoppingService
{
    private ShoppingList _shoppingList;

    public ShoppingService(ShoppingList list)
    {
        _shoppingList = list;
    }

    public void SetShoppingList(ShoppingList list)
    {
        _shoppingList = list;
    }

    public Result<ShoppingItem> AddItem(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<ShoppingItem>.Fail("Item name cannot be empty.");
        }

        if (_shoppingList.Items.Any(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            return Result<ShoppingItem>.Fail("Item with the same name already exists.");
        }

        var newItem = new ShoppingItem
        {
            Id = Guid.NewGuid(),
            Name = name,
            State = ItemState.Default,
            Shopped = false
        };

        _shoppingList.Items.Add(newItem);
        return Result<ShoppingItem>.Ok(newItem);
    }

    public Result RemoveItem(Guid itemId)
    {
        var item = _shoppingList.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            return Result.Fail("Item not found.");
        }

        _shoppingList.Items.Remove(item);
        return Result.Ok();
    }

    public Result Shop(Guid itemId)
    {
        var item = _shoppingList.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            return Result.Fail("Item not found");
        }
        if (item.State == ItemState.Shopped)
        {
            return Result.Fail("Item is already shopped.");
        }
        else if (item.State == ItemState.InBasket)
        {
            return Result.Fail("ITem is in basket. Please remove from basket.");
        }
        if (item.State == ItemState.Default)
        {
            item.State = ItemState.Shopped;
        }
        return Result.Ok();
    }

    public Result<ShoppingList> GetShoppedItems()
    {
        var items = _shoppingList.Items.Where(x => x.State == ItemState.Shopped);
        if (!items.Any())
        {
            return Result<ShoppingList>.Fail("No shopped items found.");
        }
        
        var lst = new ShoppingList();
        foreach (var i in items)
        {
            lst.Items.Add(new ShoppingItem()
            {
                Name = i.Name,
                Id = i.Id,
                State = i.State,
                Shopped = i.State == ItemState.Shopped
            });
        }

        return Result<ShoppingList>.Ok(lst);
    }
}