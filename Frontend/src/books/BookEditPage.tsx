import { useState, type SubmitEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Link, useNavigate, useParams } from "react-router-dom";
import { ApiError } from "../api";
import { getBook, updateBook } from "./booksApi";
import type { UpdateBookRequest } from "./types";

function readBookId(value: string | undefined) {
    const bookId = Number(value);
    return Number.isInteger(bookId) && bookId > 0 ? bookId : null;
}

//---------------------------------------------------------------------------

export function BookEditPage() {
    const { bookId: bookIdParameter } = useParams(); // hands back booknumber from URL as a string
    const bookId = readBookId(bookIdParameter);      // for instance /books/:bookId -> /book/42
    const [formError, setFormError] = useState<string | null>(null); // Localstate for validation messages
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const bookQuery = useQuery({
        queryKey: ["books", "detail", bookId], // reuse cached book in "books"||"detail" instead of refetching
        queryFn: () => {
            if (bookId === null) throw new Error("Invalid book id");  // check if book is null
            return getBook(bookId);                                   // If not, give back bookId
        },
        enabled: bookId !== null,
        retry: false,
    });

    const updateMutation = useMutation({ // Updates book using updateBook api 
        mutationFn: (request: UpdateBookRequest) => {
            if (bookId === null) throw new Error("Invalid book id");
            return updateBook(bookId, request);
        },
        onSuccess: async () => {  // On succes, tell bookslist "books" cache is outdated to refresh
            await queryClient.invalidateQueries({ queryKey: ["books"] });  // invalidate ---^
            navigate(`/books/${bookId}`);  // navigate to the newly created book page
        },
    });

    //---------------------------------------------------------------------------

    function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
        event.preventDefault();  // prevents reload of page so there's no state loss
        setFormError(null);      // clears out old error message before trying again.

        if (!bookQuery.data) return;

        const formData = new FormData(event.currentTarget);
        const title = formData.get("title")?.toString().trim() ?? "";     // get input value title
        const author = formData.get("author")?.toString().trim() ?? "";   // get input value author
        const yearValue = formData.get("year")?.toString().trim() ?? "";  // get input value year
        const year = Number(yearValue);                                   // convert year to int

        // Checks If anything's missing or year isn't a whole number, stop with error message
        if (!title || !author || !yearValue || !Number.isInteger(year)) {
            setFormError("Enter a title, author and valid year.");
            return;
        }

        // set these fields from submit for update mutation
        updateMutation.mutate({ title, author, year, version: bookQuery.data.version });
    }

    //---------------------------------------------------------------------------

    // Used to load Latest version in case someone else changed the book while you were editing.
    async function reloadLatest() {
        updateMutation.reset();
        await bookQuery.refetch();
    }

    //---------------------------------------------------------------------------

    if (bookId === null) {
        return (
            <main>
                <h1>Invalid book id</h1>
                <Link to="/books">Back to books</Link>
            </main>
        );
    }

    if (bookQuery.isPending) return <p>Loading book...</p>;

    const queryNotFound = bookQuery.error instanceof ApiError && bookQuery.error.status === 404;

    if (queryNotFound) {
        return (
            <main>
                <h1>Book not found</h1>
                <Link to="/books">Back to books</Link>
            </main>
        );
    }

    if (bookQuery.isError) return <p>Could not load the book.</p>;

    const book = bookQuery.data;
    const mutationStatus =
        updateMutation.error instanceof ApiError
            ? updateMutation.error.status
            : null;

    return (
        <main>
            
            <h1>Edit {book.title}</h1>
            <form key={book.version} onSubmit={handleSubmit}>
                <label>
                    Title
                    <input
                        name="title"
                        defaultValue={book.title}
                        maxLength={100}
                        required
                    />
                </label>

                <label>
                    Author
                    <input
                        name="author"
                        defaultValue={book.author}
                        maxLength={100}
                        required
                    />
                </label>

                <label>
                    Year
                    <input
                        name="year"
                        type="number"
                        defaultValue={book.year}
                        required
                    />
                </label>

                <button type="submit" disabled={updateMutation.isPending}>
                    {updateMutation.isPending ? "Saving..." : "Save changes"}
                </button>
                <Link to={`/books/${book.id}`}>Cancel</Link>
            </form>

            {formError && <p>{formError}</p>}
            {mutationStatus === 400 && <p>The API rejected the book data.</p>}
            {mutationStatus === 401 && <p>Your login is missing or expired.</p>}
            {mutationStatus === 403 && (
                <p>Only administrators can edit books.</p>
            )}
            {mutationStatus === 404 && <p>This book no longer exists.</p>}
            {mutationStatus === 409 && (
                <div>
                    <p>
                        This book was changed by another user. Your changes were not saved.
                    </p>
                    <button type="button" onClick={reloadLatest}>
                        Load latest version
                    </button>
                </div>
            )}
            {updateMutation.isError && mutationStatus === null && (
                <p>Could not update the book.</p>
            )}
        </main>
    );
}