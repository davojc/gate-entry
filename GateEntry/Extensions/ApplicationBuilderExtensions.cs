namespace GateEntry.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication SetupApp(this WebApplication app)
    {
        /*
        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        */

        //app.UseHttpsRedirection();

        app.UseCors("CorsPolicy");

        // Enable default file serving (e.g., index.html)
        app.UseDefaultFiles();
        // Enable static file serving from wwwroot
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        

        return app;
    }
}