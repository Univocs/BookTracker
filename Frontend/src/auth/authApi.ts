import { apiRequest } from "../api";
import type { CurrentMember, LoginRequest, LoginResponse } from "./types";

// return accesToken by LoginResponse by sending HTTP request "LoginRequest"
export function login(request: LoginRequest) {
    return apiRequest<LoginResponse>("/auth/login",
        {
            method: "POST",
            body: JSON.stringify(request),
        });
}

// Just GET CurrenMember data from /auth/me endpoint
export function getCurrentMember() {
    return apiRequest<CurrentMember>("/auth/me");
}