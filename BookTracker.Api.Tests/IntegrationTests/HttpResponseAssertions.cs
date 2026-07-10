using System.Net;
using System.Text.Json;
using Xunit.Sdk;

namespace BookTracker.Api.Tests.IntegrationTests;

public static class HttpResponseAssertions // Static = no instances, belongs to class
{
  private static readonly JsonSerializerOptions JsonOptions =
      new(JsonSerializerDefaults.Web);
  // Configuration for JSON parsing => JsonSerializerDefaults.Web sets sensible parsing
  // (e.g. matching "title" in JSON to Title in C#, ignoring case)
  public static async Task<T> ReadJsonAs<T>(
        this HttpResponseMessage response, // "This" = dot-syntax, not parentheses / "HELLO".Shout, not Shout("Hello")
        HttpStatusCode expectedStatusCode) // This one stays inside the normal argument parentheses.
  {
    // Grab the body of the Http Response as string/text
    var body = await response.Content.ReadAsStringAsync();

    Assert.True( // If the status code does NOT equal the expected one, fail test and show $"message"
        response.StatusCode == expectedStatusCode, // if this is true, do nothing and pass.
                                                   // if not, fail test and show reason.
        $"Expected status code: {expectedStatusCode}, Actual status code: {response.StatusCode}, Response body: {body}");

    try // Attempt to translate Json body into object of type T. if body != valid JSON => throws a JsonException.
    {
      var result = JsonSerializer.Deserialize<T>(body, JsonOptions); // Json body => Value Object with JsonOptions

      Assert.NotNull(result);  // Double check if null

      return result; // If any line fails and gives JsonException, do "catch"
                     // If both lines are succes, return object. 

    }
    catch (JsonException exception) // Catch block only runs if something above threw a JsonException
    {
      throw new XunitException( // Throw a new clearer Xunit message that explains exactly what's wrong.
          $"""
                 Response had the expected status code, but could not be parsed as JSON.

                 Expected JSON type: 
                 {typeof(T).Name}

                 Response body:
                 {body}

                 JSON error:
                 {exception.Message}
                 """);
    } // "Expected type: BookDetails / Response: this is not json at all / error: 't' is an invalid start of a value.

  } // SUMMARY: Try translate the text to object => Fails? => New error => explains the type we wanted and the text we got.

  public static async Task ShouldHaveStatusCode(
      this HttpResponseMessage response,
      HttpStatusCode expectedStatusCode)
  {
    var body = await response.Content.ReadAsStringAsync();

    Assert.True(
        response.StatusCode == expectedStatusCode,
        $"""
             Expected status code:
             {expectedStatusCode}

             Actual status code:
             {response.StatusCode}

             Response body:
             {body}
             """);
  }

  /* 
  Expected status code:      ← what you WANTED to see
  {expectedStatusCode}

  Actual status code:        ← what you ACTUALLY got
  {response.StatusCode}

  Response body:             ← whatever the API sent, for extra clues
  {body}
  */
}