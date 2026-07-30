using MyPocketList.Core.Enums;

namespace MyPocketList.Core.Models;

public class ShoppingItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ItemState State { get; set; } = ItemState.Default;
    public bool Shopped { get; set; }
}
