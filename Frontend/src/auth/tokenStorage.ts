// PROBLEM: NO LOGGED-IN AWARE NAVIGATION
/* 
    What's the actual problem we're solving?

      - Show register / Log in" → when nobody's logged in.
      - Show Account / Log out" → when somebody is.

    React doesn't know setAccesToken() writes into localStorage, it's not a React State.
    It does not re-render, so we need to let it know to pay attention! 
  */

import { useSyncExternalStore } from "react";

// SOLUTION: BUILD NOTIFICATION SYSTEM

const tokenKey = "book-tracker-access-token";

// Collection Functions() stored in this set to call later
const listeners = new Set<() => void>(); 

// Notifies each listener in "listeners" through setAccessToken && removeAccessToken.                                   
function notifyListeners() {
    listeners.forEach((listener) => listener());
}

// Called once per component that wants to receive token-change notifications.
// listener is the function to call whenever that happens
function subscribe(listener: () => void) {
    // adds function to our listeners list so notifyListeners loops through it
    listeners.add(listener); 

    // Fires on other browser tabs when localStorage changes
    // checks if the changed key is the access token when user logs in/out in different tab
    function handleStorage(event: StorageEvent) { 
        // Is the changed data specifically our token key? Call listener
        if (event.key === tokenKey) listener();  // StorageEvents.key === changed key
    }

    // tells browser when localStorage changed in another tab, by calling "handleStorage"
    window.addEventListener("storage", handleStorage);

    // Cleanup: remove this listener and stop watching for other-tab changes
    return () => {
        listeners.delete(listener);
        window.removeEventListener("storage", handleStorage);
    }; // If we don't do this: memory leak -> dead listeners in memory
}

// Get access token from storage
export function getAccessToken() {
    return localStorage.getItem(tokenKey);
} 

// Get current accessToken, and automatically re-render it's changes — whether that change happened in this tab or another one.
export function useAccessToken() {
  return useSyncExternalStore(subscribe, getAccessToken, () => null);
}

// Set access token in storage and notify listeners
export function setAccessToken(token: string) {
    localStorage.setItem(tokenKey, token);
    notifyListeners();
} 

// Remove access token from storage and notify listeners
export function removeAccessToken() {
    localStorage.removeItem(tokenKey);
    notifyListeners();
} 


