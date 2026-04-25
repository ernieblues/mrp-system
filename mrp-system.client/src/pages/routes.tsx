import Home from "./Home";
import Login from "./Login";
import Notification from "./Notification";
import PurchaseRequisition from "./purchasing/PurchaseRequisition";
import Register from "./Register";

export const routes = [
    { path: "/", title: "Home", element: <Home /> },
    { path: "/login", title: "Login", element: <Login /> },
    { path: "/notification", title: "Notification", element: <Notification /> },
    { path: "/purchase-requisitions/new", title: "New Purchase Requisition", element: <PurchaseRequisition mode="create" /> },
    { path: "/purchase-requisitions/:id", title: "Purchase Requisition", element: <PurchaseRequisition mode="read" /> },
    { path: "/purchase-requisitions/:id/edit", title: "Edit Purchase Requisition", element: <PurchaseRequisition mode="edit" /> },
    { path: "/register", title: "Register", element: <Register /> },
];
