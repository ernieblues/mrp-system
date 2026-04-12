import type { User } from "@/types/commonTypes";

export async function getUsers(): Promise<User[]> {
    const response = await fetch("/api/users/lookup", {
        credentials: "include", // so auth cookie/session flows
    });

    if (!response.ok) {
        throw new Error("Failed to fetch users");
    }

    return response.json();
}
