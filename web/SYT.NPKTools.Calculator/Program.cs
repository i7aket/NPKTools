using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SYT.NPKTools.Calculator;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// The whole calculation is local, so there is no HttpClient and no backend of any kind: the library
// is managed code with no dependencies, which is what lets the optimizer run in the browser.
builder.Services.AddSingleton<CalculatorModel>();

// The interface text, for every language at once — see Localisation/Translations.cs for why it is
// embedded rather than fetched.
builder.Services.AddSingleton<SYT.NPKTools.Calculator.Localisation.Translations>();

await builder.Build().RunAsync();
