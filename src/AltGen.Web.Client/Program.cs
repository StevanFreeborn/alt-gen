using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AltGen.Shared.Generate;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// TODO: Read base url
// from configuration
builder.Services.AddHttpClient<IAltGenService, AltGenService>(c => c.BaseAddress = new("https://localhost:7297"));

await builder.Build().RunAsync();
