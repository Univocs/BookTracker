const tokenKey = "book-tracker-access-token";

// Localstorage saves token when page reloads 

export function getAccessToken() {  
    return localStorage.getItem(tokenKey);
} // Get access token from storage

export function setAccessToken(token: string) {
    localStorage.setItem(tokenKey, token);
} // Set access token in storage

export function removeAccessToken() {
    localStorage.removeItem(tokenKey);
} // Remove access token from storage