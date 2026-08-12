using System.Text.Json;
using Microsoft.JSInterop;
using MyPocketList.Core.Models;
using MyPocketList.Core.Persistence;

namespace MyPocketList.Services;

public class ListStore : IListStore
{
    private const string StorageKey = "mypocketlist.lists";

    private readonly IJSRuntime _js;

    public ListStore(IJSRuntime js)
    {
        _js = js;
    }

    public async Task SaveAsync(IEnumerable<ShoppingList> lists)
    {
        var json = JsonSerializer.Serialize(lists);
        await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
    }

    public async Task<List<ShoppingList>> LoadAsync()
    {
        var json = await _js.InvokeAsync<string>("localStorage.getItem", StorageKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<ShoppingList>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<ShoppingList>>(json) ?? new List<ShoppingList>();
        }
        catch (JsonException)
        {
            return new List<ShoppingList>();
        }
    }
}
