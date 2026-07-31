using MyPocketList.Core.Enums;
using MyPocketList.Core.Models;
using MyPocketList.Core.Services;

namespace CoreTests;

public class ShoppingServiceTests
{
    [Fact]
    public void AddItem_WhenNameIsValid_AddsNewItemToList()
    {
        var list = new ShoppingList();
        var service = new ShoppingService(list);

        var result = service.AddItem("Milk");

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal("Milk", result.Value.Name);
        Assert.Equal(ItemState.Default, result.Value.State);
        Assert.False(result.Value.Shopped);
        Assert.Single(list.Items);
        Assert.Equal(result.Value.Id, list.Items[0].Id);
    }

    [Fact]
    public void AddItem_WhenNameIsEmpty_ReturnsFailure()
    {
        var list = new ShoppingList();
        var service = new ShoppingService(list);

        var result = service.AddItem("   ");

        Assert.False(result.Succeeded);
        Assert.Equal("Item name cannot be empty.", result.Message);
        Assert.Empty(list.Items);
    }

    [Fact]
    public void AddItem_WhenDuplicateNameExists_ReturnsFailure()
    {
        var list = new ShoppingList();
        var service = new ShoppingService(list);
        service.AddItem("Milk");

        var result = service.AddItem("milk");

        Assert.False(result.Succeeded);
        Assert.Equal("Item with the same name already exists.", result.Message);
        Assert.Single(list.Items);
    }

    [Fact]
    public void Shop_WhenItemIsDefault_MarksItemAsShopped()
    {
        var list = new ShoppingList();
        var service = new ShoppingService(list);
        var item = service.AddItem("Milk").Value!;

        var result = service.Shop(item.Id);

        Assert.True(result.Succeeded);
        Assert.Equal(ItemState.Shopped, list.Items.Single().State);
    }

    [Fact]
    public void Shop_WhenItemIsAlreadyShopped_ReturnsFailure()
    {
        var list = new ShoppingList();
        var service = new ShoppingService(list);
        var item = service.AddItem("Milk").Value!;
        service.Shop(item.Id);

        var result = service.Shop(item.Id);

        Assert.False(result.Succeeded);
        Assert.Equal("Item is already shopped.", result.Message);
    }

    [Fact]
    public void Shop_WhenItemIsInBasket_ReturnsFailure()
    {
        var list = new ShoppingList();
        var service = new ShoppingService(list);
        var item = service.AddItem("Milk").Value!;
        item.State = ItemState.InBasket;

        var result = service.Shop(item.Id);

        Assert.False(result.Succeeded);
        Assert.Equal("ITem is in basket. Please remove from basket.", result.Message);
    }

    [Fact]
    public void RemoveItem_WhenItemExists_RemovesItFromList()
    {
        var list = new ShoppingList();
        var service = new ShoppingService(list);
        var item = service.AddItem("Milk").Value!;

        var result = service.RemoveItem(item.Id);

        Assert.True(result.Succeeded);
        Assert.Empty(list.Items);
    }

    [Fact]
    public void RemoveItem_WhenItemDoesNotExist_ReturnsFailure()
    {
        var list = new ShoppingList();
        var service = new ShoppingService(list);

        var result = service.RemoveItem(Guid.NewGuid());

        Assert.False(result.Succeeded);
        Assert.Equal("Item not found.", result.Message);
    }

    [Fact]
    public void RemoveFromBasket_WhenItemIsInBasket_MovesItToShopped()
    {
        var list = new ShoppingList();
        var service = new ShoppingService(list);
        var item = service.AddItem("Milk").Value!;
        item.State = ItemState.InBasket;

        var result = service.RemoveFromBasket(item.Id);

        Assert.True(result.Succeeded);
        Assert.Equal(ItemState.Shopped, item.State);
    }

    [Fact]
    public void RemoveFromBasket_WhenItemIsNotInBasket_ReturnsFailure()
    {
        var list = new ShoppingList();
        var service = new ShoppingService(list);
        var item = service.AddItem("Milk").Value!;

        var result = service.RemoveFromBasket(item.Id);

        Assert.False(result.Succeeded);
        Assert.Equal("Item is not in basket.", result.Message);
    }

    [Fact]
    public void GetShoppedItems_WhenShoppedItemsExist_ReturnsOnlyShoppedItems()
    {
        var list = new ShoppingList();
        var service = new ShoppingService(list);
        var milk = service.AddItem("Milk").Value!;
        var bread = service.AddItem("Bread").Value!;
        service.Shop(milk.Id);

        var result = service.GetShoppedItems();

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Items);
        Assert.Equal("Milk", result.Value.Items[0].Name);
        Assert.Equal(ItemState.Shopped, result.Value.Items[0].State);
        Assert.True(result.Value.Items[0].Shopped);
    }

    [Fact]
    public void GetShoppedItems_WhenNoShoppedItemsExist_ReturnsFailure()
    {
        var list = new ShoppingList();
        var service = new ShoppingService(list);
        service.AddItem("Milk");

        var result = service.GetShoppedItems();

        Assert.False(result.Succeeded);
        Assert.Equal("No shopped items found.", result.Message);
    }

    [Fact]
    public void Update_WhenItemExists_UpdatesItemDetails()
    {
        var list = new ShoppingList();
        var service = new ShoppingService(list);
        var item = service.AddItem("Milk").Value!;
        var updatedItem = new ShoppingItem
        {
            Name = "Cheese",
            State = ItemState.InBasket
        };

        var result = service.Update(item.Id, updatedItem);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal("Cheese", item.Name);
        Assert.Equal(ItemState.InBasket, item.State);
        Assert.Equal(item.Id, result.Value.Id);
    }

    [Fact]
    public void SetShoppingList_ReplacesTheActiveList()
    {
        var originalList = new ShoppingList();
        var updatedList = new ShoppingList();
        var service = new ShoppingService(originalList);
        service.AddItem("Milk");

        service.SetShoppingList(updatedList);
        var addResult = service.AddItem("Bread");

        Assert.True(addResult.Succeeded);
        Assert.Single(updatedList.Items);
        Assert.Single(originalList.Items);
        Assert.Equal("Milk", originalList.Items[0].Name);
    }
}
