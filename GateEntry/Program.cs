using GateEntry.Extensions;
var builder = WebApplication.CreateBuilder(args);

var settings = builder.ConfigureAppSettings();

builder.Services.AddCustomServices()
    .AddCustomAuthentication(builder.Configuration)
    .AddCustomAuthorization()
    .AddStaticFiles()
    .ConfigureCors()
    .ConfigureOpenApi()
    .AddControllers();

var app = builder.Build();

app.MapOpenApi();
app.SetupApp();


//app.UseSpaStaticFiles();





/*
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "wwwroot"; // Not used at runtime, but required

    // Optional: If running a dev server like Vite or npm:
    // spa.UseProxyToSpaDevelopmentServer("http://localhost:5173");
});
*/

app.Run();
