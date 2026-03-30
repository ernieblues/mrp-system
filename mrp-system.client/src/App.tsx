import { BrowserRouter, Routes, Route } from "react-router-dom";

import Layout from "./components/Layout";
import { routes } from "./pages/routes";

function App() {
    return (
        <BrowserRouter>
            <Routes>
                {routes.map(r => (
                    <Route
                        key={r.path}
                        path={r.path}
                        element={
                            <Layout title={r.title}>
                                {r.element}
                            </Layout>
                        }
                    />
                ))}

                {/* Catch-all route */}
                <Route
                    path="*"
                    element={
                        <Layout title="Not Found">
                            <h2>Sorry, this page does not exist.</h2>
                        </Layout>
                    }
                />
            </Routes>
        </BrowserRouter>
    );
}

export default App;
