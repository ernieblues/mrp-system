import { CheckCircle, XCircle, AlertCircle } from "lucide-react";
import React from "react";
import { Link, useLocation } from "react-router-dom";

export default function Notification() {
    const location = useLocation();
    const { icon, message, color, link, linkText } = location.state || {};

    const icons: { [key: string]: React.ReactNode } = {
        check: <CheckCircle className="w-6 h-6" />,
        x: <XCircle className="w-6 h-6" />,
        alert: <AlertCircle className="w-6 h-6" />,
    };

    return (
        <div className="p-8">
            <div className={`flex items-center gap-2 mb-4 text-${color}-600`}>
                {icons[icon]}
                <h1 className="text-xl font-semibold">{message || "No message"}</h1>
            </div>

            {link && (
                <Link
                    to={link}
                    className="inline-block mt-2 px-3 py-1 text-sm bg-gray-200 text-gray-800 rounded hover:bg-gray-300"
                >
                    {linkText || "Continue"}
                </Link>
            )}
        </div>
    );
}
