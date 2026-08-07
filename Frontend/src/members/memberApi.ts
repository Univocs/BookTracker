import { apiRequest } from "../api";
import type { RegisterMemberResponse, RegisterMemberRequest } from "./types";

export function registerMember(request: RegisterMemberRequest) {
    return apiRequest<RegisterMemberResponse>("/members", {
        method: "POST",
        body: JSON.stringify(request)
    })
}