import { useMutation } from "@tanstack/react-query";
import { useState, type SubmitEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { registerMember } from "./memberApi";
import { ApiError } from "../api";

export function RegisterPage() {
    const [formError, setFormError] = useState<string | null>(null); // Localstate for validation messages
    const navigate = useNavigate();

    const registerMutation = useMutation({
        mutationFn: registerMember,
        onSuccess: (member) => {
            navigate("/login", {         // IF REGISTERED -> navigate to "/login
                state: {                 // Also send this to "/login" so to greet the new member! 
                    registered: true,    // This person just finished registration
                    email: member.email  // the email they signed up with
                }
            });
        }
    });

    function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
        event.preventDefault(); // prevents reload of page so there's no state loss
        setFormError(null);     // clears out old error message before trying again.

        const formData = new FormData(event.currentTarget);            // GRAB DATA FROM SUBMIT!!

        const name = formData.get("name")?.toString().trim() ?? "";    // get input value name
        const email = formData.get("email")?.toString().trim() ?? "";  // get input value email
        const password = formData.get("password")?.toString() ?? "";   // get input value password
        const confirmPassword = formData.get("confirmPassword")?.toString() ?? ""; // write pass again

        if (!name || !email || !password) {
            setFormError("Name, email and password are required.");
            return;
        }

        if (password.length < 8) {
            setFormError("Password must contain at least 8 characters.");
            return;
        }

        if (password !== confirmPassword) {
            setFormError("Passwords do not match.");
            return;
        }

        registerMutation.mutate({ name, email, password });
    }

    const badRequest =
        registerMutation.error instanceof ApiError &&
        registerMutation.error.status === 400;

    const duplicateEmail =
        registerMutation.error instanceof ApiError &&
        registerMutation.error.status === 409;

    return (
        <main>
            <h1>Create account</h1>

            <form onSubmit={handleSubmit}>
                <label>
                    Name
                    <input
                        name="name"
                        autoComplete="name"
                        maxLength={100}
                        required
                    />
                </label>
                <label>
                    Email
                    <input
                        name="email"
                        type="email"
                        autoComplete="email"
                        maxLength={200}
                        required
                    />
                </label>
                <label>
                    Password
                    <input
                        name="password"
                        type="password"
                        autoComplete="new-password"
                        minLength={8}
                        required
                    />
                </label>
                <label>
                    Confirm password
                    <input
                        name="confirmPassword"
                        type="password"
                        autoComplete="new-password"
                        minLength={8}
                        required
                    />
                </label>

                <button type="submit" disabled={registerMutation.isPending}>
                    {registerMutation.isPending ? "Creating account..." : "Register"}
                </button>
            </form>

            {formError && <p>{formError}</p>}
            {badRequest && <p>The API rejected the registration data.</p>}
            {duplicateEmail && <p>An account with this email already exists.</p>}
            {registerMutation.isError && !badRequest && !duplicateEmail && (
                <p>Could not create the account. Is the API running?</p>
            )}

            <p>
                Already have an account? <Link to="/login">Log in</Link>
            </p>
        </main>
    );
}