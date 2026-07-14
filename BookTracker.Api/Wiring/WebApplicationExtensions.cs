using BookTracker.Api.Endpoints.Books;
using BookTracker.Api.Endpoints.Members;
using BookTracker.Api.Seeding;
using BookTracker.Api.Storage;

namespace BookTracker.Api.Wiring;

public static class WebApplicationExtensions
{
    public static WebApplication UseBookTracker(this WebApplication app)
    {
      // This ensures that if the database does not yet exist, it is automatically created.
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.EnsureCreated();

            if (app.Configuration.GetValue<bool>("SeedDatabase"))
            {
                DatabaseSeeder.SeedBooks(dbContext, 500);
            }
        }

        app.MapBookEndpoints();
        app.MapMemberEndpoints();
        // BookEndpoints => Where all the mappings are located for the api

        return app;
    }
}