namespace MyPocketList.Core.Models;

public class ShoppingList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ShoppingItem> Items { get; set; } = new List<ShoppingItem>();
}