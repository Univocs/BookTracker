import { apiRequest } from "../api";
import type { PagedResult } from "../types";
import type { BookDetails, BookSummary, GetBooksRequest } from "./types";

export function getBooks(request: GetBooksRequest) {
    const parameters = new URLSearchParams({ // Builds safe query strings
        page: request.page.toString(),
        pageSize: request.pageSize.toString(),
    });

    if (request.search) {
        parameters.set("search", request.search);
    }

    return apiRequest<PagedResult<BookSummary>>(
        `/books?${parameters.toString()}`
    );
}

// For instance: getBooks({ page: 1, pageSize: 10, search: "dune" });
//     builds -> GET /books?page=2&pageSize=10&search=dune

export function getBook(bookId: number){
    return apiRequest<BookDetails>(`/books/${bookId}`);
}