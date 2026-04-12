import Home from "./Home";
import Login from "./Login";
import Notification from "./Notification";
import Register from "./Register";

export const routes = [
    { path: "/", title: "Home", element: <Home /> },
    { path: "/login", title: "Login", element: <Login /> },
    { path: "/notification", title: "Notification", element: <Notification /> },
    { path: "/register", title: "Register", element: <Register /> },
];
