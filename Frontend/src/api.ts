import { getAccessToken } from "./auth/tokenStorage";

// Vite exposes env vars prefixed with VITE_ to client-side code at build time
const apiUrl = import.meta.env.VITE_API_URL;

// Custom error type so callers can check the HTTP status code, not just a message
// ApiError enhirates from TypeScript built-in Error class --> like C# : exception
export class ApiError extends Error {
    status: number;

    constructor(status: number, message: string) {
        super(message); // sets message to the base Error class
        this.status = status;
    }
}

//----------------------------------------------------------------------------------

// Do Backed Api request and return T -- else throw ApiError if invalid request
export async function apiRequest<T>(
  path: string, // Path of the endpoint --> "/auth/login"
  options: RequestInit = {}, //RequestInit has everything you pass to fetch (method, body, headers...)
): Promise<T> { // return of value T when fetched
  
  const headers = new Headers(options.headers); // header options of HTTP requests
  const token = getAccessToken(); // Fetches the stored token from localStorage

  headers.set("Accept", "application/json"); // Tells server we accept JSON back

  if (options.body) { // if we send HTTP body back
    headers.set("Content-Type", "application/json"); // declare content type as JSON
  }

  if (token) { // if we have a token
    headers.set("Authorization", `Bearer ${token}`); // declare authorization token
  }
/*    So basically this:

      POST http://localhost:5015/books
      Content-Type: application/json
      Authorization: Bearer ${token}     */
      
  const response = await fetch(`${apiUrl}${path}`, {
    ...options,   // method, body, etc. from the caller
    headers,      // our merged/overridden headers
  });

  // response.ok is true only for status 200–299, so we throw manually here.
  if (!response.ok) {
    throw new ApiError(response.status, `Request failed with status ${response.status}`);
  }

  return response.json() as Promise<T>;
}