import React, { useEffect, useState } from "react";

export interface Option {
    value: string;
    label: string;
}

export interface Field {
    name: string;
    label: string;
    type: "text" | "date" | "select" | "time" | "email" | "password";
    required?: boolean;
    default?: string;
    readOnly?: boolean;
    value?: string;
    options?: readonly (string | Option)[];
}

interface FormProps {
    schema: Field[];
    onSubmit: (values: Record<string, string>) => void;
}

export const Form: React.FC<FormProps> = ({ schema, onSubmit }) => {
    const [values, setValues] = useState<Record<string, string>>(
        () => Object.fromEntries(schema.map(f => [f.name, f.value ?? f.default ?? ""]))
    );

    useEffect(() => {
        setValues(prev => {
            const updated = { ...prev };
            for (const f of schema) {
                if (f.value !== undefined) updated[f.name] = f.value;
            }
            return updated;
        });
    }, [schema]);

    const handleChange = (field: string, value: string) => {
        setValues(prev => ({ ...prev, [field]: value }));
    };

    const handleSubmit = (e: React.SyntheticEvent) => {
        e.preventDefault();
        console.log("SUBMITTING:", values);
        onSubmit(values);
    };

    // --------------------------------------------------
    //                   Render Helpers
    // --------------------------------------------------
    const renderInputField = (field: Field): React.ReactElement => (
        <div className="flex flex-col">
            <label className="mb-1 font-medium">{field.label}</label>
            <input
                className="input input-bordered focus:outline-none w-full"
                name={field.name}
                type={field.type}
                value={values[field.name] ?? ""}
                onChange={(e) => handleChange(field.name, e.target.value)}
                required={field.required}
                readOnly={field.readOnly}
            />
        </div>
    );

    // --------------------------------------------------
    //                    Render Form
    // --------------------------------------------------

    const renderField = (field: Field): React.ReactElement => {
        return renderInputField(field);
    };

    return (
        <form
            onSubmit={handleSubmit}
            className="flex flex-col gap-4 w-full max-w-sm bg-base-100 p-6 rounded-lg shadow h-min"
        >
            {schema.map((field) => (
                <React.Fragment key={field.name}>
                    {renderField(field)}
                </React.Fragment>
            ))}

            <button type="submit" className="btn btn-primary mt-2">
                Submit
            </button>
        </form>
    );
};
