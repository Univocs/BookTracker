import { useState, type SubmitEvent } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { login } from "./authApi";
import { setAccessToken } from "./tokenStorage";

// React Component
export function LoginPage() { // USESTATE UPDATE

    // useState re-renders when the value changes and UI updates
    const [email, setEmail] = useState("");       // function to set email
    const [password, setPassword] = useState(""); // function to set password
    const navigate = useNavigate(); // navigate("/endpoint") without browser reload
    const queryClient = useQueryClient(); // Tanst. Query's cached server data

    // mutations change server data --> like POST /auth/login
    const loginMutation = useMutation({
        mutationFn: login,                   // perform login and reveive a token (ID)
        onSuccess: async (response) => {
            setAccessToken(response.accessToken); // saves token to localStorage
            await queryClient.invalidateQueries({ queryKey: ["current-member"] });
            navigate("/account");
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
