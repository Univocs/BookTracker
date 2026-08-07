import { useState, type SubmitEvent } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useLocation, useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { login } from "./authApi";
import { setAccessToken } from "./tokenStorage";

type LoginLocationState = {  // Received state data when member registered
    registered?: boolean;
    email?: string;
}

// React Component
export function LoginPage() { // USESTATE UPDATE

    // useLocation() -> object with everything of the current URL
    // Includes any invisible "state" data that was attached when navigating here.
    // In your login page, you're using it to read and pre-fill the email & show "just registered".
    const location = useLocation();
    const locationState = location.state as LoginLocationState | null;

    // IF STATE  -> Use "email" from RegisterPage.
    // IF NOT    -> Fall back to empty string.
    const [email, setEmail] = useState(locationState?.email ?? "");

    // useState re-renders when the value changes and UI updates
    const [password, setPassword] = useState(""); // function to set password
    const navigate = useNavigate(); // navigate("/endpoint") without browser reload
    const queryClient = useQueryClient(); // Tanst. Query's cached server data

    // mutations change server data --> like POST /auth/login
    const loginMutation = useMutation({
        mutationFn: login,                   // perform login and reveive a token (ID)
        onSuccess: async (response) => {
            setAccessToken(response.accessToken); // saves token to localStorage
            await queryClient.invalidateQueries({ queryKey: ["current-member"] });
            navigate("/account", { replace: true });
        },
    });

    function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
        event.preventDefault(); // stops HTML form's default behavior -> a full-page navigation/reload
        loginMutation.mutate({ email, password }); // The actual trigger
    }

    const invalidCredentials =
        loginMutation.error instanceof ApiError && loginMutation.error.status === 401;

    return (
        <main>
            {locationState?.registered && (
                <p>Your account was created. You can now log in.</p>
            )}
            <h1>Log in</h1>
            <form onSubmit={handleSubmit}>
                <label>
                    Email
                    <input
                        type="email"
                        value={email}
                        onChange={(event) => setEmail(event.target.value)}
                        autoComplete="email"
                        required
                    />
                </label>
                <label>
                    Password
                    <input
                        type="password"
                        value={password}
                        onChange={(event) => setPassword(event.target.value)}
                        autoComplete="current-password"
                        required
                    />
                </label>
                {/* 
                        This button triggers type=submit 
                        When Login.isPending, then "logging in...", else "log in"
                    */}
                <button type="submit" disabled={loginMutation.isPending}>
                    {loginMutation.isPending ? "Logging in..." : "Log in"}
                </button>
                {/* 
                        && -> if left expr. TRUE -->  returns right side, else false return nothing
                        if invalid credentials -> return "email or password is incorrect."
                    */}
                {invalidCredentials && <p>Email or password is incorrect.</p>}
                {/* 
                        Did the login go wrong AND not specifically wrong credentials THEN <p>...</p>
                    */}
                {loginMutation.isError && !invalidCredentials && (
                    <p>Login failed. Is the API running?</p>
                )}
            </form>
        </main>
    );
}
