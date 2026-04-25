
export interface CostCenter {
    id: number;
    name: string;
    code?: string;
    description?: string;
    isActive: boolean;
}

export type LookupItem = {
    id: number | string;
    display: string;
}

export interface User {
    id: string;
    userName: string;
}

export interface Vendor {
    id: number;
    name: string;
    code?: string;
    contactName?: string;
    email?: string;
    phone?: string;
    addressLine1?: string;
    addressLine2?: string;
    city?: string;
    state?: string;
    postalCode?: string;
    country?: string;
    isActive: boolean;
}
