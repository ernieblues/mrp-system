import { Link, useNavigate } from "react-router-dom";

import { Form, type Field } from "@/components/Form";
import { useAuth } from "@/hooks/useAuth";

export default function Login() {
    const navigate = useNavigate();
    const { loginUser } = useAuth();

    const schema: Field[] = [
        {
            name: "username",
            label: "Username",
            type: "email",
            required: true,
        },
        {
            name: "password",
            label: "Password",
            type: "password",
            required: true,
        },
    ];

    const handleSubmit = async (values: Record<string, string>) => {
        try {
            await loginUser(values.username, values.password);
            navigate("/", { replace: true });
        } catch (err) {
            alert(err instanceof Error ? err.message : "Login failed");
        }
    };

    return (
        <div className="w-full flex justify-center pt-20">
            <div className="w-full max-w-md">
                <Form schema={schema} onSubmit={handleSubmit} />
                <p className="mt-6 text-sm text-gray-600">
                    {"Don't have an account? "}
                    <Link to="/register" className="text-blue-600 hover:underline">
                        Register here
                    </Link>
                </p>
            </div>
        </div>
    );
}
