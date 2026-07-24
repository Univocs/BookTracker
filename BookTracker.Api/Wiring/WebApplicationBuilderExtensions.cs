using BookTracker.Api.Application;
using BookTracker.Api.Storage.Books;
using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;
using BookTracker.Api.Storage.Members;
using Microsoft.AspNetCore.Identity;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Security;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace BookTracker.Api.Wiring;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddBookTracker(this WebApplicationBuilder builder)
    {
        RegisterStorage(builder);           // Register database + repository
        RegisterHandlers(builder.Services); /* Register every class that uses IHandler. 
                                                  (in order to register handlers)       */
        RegisterAuthentication(builder);
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

    private static void RegisterAuthentication(WebApplicationBuilder builder)
    {
        var settings = builder.Configuration // ASP.NET's unified config system
                      .GetRequiredSection(JwtSettings.SectionName)
                      .Get<JwtSettings>()
                      ?? throw new InvalidOperationException("JWT settings are missing.");

        if (string.IsNullOrWhiteSpace(settings.SigningKey))
        {
            throw new InvalidOperationException("JWT signing key is missing.");
        }

        // Registering the already-built settings object, so build once
        builder.Services.AddSingleton(settings);
        // JwtTokenGenerator manual registration because LoginCommandHandler will depend on it.
        builder.Services.AddScoped<JwtTokenGenerator>();

        builder.Services
        // this app has a concept of 'logged in,' and here's the default method for checking it.
       .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options => // configures the actual rules how to validate an incoming JWT
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer, // reject tokens not issued by "BookTracker"

                    ValidateAudience = true,
                    ValidAudience = settings.Audience, // reject tokens not meant for "BookTracker"

                    ValidateLifetime = true, // reject tokens where expiresAt has passed

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(settings.SigningKey)),
                    // checks if the token was tampered with or signed elsewhere, if so FAILS
                    // Has to be same key + same conversion as JwtTokenGenerator

                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role,

                    ClockSkew = TimeSpan.Zero // Clock at zero so time is exact with lifetime
                };
        });

        builder.Services.AddAuthorization();
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