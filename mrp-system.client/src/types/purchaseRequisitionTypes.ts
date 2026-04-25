export type Mode = "read" | "edit" | "create";

export interface PurchaseRequisitionDetail {
    id: number;
    purchaseRequisitionNumber: number;
    dateRequested: string;
    dateRequired?: string;

    requestedBy: {
        id: string;
        userName: string;
    };

    costCenter: {
        id: number;
        code: string;
        name: string;
    };

    vendor: {
        id: number;
        code: string;
        name: string;
    };

    comments?: string;
    totalCost: number;
    status: number;

    items: PurchaseRequisitionItemDetail[];
}

export interface PurchaseRequisitionForm {
    id?: number;
    purchaseRequisitionNumber?: number;
    dateRequired?: string;
    requestedById: string;
    costCenterId: number | null;
    vendorId: number | null;
    comments?: string;
    status?: number;
    items: PurchaseRequisitionItemForm[];
}

export interface PurchaseRequisitionItemDetail {
    id: number;
    lineNumber: number;
    productId?: number;
    description: string;
    vendorPartNumber?: string;
    quantity: number;
    unitOfMeasure: string;
    unitPrice: number;
    totalPrice: number; // computed by backend
}

export interface PurchaseRequisitionItemForm {
    id?: number; // may not exist yet (new item)
    purchaseRequisitionId?: number;
    lineNumber: number;
    productId?: number;
    description: string;
    vendorPartNumber?: string;
    quantity: number;
    unitOfMeasure: string;
    unitPrice: number;
    totalPrice?: number; // not required, backend recalculates
}
