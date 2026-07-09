using BookTracker.Api.Domain;
using QuickFuzzr;

namespace BookTracker.Api.Seeding;

public static class BookFuzzr
{
  public static IEnumerable<Book> Many(int count)
      => One.Many(count).Generate();

  // BookFuzzr.Many(5) gives you 50 randomly generated Book objects. 
  // Everything else in the file is private plumbing that builds up to this.

  private static readonly string[] Adjectives =
  [
      "Suspicious",
        "Melancholy",
        "Quantum",
        "Reluctant",
        "Extremely Polite",
        "Mildly Haunted",
        "Unreasonably Confident",
        "Invisible",
        "Chronically Late",
        "Over-Caffeinated"
  ];

  // These are just a string list for example called "Nouns"

  private static readonly string[] Nouns =
  [
      "Badger",
        "Librarian",
        "Spaceship",
        "Cupcake",
        "Philosopher",
        "Typewriter",
        "Goblin",
        "Umbrella",
        "Database",
        "Octopus"
  ];

  private static readonly string[] Situations =
  [
      "Who Knew Too Much",
        "At the End of Time",
        "With a Suspicious Hat",
        "In Production",
        "Under the Stairs",
        "Against Better Judgement",
        "During Standup",
        "With No Unit Tests",
        "On a Tuesday",
        "After the Refactor"
  ];

  private static readonly string[] FirstNames =
  [
      "Ada",
        "Grace",
        "Douglas",
        "Ursula",
        "Terry",
        "Octavia",
        "Isaac",
        "Mary",
        "Kurt",
        "Agatha"
  ];

  private static readonly string[] LastNames =
  [
      "Byte",
        "Stackwell",
        "Nullman",
        "Loopington",
        "Brackets",
        "Mergefield",
        "Bugworthy",
        "Semicolon",
        "Heap",
        "Async"
  ];

  private static readonly FuzzrOf<string> Situational =
      from adjective in Fuzzr.OneOf(Adjectives)
      from noun in Fuzzr.OneOf(Nouns)
      from situation in Fuzzr.OneOf(Situations)
      select $"The {adjective} {noun} {situation}";

  private static readonly FuzzrOf<string> Memoir =
      from adjective in Fuzzr.OneOf(Adjectives)
      from noun in Fuzzr.OneOf(Nouns)
      select $"My Life as an {adjective} {noun}";

  private static readonly FuzzrOf<string> Academic =
      from adjective in Fuzzr.OneOf(Adjectives)
      from noun in Fuzzr.OneOf(Nouns)
      select $"A Brief History of {adjective} {noun}s";

  // "Title" uses the random generated strings above using the string lists
  // So (Situational, Memoir, Academic) go into title. It chooses one of them
  private static readonly FuzzrOf<string> Title =
      Fuzzr.OneOf(Situational, Memoir, Academic);

  // "Author" uses the FirstNames and LastNames lists to create author string
  private static readonly FuzzrOf<string> Author =
      from firstName in Fuzzr.OneOf(FirstNames)
      from lastName in Fuzzr.OneOf(LastNames)
      select $"{firstName} {lastName}";

  // "One" generates a random book with random title, author and year.
  // This is used by => One.Many(count).Generate(); above! 
  private static readonly FuzzrOf<Book> One =
      from title in Title
      from author in Author
      from year in Fuzzr.Int(1930, 2026)
      select new Book
      {
        Title = new BookTitle(title),
        Author = new AuthorName(author),
        Year = year
      };
}