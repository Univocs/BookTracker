/*  rather than copy pasting this in several places,
    we make one reusable hook called useCurrentMember() that does:

        - block non-admins members from book/new
        - Hide "Add book" link from non-admins members
        - Every component using useCurrentMember() hook, refers "current-member" cached data
*/

import { useQuery } from "@tanstack/react-query";
import { getCurrentMember } from "./authApi";
import { getAccessToken, removeAccessToken } from "./tokenStorage";
import { ApiError } from "../api";
import { useEffect } from "react";

export function useCurrentMember() {
    const query = useQuery({          // useQuery to fetch current member 
        queryKey: ["current-member"], // Lets multiple components share same cached key
        queryFn: getCurrentMember,
        enabled: getAccessToken() !== null, // queryFn disables if this is false, else run it
        retry: false,                       // Retry turned off, because waste of effort
    });

    // Checking if the error is a 401 - unauthorized - invalid accessToken
    const unauthorized = query.error instanceof ApiError && query.error.status === 401;

    // useEffect -> () => {function}, [dependancy array]
    // When something in the dependancy array [unauthorized] changes, run the function! 
    // A consequence of rendering rather then running it during rendering.
    useEffect(() => {
        if (unauthorized) removeAccessToken(); // remove from storage.
    }, [unauthorized]);   // if unauthorized flips true, run the function

    return query;
}