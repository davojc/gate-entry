namespace GateEntry.Extensions;

public static class ConfigurationExtensions
{
    public static Settings ConfigureAppSettings(this WebApplicationBuilder builder)
    {
        var settings = new Settings();
        builder.Configuration.GetSection("Settings").Bind(settings);

        // Use the settings in Kestrel
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(settings.Host.Port);
        });

        builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));

        return settings;
    }
}