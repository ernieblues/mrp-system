export interface User {
    id: string;
    email: string;
    roles: string[];
}

/**
 * Login with email + password.
 * If successful, backend sets the cookie/session,
 * then we fetch the current user.
 */
export async function login(email: string, password: string): Promise<User> {
    const response = await fetch("/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error("Login failed");
    }

    const user = await getCurrentUser();
    if (!user) {
        throw new Error("Login succeeded but user data is missing");
    }

    return user;
}

/**
 * Log out the current user.
 */
export async function logout(): Promise<void> {
    const response = await fetch("/api/auth/logout", {
        method: "POST",
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error("Logout failed");
    }
}

/**
 * Get the currently authenticated user from the backend.
 * Returns null if not logged in.
 */
export async function getCurrentUser(): Promise<User | null> {
    const response = await fetch("/api/auth/me", {
        credentials: "include",
    });

    if (!response.ok) {
        return null;
    }

    const data: User = await response.json();
    return data ?? null;
}

// Register a new user with email + password.
export async function register(values: Record<string, string>) {
    const response = await fetch("/api/auth/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(values),
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData?.[0]?.description || "Registration failed");
    }

    return await response.json();
}
