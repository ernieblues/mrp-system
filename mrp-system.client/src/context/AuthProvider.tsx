import { useEffect, useState } from "react";

import { AuthContext } from "./authContext";
import { getCurrentUser, login, logout, type User } from "@/services/authService";

// This component wraps the app and provides authentication state
export function AuthProvider({ children }: { children: React.ReactNode }) {
    const [currentUser, setCurrentUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);

    // Check once on startup if a user is already logged in
    useEffect(() => {
        (async () => {
            const user = await getCurrentUser();
            setCurrentUser(user);
            setLoading(false);
        })();
    }, []);

    // Log in and update context
    const loginUser = async (email: string, password: string) => {
        const user = await login(email, password);
        setCurrentUser(user);
    };

    // Log out and clear context
    const logoutUser = async () => {
        await logout();
        setCurrentUser(null);
    };

    // Refresh the user from the server
    const refreshUser = async () => {
        const user = await getCurrentUser();
        setCurrentUser(user);
    };

    // Check if the current user has a specific role
    const hasRole = (role: string) => {
        return currentUser?.roles.includes(role) ?? false;
    };

    return (
        <AuthContext.Provider
            value={{ currentUser, loading, loginUser, logoutUser, refreshUser, hasRole }}
        >
            {children}
        </AuthContext.Provider>
    );
}
