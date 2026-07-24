using BookTracker.Api.Domain.Members;
using BookTracker.Api.Endpoints.Auth;
using BookTracker.Api.Endpoints.Books;
using BookTracker.Api.Endpoints.Members;
using BookTracker.Api.Seeding;
using BookTracker.Api.Storage;
using Microsoft.AspNetCore.Identity;

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
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Member>>();

            dbContext.Database.EnsureCreated();

            if (app.Configuration.GetValue<bool>("SeedDatabase"))
            {
                DatabaseSeeder.SeedBooks(dbContext, 500);
                DatabaseSeeder.SeedAdministrator(dbContext, app.Configuration, passwordHasher);
            }
        }

        app.UseAuthentication(); // runs Authentication
        app.UseAuthorization();  // then Authorizes

        app.MapAuthEndpoints();   // maps Authorization endpoints
        app.MapBookEndpoints();   // maps Book endpoints
        app.MapMemberEndpoints(); // maps Member endpoints

        return app;
    }
}