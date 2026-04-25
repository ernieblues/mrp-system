import type { PurchaseRequisitionForm } from "@/types/purchaseRequisitionTypes";

const BASE_URL = "/api/purchasing/purchaserequisitions";

export async function createPurchaseRequisition(data: PurchaseRequisitionForm) {
    const response = await fetch(BASE_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
    });
    if (!response.ok) throw new Error("Failed to create purchase requisition");
    return response.json();
}

export async function getAllPurchaseRequisitions() {
    const response = await fetch(BASE_URL);
    if (!response.ok) throw new Error("Failed to fetch purchase requisitions");
    return response.json();
}

export async function getPurchaseRequisitionById(id: number) {
    const response = await fetch(`${BASE_URL}/${id}`);
    if (!response.ok) throw new Error("Failed to fetch purchase requisition");
    return response.json();
}

export async function updatePurchaseRequisition(id: number, data: PurchaseRequisitionForm) {
    const response = await fetch(`${BASE_URL}/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
    });
    if (!response.ok) throw new Error("Failed to update purchase requisition");
    return response.json();
}

export async function deletePurchaseRequisition(id: number) {
    const response = await fetch(`${BASE_URL}/${id}`, {
        method: "DELETE",
    });
    if (!response.ok) throw new Error("Failed to delete purchase requisition");
    return response.json();
}
