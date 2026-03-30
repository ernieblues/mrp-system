import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";

import { Form, type Field } from "@/components/Form";
import { register } from "@/services/authService";

export default function Register() {
    const [error, setError] = useState("");
    const navigate = useNavigate();

    const schema: Field[] = [
        {
            name: "email",
            label: "Email",
            type: "email",
            required: true,
        },
        {
            name: "password",
            label: "Password",
            type: "password",
            required: true,
        },
        {
            name: "confirmPassword",
            label: "Confirm Password",
            type: "password",
            required: true,
        },
    ];

    const handleSubmit = async (values: Record<string, string>) => {
        try {
            const result = await register(values);
            navigate("/notification", {
                state: {
                    icon: "check",
                    message: result.message || "Registration successful",
                    link: "/login",
                    linkText: "Go to Login",
                },
            });
        } catch (err) {
            if (err instanceof Error) {
                setError(err.message);
            } else {
                setError("Something went wrong.");
            }
        }
    };

    return (
        <div className="w-full flex justify-center pt-20">
            <div className="w-full max-w-md">
                {error && <div className="mb-4 text-red-600">{error}</div>}
                <Form schema={schema} onSubmit={handleSubmit} />
                <p className="mt-6 text-sm text-gray-600">
                    {"Already have an account? "}
                    <Link to="/login" className="text-blue-600 hover:underline">
                        Login here
                    </Link>
                </p>
            </div>
        </div>
    );
}
