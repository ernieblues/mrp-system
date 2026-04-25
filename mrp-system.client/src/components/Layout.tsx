import { ClipboardList, Home, Sun, User as UserIcon } from "lucide-react";
import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";

import { useAuth } from "../hooks/useAuth";

export default function Layout({
    title,
    children,
}: {
    title: string;
    children: React.ReactNode;
}) {
    const { currentUser, logoutUser, loading } = useAuth();
    const navigate = useNavigate();

    const [menuOpen, setMenuOpen] = useState(false);
    const [sidebarOpen, setSidebarOpen] = useState(false);
    const [themeOpen, setThemeOpen] = useState(false);

    const handleLogout = async () => {
        await logoutUser();
        navigate("/login");
    };

    function setTheme(theme: string) {
        document.documentElement.setAttribute("data-theme", theme);
        setThemeOpen(false);
    }

    useEffect(() => {
        setMenuOpen(false);
    }, [currentUser]);

    if (loading) {
        return (
            <div className="flex items-center justify-center h-screen bg-base-100 text-base-content">
                Loading...
            </div>
        );
    }

    return (
        <div className="flex text-base-content">
            {/* Sidebar */}
            <div
                className={`flex flex-col justify-between bg-base-300 text-base-content transition-all duration-300
                ${sidebarOpen ? "w-28" : "w-10"}`}
            >
                {/* Sidebar: top */}
                <div>
                    {/* Hamburger menu */}
                    <button
                        onClick={() => setSidebarOpen(!sidebarOpen)}
                        className="flex items-center h-12 w-full px-2 hover:bg-base-200 rounded"
                    >
                        <div className="space-y-1">
                            <div className="w-5 h-0.5 bg-base-content"></div>
                            <div className="w-5 h-0.5 bg-base-content"></div>
                            <div className="w-5 h-0.5 bg-base-content"></div>
                        </div>
                        <span
                            className={`ml-2 whitespace-nowrap overflow-hidden transition-opacity duration-200 ${sidebarOpen ? "opacity-100" : "opacity-0"
                                }`}
                        >
                            Menu
                        </span>
                    </button>

                    {/* Navigation links */}
                    <ul className="mt-2 space-y-1">
                        <li>
                            {/* Home */}
                            <Link
                                to="/"
                                className="flex items-center h-12 px-2 hover:bg-base-200 rounded"
                            >
                                <Home className="w-5 h-5 flex-shrink-0" />
                                <span
                                    className={`ml-2 whitespace-nowrap overflow-hidden transition-opacity duration-200 ${sidebarOpen ? "opacity-100" : "opacity-0"
                                        }`}
                                >
                                    Home
                                </span>
                            </Link>
                        </li>
                        <li>
                            {/* Purchase Requisition */}
                            <Link
                                to="/purchase-requisitions/1"
                                className="flex items-center h-12 px-2 hover:bg-base-200 rounded"
                            >
                                <ClipboardList className="w-5 h-5 flex-shrink-0" />
                                <span
                                    className={`ml-2 whitespace-nowrap overflow-hidden transition-opacity duration-200 ${sidebarOpen ? "opacity-100" : "opacity-0"
                                        }`}
                                >
                                    PRs
                                </span>
                            </Link>
                        </li>
                    </ul>
                </div>

                {/* Sidebar: bottom */}
                <div className="relative">

                    {/* Theme selector */}
                    <button
                        onClick={() => setThemeOpen(!themeOpen)}
                        className="flex items-center h-12 w-full px-2 hover:bg-base-200"
                    >
                        <Sun className="w-6 h-6 flex-shrink-0" />
                        <span
                            className={`ml-2 whitespace-nowrap overflow-hidden transition-opacity duration-200 ${sidebarOpen ? "opacity-100" : "opacity-0"
                                }`}
                        >
                            Theme
                        </span>
                    </button>

                    {themeOpen && (
                        <div className="absolute left-full bottom-12 mb-2 ml-2 w-40 bg-base-100 text-base-content rounded-lg shadow-lg py-2 z-10">
                            <button
                                onClick={() => setTheme("light")}
                                className="block w-full text-left px-4 py-2 text-sm hover:bg-base-200"
                            >
                                Light
                            </button>
                            <button
                                onClick={() => setTheme("dark")}
                                className="block w-full text-left px-4 py-2 text-sm hover:bg-base-200"
                            >
                                Dark
                            </button>
                            <button
                                onClick={() => setTheme("night")}
                                className="block w-full text-left px-4 py-2 text-sm hover:bg-base-200"
                            >
                                Night
                            </button>
                            <button
                                onClick={() => setTheme("corporate")}
                                className="block w-full text-left px-4 py-2 text-sm hover:bg-base-200"
                            >
                                Corporate
                            </button>
                            <button
                                onClick={() => setTheme("cupcake")}
                                className="block w-full text-left px-4 py-2 text-sm hover:bg-base-200"
                            >
                                Cupcake
                            </button>
                        </div>
                    )}

                    {/* User */}
                    {currentUser ? (
                        <>
                            <button
                                onClick={() => setMenuOpen(!menuOpen)}
                                className="flex items-center h-12 w-full px-2 hover:bg-base-200"
                            >
                                <UserIcon className="w-6 h-6 flex-shrink-0" />
                                <span
                                    className={`ml-2 whitespace-nowrap overflow-hidden transition-opacity duration-200 ${sidebarOpen ? "opacity-100" : "opacity-0"
                                        }`}
                                >
                                    Account
                                </span>
                            </button>

                            {menuOpen && (
                                <div className="absolute left-full bottom-0 mb-2 ml-2 w-56 bg-base-100 text-base-content rounded-lg shadow-lg py-2 z-10">
                                    <div className="px-4 py-2 text-sm font-medium border-b border-base-300">
                                        {currentUser.email}
                                    </div>
                                    <button
                                        onClick={handleLogout}
                                        className="block w-full text-left px-4 py-2 text-sm hover:bg-base-200"
                                    >
                                        Logout
                                    </button>
                                </div>
                            )}
                        </>
                    ) : (
                        <Link
                            to="/login"
                            className="flex items-center h-12 px-2 hover:bg-base-200"
                        >
                            <UserIcon className="w-6 h-6 flex-shrink-0" />
                            <span
                                    className={`ml-2 whitespace-nowrap overflow-hidden transition-opacity duration-200 ${sidebarOpen ? "opacity-100" : "opacity-0"
                                    }`}
                            >
                                Login
                            </span>
                        </Link>
                    )}
                </div>
            </div>
            
            {/* Main content */}
            <div className="flex-1 flex flex-col text-base-content h-screen overflow-auto">
                {/* Title bar */}
                <div className="bg-primary/15 shadow px-4 py-1 items-center">
                    <h1 className="text-lg font-semibold">{title}</h1>
                </div>

                {/* Content area */}
                <div className="flex-1 flex flex-col bg-base-100">{children}</div>
            </div>
        </div>
    );
}
