namespace BookTracker.Api.Storage.Books;

public enum UpdateBookResult
{
    Updated,  // Book has been updated
    NotFound, // Book was not found
    Conflict // Book has been updated by someone else
}