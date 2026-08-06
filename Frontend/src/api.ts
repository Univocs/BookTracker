import { getAccessToken } from "./auth/tokenStorage";

// Read the API's base URL from an environment variable (e.g. "http://localhost:5015")
const apiUrl = import.meta.env.VITE_API_URL;

// Custom error type from TypeScript built-in Error class --> like C# : exception
// Callers can check the HTTP status code - not just receive the error message
export class ApiError extends Error {
  status: number;

  constructor(status: number, message: string) {
    super(message);        // sets message to the base Error class
    this.status = status;  // Stores the status code on this instance
  }
}

//----------------------------------------------------------------------------------

// HELPER FUNCTION FOR REQUESTS —> "build the request" — "send it" — "check if fails"
async function sendRequest(path: string, options: RequestInit) {
  const headers = new Headers(options.headers); // header options of HTTP requests
  const token = getAccessToken();               // Fetches the stored token from localStorage

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


  // Actually send the request. "awaits" for the response to arrive.
  const response = await fetch(`${apiUrl}${path}`, {
    ...options,   // method, body, etc. from the caller
    headers,      // our merged/overridden headers
  });

  // response.ok is true only for status 200–299, so we throw manually here.
  if (!response.ok) {
    throw new ApiError(response.status, `Request failed with status ${response.status}`);
  }

  return response; // return the raw response if everything succeeded
}

//----------------------------------------------------------------------------------

// FUNCTION REQUEST 1 --> for requests where you EXPECT a JSON body back.

export async function apiRequest<T>(
  path: string,              // Path of the endpoint --> "/auth/login"
  options: RequestInit = {}, // RequestInit has everything you pass to fetch (method, body, headers...)
): Promise<T> {              // return of value T when fetched

  const response = await sendRequest(path, options);
  // take whatever the server sent back, and turn it from raw text into a usable JavaScript object.
  return response.json() as Promise<T>; // Expects to return JSON and promise T
}

//----------------------------------------------------------------------------------

// FUNCTION REQUEST 2 --> for requests where you expect NOTHING back (like our 204 update).

export async function apiRequestWithoutResponse(
  path: string,
  options: RequestInit = {},
): Promise<void> {

  await sendRequest(path, options); // Does not expect a response body
}