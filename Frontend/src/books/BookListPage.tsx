import { useSearchParams } from "react-router-dom";
import { getBooks } from "./booksApi";
import { keepPreviousData, useQuery } from "@tanstack/react-query";
import type { SubmitEvent } from "react";

const pageSize = 10;

function readPage(value: string | null) {
    const page = Number(value);
    return Number.isInteger(page) && page > 0 ? page : 1;
    // Checks if page is integer and more then 0, else return 1.
}

//----------------------------------------------------------------------------

export function BookListPage() {
    const [searchParams, setSearchParams] = useSearchParams();
    const page = readPage(searchParams.get("page"));
    const search = searchParams.get("search")?.trim() ?? "";
    // if "search" param exists, return it trimmed; otherwise (missing) return ""


    // Get books and cache them in "books"
    const booksQuery = useQuery({
        queryKey: ["books", { page, pageSize, search }],
        queryFn: () => getBooks({ page, pageSize, search }),
        placeholderData: keepPreviousData, // Keeps 1st page until switch to 2nd
    });                                    // otherwise it shows "Loading.."

    // Change URL when clicking Previous/Next
    function setPage(nextPage: number) {
        const next = new URLSearchParams(searchParams); // make new copy of URL
        // Can't edit React state
        if (nextPage === 1) {     // if you go to page 1 
            next.delete("page"); // Keep URL clean /books - not /books?page=1
        } else {
            next.set("page", nextPage.toString()); // Set "page" to the desired page
        }
        setSearchParams(next);
    }

    // Submits when you hit Enter or click "Search"
    function handleSearch(event: SubmitEvent<HTMLFormElement>) {
        event.preventDefault(); // prevents reload of page so there's no state loss

        const next = new URLSearchParams;                   // new copy of URL
        // event.currentTarget = the <form> element; FormData reads its field values at submit time
        const formData = new FormData(event.currentTarget);
        const value = formData.get("search")?.toString().trim() ?? ""; // gets value of search input

        if (value) {                      // if value exists (not an empty string)
            next.set("search", value);    // set value as search input
        }
        setSearchParams(next);
    }

    if (booksQuery.isPending) {           // Early return: there's no data yet, first time loading
        return <p>Loading books...</p>;
    }

    if (booksQuery.isError) {             // Early return: fetch failed because API is not running?
        return <p>Could not load the books. Is the API running?</p>;
    }

    const result = booksQuery.data;       // After both checks, data is garanteed (narrowing)

    return (
        <main>
            <h1>Books</h1>

            <form key={search} onSubmit={handleSearch}>
                <label>
                    Search by title or author
                    <input
                        type="search"
                        name="search"
                        defaultValue={search}
                    />
                </label>
                <button type="submit">Search</button>
            </form>

            {result.items.length === 0 ? (
                <p>No books found.</p>
            ) : (
                <ul>
                    {result.items.map((book) => (
                        <li key={book.id}>
                            <strong>{book.title}</strong> by {book.author}
                        </li>
                    ))}
                </ul>
            )}

            <p>
                Page {result.page} of {result.totalPages}. {result.totalItems} books found.
            </p>

            <button
                type="button"
                onClick={() => setPage(result.page - 1)}
                disabled={result.page <= 1 || booksQuery.isFetching}
            >
                Previous
            </button>{" "}

            <button
                type="button"
                onClick={() => setPage(result.page + 1)}
                disabled={result.page >= result.totalPages || booksQuery.isFetching}
            >
                Next
            </button>

            {booksQuery.isFetching && <p>Updating books...</p>}
        </main>
    );
}