import { Link, useSearchParams } from "react-router-dom";
import { getBooks } from "./booksApi";
import { keepPreviousData, useQuery } from "@tanstack/react-query";
import type { SubmitEvent } from "react";
import { CreateBookLink } from "./CreateBookLink";

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
    const booksQuery = useQuery({  // useQuery actually check if booklist is outdated to refetch
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

    const books = booksQuery.data;       // After both checks, data is garanteed (narrowing)

    return (
        <main>
            <h1>Books</h1>
            <CreateBookLink />

            <form key={search} onSubmit={handleSearch} /* When search key changes, new input overwrites empty */>
                <label                               /* for instance when you go back and old search is reset */>
                    Search by title or author
                    {" "}<input
                        type="search"         // Tells browser this is a search field.
                        name="search"         // This is what makes FormData work in handleSearch function.
                        defaultValue={search} // React sets value once, DOM manages it on every keystroke.
                    />
                </label>
                <button type="submit">Search</button>
            </form>

            {books.items.length === 0 ? (   // If ammount of books === 0, render "No books Found"
                <p>No books found.</p>
            ) : (                            // else render the list
                <ul /* unordened list */>
                    {books.items.map((book) => (
                        <li key={book.id} /* list item */ >
                            <Link to={`/books/${book.id}`} /* link to the bookdetails for specific book */>
                                <strong>{book.title}</strong> by {book.author}
                            </Link>
                        </li>
                    ))}
                </ul>
            )}
            <p /* Shows page information */ >
                Page {books.page} of {books.totalPages} - {books.totalItems} books found.
            </p>

            <button
                type="button"
                onClick={() => setPage(books.page - 1)}              // page - 1 goes back a page
                disabled={books.page <= 1 || booksQuery.isFetching}  // button disabled if <=1 or fetching
            >
                Previous
            </button>{" "}

            <button
                type="button"
                onClick={() => setPage(books.page + 1)}               // page + 1 goes up a page
                disabled={books.page >= books.totalPages || booksQuery.isFetching}
                                                                       /* button disabled >= totalpages or fetching */>
                Next
            </button>

            {booksQuery.isFetching && <p /* if fetching, show ---> */>Updating books...</p>}
        </main>
    );
}