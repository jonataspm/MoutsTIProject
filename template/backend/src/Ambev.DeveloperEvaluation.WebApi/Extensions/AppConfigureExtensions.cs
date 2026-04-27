using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Extensions;
public static class AppConfigureExtensions
{
    public static void ApplyMigration(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                Log.Information("Applicate Migrations");
                var context = services.GetRequiredService<DefaultContext>();
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
                Log.Information("Migration Applied");
            }
            catch (Exception ex)
            {
                Log.Information($"Migration Error: {JsonSerializer.Serialize(ex)}");
            }
        }
    }
}
