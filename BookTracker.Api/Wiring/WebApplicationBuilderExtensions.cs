using BookTracker.Api.Application;
using BookTracker.Api.Storage.Books;
using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;
using BookTracker.Api.Storage.Members;
using Microsoft.AspNetCore.Identity;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Wiring;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddBookTracker(this WebApplicationBuilder builder)
    {
        RegisterStorage(builder);           // Register database + repository
        RegisterHandlers(builder.Services); /* Register every class that uses IHandler. 
                                                  (in order to register handlers)       */
        return builder;
    }

    private static void RegisterStorage(WebApplicationBuilder builder) // builder for WebApplication
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("BookTracker")));
        // Register AppDbContext with SQLite, using the connection string from config
        builder.Services.AddScoped<IBookRepository, EfBookRepository>();
        builder.Services.AddScoped<IMemberRepository, EfMemberRepository>();
        builder.Services.AddScoped<IPasswordHasher<Member>, PasswordHasher<Member>>();
        // Scoped = fresh AppDbContext + EfBookRepository per request
        // thrown away after -> no cross-contamination between users
    }

    private static void RegisterHandlers(IServiceCollection services)
    {
        var handlerTypes = HandlerMarker.Assembly // Finds project BookTracker.Api where type IHandler is in.
            .GetTypes()         // Lists EVERY single class, interface, every type that exists in that project.      
            .Where(IsHandler);  // Where classes use IHandler.

        foreach (var type in handlerTypes)
        {
            services.AddScoped(type);  // Register every type selected in handlerTypes.
        }
    }

    private static bool IsHandler(Type type) // Checks if classes && if they use IHandler or not. 
    {
        return type is { IsClass: true, IsAbstract: false }// True if class and not abstract so instance possible
            && type.IsAssignableTo(HandlerMarker);         // Checks classes if they implement IHandler
    }

    private static readonly Type HandlerMarker = typeof(IHandler);
    // 1) Give information about type (interface) Ihandler = "HandlerMarker"
}