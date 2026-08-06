
// books types

export type BookSummary = {
    id: number
    title: string;
    author: string;
}

export type GetBooksRequest = {
    page: number;
    pageSize: number;
    search: string;
}

export type BookDetails = {
    id: number
    title: string;
    author: string;
    year: number;
    version: string;
}

export type CreateBookRequest = {
  title: string;
  author: string;
  year: number;
};

export type CreateBookResponse = {
  id: number;
  title: string;
  author: string;
  year: number;
};

export type UpdateBookRequest = {
  title: string;
  author: string;
  year: number;
  version: string;
};