import { useQuery } from "@tanstack/react-query";
import { Link, useParams } from "react-router-dom";
import { getBook } from "./booksApi";
import { ApiError } from "../api";
import { EditBookLink } from "./EditBookLink";
import { BookDeleteButton } from "./BookDeleteButton";

function readBookId(value: string | undefined) {
    const bookId = Number(value);
    return Number.isInteger(bookId) && bookId > 0 ? bookId : null;
}

export function BookDetailsPage() {
    const { bookId: bookIdParameter } = useParams(); // hands back booknumber from URL as a string
    const bookId = readBookId(bookIdParameter); // for instance /books/:bookId -> /book/42

    const bookQuery = useQuery({
        queryKey: ["books", "detail", bookId], // reuse cached book in "books"||"detail" instead of refetching
        queryFn: () => {
            if (bookId === null) { throw new Error("invalid book id"); }
            return getBook(bookId);            // if it doesn't throw, return bookId
        },
        enabled: bookId !== null,              // don't run queryFn if this is false
        retry: false,                          // retrying a failure is a waste, retry turned off
    })

    if (bookId === null) {                      // if book does not exist, click "back to books"
        return (
            <main>
                <h1>Invalid book id</h1>
                <Link to="/books">Back to books</Link>
            </main>
        );
    }

    if (bookQuery.isPending) { return <p>Loading book...</p> }

    const querynotFound = bookQuery.error instanceof ApiError && bookQuery.error.status === 404;

    if (querynotFound) {     // This book genuinely doesn't exist - 404
        return (
            <main>
                <h1>Book not found</h1>
                <p>The requested book does not exist.</p>
                <Link to="/books">Back to books</Link>
            </main>
        );
    }

    if (bookQuery.isError) {     // Early return: fetch failed because API is not running?
        return (
            <main>
                <h1>Could not load the book</h1>
                <p>Is the API running?</p>
                <Link to="/books">Back to books</Link>
            </main>
        );
    }

    const book = bookQuery.data;

    return (
        <main>
            <Link to="/books">Back to books</Link>
            <h1>{book.title}</h1>
            <p>Author: {book.author}</p>
            <p>Year: {book.year}</p>
            <EditBookLink bookId={book.id} />
            <BookDeleteButton bookId={book.id} title={book.title} />
        </main>
    );
}