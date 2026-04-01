using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AegisSec.Client;
using AegisSec.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => 
{
    var config = sp.GetRequiredService<IConfiguration>();
    var apiBaseUrl = config["ApiBaseUrl"] ?? "http://localhost:5000";
    return new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
});
builder.Services.AddScoped<SecurityService>();

await builder.Build().RunAsync();
