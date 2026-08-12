using Microsoft.JSInterop;

namespace MyPocketList.Services;

public interface IInfoService
{
    Task Alert(string message);
    Task<bool> Confirm(string message);
}

public class InfoService : IInfoService
{
    private readonly IJSRuntime js;

    public InfoService(IJSRuntime js)
    {
        this.js = js;
    }

    public async Task Alert(string message)
    {
        await js.InvokeVoidAsync("alert", message);
    }

    public async Task<bool> Confirm(string message)
    {
        return await js.InvokeAsync<bool>("confirm", message);
    }
}