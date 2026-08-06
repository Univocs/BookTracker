import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useCurrentMember } from "../auth/useCurrentMember";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { deleteBook } from "./booksApi";
import { ApiError } from "../api";


// Component receives title so you can do: "Are you sure you want to delete 'Dune'?"
type DeleteBookButtonProps = {
    bookId: number;
    title: string;
}

export function BookDeleteButton({ bookId, title }: DeleteBookButtonProps) {
    const [confirming, setConfirming] = useState(false);
    const currentMemberQuery = useCurrentMember();  // to check if admin or not
    const queryClient = useQueryClient();           // Access to cache
    const navigate = useNavigate();                 // Redirection after succes

    // HELPER FUNCTION => After book deletion, clean up cached data, go back to booklist! 
    function leaveDeletedBook() {
        // Marks "books" as old, stale  --  don't go fetching it right away
        queryClient.invalidateQueries({ queryKey: ["books"], refetchType: "none" });
        queryClient.removeQueries({ // Book will be gone, so data should be gone too! 
            queryKey: ["books", "detail", bookId],
            exact: true,            // Only detele entry that matches the key exactly! 
        });
        navigate("/books");         // Send user back to book list! 
    }

    const deleteMutation = useMutation({
        mutationFn: () => deleteBook(bookId),  // Delete book through bookId
        onSuccess: leaveDeletedBook,           // then clean data and back to booklist
    });

    if (
        !currentMemberQuery.isSuccess ||
        currentMemberQuery.data.role !== "Administrator"
    ) {
        return null;
    }

    if (!confirming) {
        return (
            <button type="button" onClick={() => setConfirming(true)}>
                Delete book
            </button>
        );
    }

    const mutationStatus =
        deleteMutation.error instanceof ApiError
            ? deleteMutation.error.status
            : null;

    return (
        <section aria-labelledby="delete-book-heading">
            <h2 id="delete-book-heading">Delete {title}?</h2>
            <p>This action cannot be undone.</p>

            <button
                type="button"
                onClick={() => deleteMutation.mutate()}
                disabled={deleteMutation.isPending}
            >
                {deleteMutation.isPending ? "Deleting..." : "Yes, delete book"}
            </button>{" "}

            <button
                type="button"
                onClick={() => {
                    deleteMutation.reset();
                    setConfirming(false);
                }}
                disabled={deleteMutation.isPending}
            >
                Cancel
            </button>

            {mutationStatus === 401 && <p>Your login is missing or expired.</p>}
            {mutationStatus === 403 && (
                <p>Only administrators can delete books.</p>
            )}
            {mutationStatus === 404 && (
                <div>
                    <p>This book no longer exists. It may already have been deleted.</p>
                    <button type="button" onClick={leaveDeletedBook}>
                        Back to books
                    </button>
                </div>
            )}
            {deleteMutation.isError && mutationStatus === null && (
                <p>Could not delete the book.</p>
            )}
        </section>
    );
}