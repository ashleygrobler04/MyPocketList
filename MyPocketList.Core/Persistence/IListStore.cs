using MyPocketList.Core.Models;

namespace MyPocketList.Core.Persistence;

/// <summary>
/// Provides persistence of shopping lists to a backing store.
/// </summary>
public interface IListStore
{
    /// <summary>
    /// Persists the given shopping lists.
    /// </summary>
    /// <param name="lists">The shopping lists to save.</param>
    Task SaveAsync(IEnumerable<ShoppingList> lists);

    /// <summary>
    /// Loads the persisted shopping lists.
    /// </summary>
    /// <returns>The loaded lists, or an empty collection when nothing is stored.</returns>
    Task<List<ShoppingList>> LoadAsync();
}
