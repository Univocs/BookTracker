import { useState, type SubmitEvent } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Link, useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { createBook } from "./booksApi";

export function BookCreatePage() {
    const [formError, setFormError] = useState<string | null>(null);
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const createMutation = useMutation({
        mutationFn: createBook,                 // The actual POST request
        onSuccess: async (book) => {            // book is data the API sent back! 

            // books list is now marked outdated so that BookListpage refetches through useQuery()
            await queryClient.invalidateQueries({ queryKey: ["books"] });
            navigate(`/books/${book.id}`); // navigate to the newly created book page
        },
    });

    // Submits when you hit Enter or click "Search"
    function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
        event.preventDefault(); // prevents reload of page so there's no state loss
        setFormError(null);     // clears out old error message before trying again.

        // event.currentTarget = the <form> element; FormData reads its field values at submit time
        const formData = new FormData(event.currentTarget);
        const title = formData.get("title")?.toString().trim() ?? "";    // get input value title
        const author = formData.get("author")?.toString().trim() ?? "";  // get input value author
        const yearValue = formData.get("year")?.toString().trim() ?? ""; // get input value year
        const year = Number(yearValue);                                  // convert year to int

        // Checks If anything's missing or year isn't a whole number, stop with error message
        if (!title || !author || !yearValue || !Number.isInteger(year)) {
            setFormError("Enter a title, author and valid year.");
            return;
        }

        createMutation.mutate({ title, author, year }); // set title, author, year from submit for mutation
    }

    const unauthorized =
        createMutation.error instanceof ApiError &&
        createMutation.error.status === 401;

    const forbidden =
        createMutation.error instanceof ApiError &&
        createMutation.error.status === 403;

    const badRequest =
        createMutation.error instanceof ApiError &&
        createMutation.error.status === 400;

    return (
        <main>
            <Link to="/books">Cancel</Link>
            <h1>Add book</h1>

            <form onSubmit={handleSubmit}>
                <label>
                    Title
                    <input name="title" maxLength={100} required />
                </label>

                <label>
                    Author
                    <input name="author" maxLength={100} required />
                </label>

                <label>
                    Year
                    <input name="year" type="number" required />
                </label>

                <button type="submit" disabled={createMutation.isPending}>
                    {createMutation.isPending ? "Saving..." : "Add book"}
                </button>
            </form>

            {formError && <p>{formError}</p>}
            {badRequest && <p>The API rejected the book data.</p>}
            {unauthorized && <p>Your login is missing or expired.</p>}
            {forbidden && <p>Only administrators can add books.</p>}
            {createMutation.isError &&
                !badRequest &&
                !unauthorized &&
                !forbidden && <p>Could not add the book.</p>}
        </main>
    );
}