import { useContext } from "react";

import { AuthContext } from "@/context/authContext";

// This hook is the standard way to access the authentication context.
// Instead of importing useContext(AuthContext) everywhere, you just call useAuth().
export function useAuth() {
    // Get the context value from AuthContext
    const ctx = useContext(AuthContext);

    // If there is no provider above in the tree, throw an error
    // This prevents components from accidentally calling useAuth without AuthProvider
    if (!ctx) {
        throw new Error("useAuth must be used inside an AuthProvider");
    }

    // Return the authentication data and helper functions
    return ctx;
}
