namespace BookTracker.Api.Tests.IntegrationTests;

// "abstract" = Can never be instantiated directly (no "new IntegrationTest()").
// It only exists to be inherited from, it has no tests of its own.
public abstract class IntegrationTest : IDisposable
{
  private readonly CustomWebApplicationFactory factory = new();

  // "protected" = visible to this class and any subclass, but not to outside code.
  // "{ get; }" = get-only property. Test class can READ Client, but no set (no "Client = something;").
  protected HttpClient Client { get; }
  protected EfReader Reader { get; }
  protected EfWriter Writer { get; }

  // Build all three once, right when the test starts, using the factory.
  protected IntegrationTest()
  {
    Client = factory.CreateClient();
    Reader = factory.GetReader();
    Writer = factory.GetWriter();
  }

  // Required by IDisposable. xUnit automatically calls this after each test
  public void Dispose()
  {
    // Clean up the HttpClient first.
    Client.Dispose();

    // Then dispose the factory itself, the open SQLite in-memory connection.
    factory.Dispose();
  }
}