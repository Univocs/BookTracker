import { Navigate } from "react-router-dom";
import { ApiError } from "../api";
import { getAccessToken } from "./tokenStorage";
import { useCurrentMember } from "./useCurrentMember";

export function AccountPage() {          // Page checks if you have a token (ID) - no login again
  const currentMemberQuery = useCurrentMember();

  if (!getAccessToken()) {                    // no token at all in storage?
    return <Navigate to="/login" replace />;  // navigated directly to /account? -> redirect to /login
  }

  if (currentMemberQuery.isPending) {           // request is in flight! 
    return <p>Loading account...</p>;
  }

  const unauthorized =
    currentMemberQuery.error instanceof ApiError &&
    currentMemberQuery.error.status === 401;

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