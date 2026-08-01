import { useEffect } from "react";
import { useQuery } from "@tanstack/react-query";
import { Navigate } from "react-router-dom";
import { ApiError } from "../api";
import { getCurrentMember } from "./authApi";
import { getAccessToken, removeAccessToken } from "./tokenStorage";

export function AccountPage() {          // Page checks if you have a token (ID) - no login again
  const currentMemberQuery = useQuery({  // useQuery to fetch current member 
    queryKey: ["current-member"],        // Cached key stored under "current-member"
    queryFn: getCurrentMember,  
    enabled: getAccessToken() !== null,  // Id acces token not null -> run it
    retry: false,                        // Retry turned off, because waste of effort
  });

  const unauthorized =
    currentMemberQuery.error instanceof ApiError &&
    currentMemberQuery.error.status === 401;

  useEffect(() => {                 // useEffect -> not rendering, outside rendering (localstorage)
    if (unauthorized) {             // if token unauthorized
      removeAccessToken();          // remove it from storage.
    }}, [unauthorized]);            // [] re-run effect when unauthorized changes value between renders

  if (!getAccessToken()) {                    // no token at all in storage?
    return <Navigate to="/login" replace />;  // navigated directly to /account? -> redirect to /login
  }

  if (currentMemberQuery.isPending) {           // request is in flight! 
    return <p>Loading account...</p>;
  }

  if (unauthorized) {                           // no token at all in storage?
    return <Navigate to="/login" replace />;    // Redirect to /login
  }

  if (currentMemberQuery.isError) {     // something went wrong for not any other specific error case
    return <p>Could not load the account.</p>;
  }

  const member = currentMemberQuery.data; // none of the above return conditions matched, return member

  return (
    <main>
      <h1>{member.name}</h1>
      <p>{member.email}</p>
      <p>Role: {member.role}</p>
    </main>
  );
}