using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using MyPocketList;
using MyPocketList.Core.Models;
using MyPocketList.Core.Persistence;
using MyPocketList.Core.Services;
using MyPocketList.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddFluentUIComponents();
builder.Services.AddScoped<IInfoService, InfoService>();
builder.Services.AddScoped<IListStore, ListStore>();
builder.Services.AddScoped<IShoppingListService, ShoppingListService>();
builder.Services.AddScoped<IShoppingService>(_ => new ShoppingService(new ShoppingList()));
await builder.Build().RunAsync();
