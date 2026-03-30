import { createContext } from "react";

import type { User } from "@/services/authServices";

// This interface defines the shape of the authentication data
export interface AuthContextType {
    currentUser: User | null; // the logged-in user, or null if none
    loading: boolean;         // true while we check the login status
    loginUser: (email: string, password: string) => Promise<void>;
    logoutUser: () => Promise<void>;
    refreshUser: () => Promise<void>;
    hasRole: (role: string) => boolean; // helper to check if user has a role
}

// This is the context object that will hold our authentication data
export const AuthContext = createContext<AuthContextType | undefined>(undefined);
