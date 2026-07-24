using BookTracker.Api.Domain.Members;
using BookTracker.Api.Security;
using BookTracker.Api.Storage;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Seeding;

// The code that actually puts the fake books into the database, only when it makes sense
public static class DatabaseSeeder
{
  //count = 50 is a default parameter value, if no SeedBooks(dbContext) without count, defaults 50. 
  // You can still override it: SeedBooks(dbContext, 200).
  public static void SeedBooks(AppDbContext dbContext, int count = 50)
  {
    // Any() checks: "does the Books table already have at least one row?" 
    // If yes, bail out immediately — do nothing. This is a guard against duplicate seeding
    // you don't want to re-run and keep adding 50 books every time the app starts. It seeds an empty database.
    if (dbContext.Books.Any())
    {
      return;
    }

    // Generate count books, using the fuzzer.
    var books = BookFuzzr.Many(count);

    // Same pattern as EfWriter.Seed, queue all the generated books with AddRange, 
    // then commit them to the database in one go with SaveChanges().
    dbContext.Books.AddRange(books);
    dbContext.SaveChanges();
  }

  public static void SeedAdministrator(AppDbContext dbContext,
                                       IConfiguration configuration,
                                       IPasswordHasher<Member> passwordHasher)
  {
    // Load the admin settings (name, email, password) from config
    var settings = configuration.GetSection(DevelopmentAdminSettings.SectionName)
                         .Get<DevelopmentAdminSettings>();

    // No settings or no password configured? Don't seed an admin.
    if (settings is null || string.IsNullOrWhiteSpace(settings.Password)) return;

    var email = new MemberEmail(settings.Email);

    // Already seeded before? Don't create a second admin.
    var exists = dbContext.Members.Any(member => (string)member.Email == email.Value);
    if (exists) return;

    // Build the admin manually (not through the normal registration flow)
    var administrator = new Member
    {
      Name = new MemberName(settings.Name),
      Email = email, // settings.email
      PasswordHash = string.Empty,  // Filled in below
      Role = MemberRole.Administrator
    };

    // Turn the plain-text password from config into a proper hash
    administrator.PasswordHash = passwordHasher.HashPassword(
                                                administrator,
                                                settings.Password);
    // Save the new admin to the database                                            
    dbContext.Members.Add(administrator);
    dbContext.SaveChanges();
  }
}