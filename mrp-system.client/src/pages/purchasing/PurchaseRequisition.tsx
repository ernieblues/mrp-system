import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import {
    getPurchaseRequisitionById,
    createPurchaseRequisition,
    updatePurchaseRequisition
} from "@/services/purchaseRequisitionService";
import type { LookupItem } from "@/types/commonTypes";
import type {
    Mode,
    PurchaseRequisitionDetail,
    PurchaseRequisitionForm
} from "@/types/purchaseRequisitionTypes";

function mapDetailToForm(detail: PurchaseRequisitionDetail): PurchaseRequisitionForm {
    return {
        id: detail.id,
        purchaseRequisitionNumber: detail.purchaseRequisitionNumber,
        dateRequired: detail.dateRequired,
        requestedById: detail.requestedBy.id,
        costCenterId: detail.costCenter.id,
        vendorId: detail.vendor.id,
        comments: detail.comments ?? "",
        status: detail.status,
        items: detail.items.map(item => ({
            id: item.id,
            purchaseRequisitionId: detail.id,
            lineNumber: item.lineNumber,
            productId: item.productId,
            description: item.description,
            vendorPartNumber: item.vendorPartNumber,
            quantity: item.quantity,
            unitOfMeasure: item.unitOfMeasure,
            unitPrice: item.unitPrice,
            totalPrice: item.totalPrice
        }))
    };
}

function usePurchaseRequisition(
    mode: Mode,
    id?: number,
) {
    const [costCenters, setCostCenters] = useState<LookupItem[]>([]);
    const [detailData, setDetailData] = useState<PurchaseRequisitionDetail | null>(null);
    const [error, setError] = useState<string | null>(null);

    const [formData, setFormData] = useState<PurchaseRequisitionForm>(() => ({
        id: undefined,
        purchaseRequisitionNumber: undefined,
        dateRequested: "",
        requestedById: "",
        costCenterId: null,
        vendorId: null,
        comments: "",
        status: undefined,
        items: [],
    }));

    const [isLoading, setIsLoading] = useState(mode !== "create");
    const [vendors, setVendors] = useState<LookupItem[]>([]);

    useEffect(() => {
        if (mode === "create" || !id) return;

        setIsLoading(true);
        getPurchaseRequisitionById(id)
            .then(detail => {
                setDetailData(detail);
                setFormData(mapDetailToForm(detail));
            })
            .catch(err => setError(err instanceof Error ? err.message : "Failed to load"))
            .finally(() => setIsLoading(false));
    }, [mode, id]);

    useEffect(() => {
        fetch("/api/costcenters/lookup")
            .then(r => r.json())
            .then(setCostCenters);

        fetch("/api/vendors/lookup")
            .then(r => r.json())
            .then(setVendors);
    }, []);

    const updateField = <K extends keyof PurchaseRequisitionForm>(
        field: K,
        value: PurchaseRequisitionForm[K]
    ) => {
        setFormData(prev => ({ ...prev, [field]: value }));
    };

    const save = async () => {
        if (mode === "create") {
            await createPurchaseRequisition(formData);
            return;
        }

        if (!id) {
            throw new Error("Missing id for update");
        }

        await updatePurchaseRequisition(id, formData);
    };

    return { costCenters, detailData, error, formData, isLoading, save, updateField, vendors };
}

type Props = {
    mode: Mode;
};

export default function PurchaseRequisitionPage({ mode }: Props) {
    const { id } = useParams();
    const { costCenters, detailData, error, formData, isLoading, save, updateField, vendors } =
        usePurchaseRequisition(mode, id ? parseInt(id, 10) : undefined);

    const [isSaving, setIsSaving] = useState(false);
    const [saveError, setSaveError] = useState<string | null>(null);

    const handleSave = async () => {
        setIsSaving(true);
        setSaveError(null);

        try {
            await save();
        } catch (err) {
            setSaveError(err instanceof Error ? err.message : "Save failed");
        } finally {
            setIsSaving(false);
        }
    };

    if (isLoading) return <div>Loading...</div>;

    const total = formData.items.reduce(
        (sum, item) => sum + (item.totalPrice ?? 0),
        0
    );

    return (
        <div className="max-w-3xl mx-auto">
            {/* Header section */}
            <form
                onSubmit={(e) => {
                    e.preventDefault();
                    handleSave();
                }}
                className="p-4"
            >
                <div className="grid grid-cols-4 gap-x-10 gap-y-4">

                    <div>
                        <label htmlFor="requisitionNo" className="label">Requisition No.</label>
                        <input
                            id="requisitionNo"
                            className="input input-ghost w-full"
                            value={formData.purchaseRequisitionNumber ?? ""}
                            disabled
                        />
                    </div>

                    <div className="col-start-3">
                        <label htmlFor="dateRequested" className="label">Date Requested</label>
                        <input
                            id="dateRequested"
                            className="input input-ghost w-full"
                            type="date"
                            value={
                                mode === "create"
                                    ? new Date().toISOString().substring(0, 10)
                                    : detailData?.dateRequested ?? ""
                            }
                            disabled
                            readOnly
                        />
                    </div>

                    <div className="col-start-4">
                        <label htmlFor="dateRequired" className="label">Date Required</label>
                        <input
                            id="dateRequired"
                            className="input input-ghost w-full"
                            type="date"
                            value={formData.dateRequired ?? ""}
                            min={new Date().toISOString().substring(0, 10)}
                            disabled={mode === "read"}
                            onChange={e => updateField("dateRequired", e.target.value)}
                        />
                    </div>

                    <div className="col-span-2">
                        <label htmlFor="requestedBy" className="label">Requested By</label>
                        <input
                            id="requestedBy"
                            className="input input-ghost w-full"
                            value={detailData?.requestedBy.userName ?? ""}
                            disabled
                        />
                    </div>

                    <div className="col-start-4">
                        <label htmlFor="status" className="label">Status</label>
                        <input
                            id="status"
                            className="input input-ghost w-full"
                            value={formData.status ?? ""}
                            disabled
                        />
                    </div>

                    <div className="col-span-2">
                        <label className="label">Cost Center</label>
                        {mode === "read" ? (
                            <input
                                className="input input-ghost w-full"
                                value={
                                    costCenters.find(cc => cc.id === formData.costCenterId)?.display || ""
                                }
                                disabled
                            />
                        ) : (
                            <select
                                className="select select-ghost w-full"
                                value={formData.costCenterId ?? ""}
                                onChange={(e) =>
                                    updateField(
                                        "costCenterId",
                                        e.target.value === "" ? null : Number(e.target.value)
                                    )
                                }
                            >
                                <option value="">Select a cost center</option>
                                {costCenters.map(cc => (
                                    <option key={cc.id} value={cc.id}>
                                        {cc.display}
                                    </option>
                                ))}
                            </select>
                        )}
                    </div>

                    <div className="col-span-2">
                        <label htmlFor="vendor" className="label">Vendor</label>
                        {mode === "read" ? (
                            <input
                                id="vendor"
                                className="input input-ghost w-full"
                                value={
                                    vendors.find(v => v.id === formData.vendorId)?.display || ""
                                }
                                disabled
                            />
                        ) : (
                            <select
                                id="vendor"
                                className="select select-ghost w-full"
                                value={formData.vendorId ?? ""}
                                onChange={(e) =>
                                    updateField(
                                        "vendorId",
                                        e.target.value === "" ? null : Number(e.target.value)
                                    )
                                }
                            >
                                <option value="">Select a vendor</option>
                                {vendors.map(v => (
                                    <option key={v.id} value={v.id}>
                                        {v.display}
                                    </option>
                                ))}
                            </select>
                        )}
                    </div>

                    <div className="col-span-2">
                        <label htmlFor="comments" className="label">Comments</label>
                        <input
                            id="comments"
                            className="input input-ghost w-full"
                            value={formData.comments}
                            disabled={mode === "read"}
                            onChange={e => updateField("comments", e.target.value)}
                        />
                    </div>

                </div>

                {error && <p className="text-red-500 mt-2">{error}</p>}
                {saveError && <p className="text-red-500 mt-2">{saveError}</p>}

                {/* Save button */}
                {mode !== "read" && (
                    <div className="mt-4 flex justify-end">
                        <button
                            type="submit"
                            className="btn btn-sm btn-primary"
                            disabled={isSaving}>
                            {isSaving
                                ? "Saving..."
                                : mode === "create"
                                    ? "Submit"
                                    : "Save header"}
                        </button>
                    </div>
                )}
            </form>

            {/* Items section */}
            <div className="mt-6">
                <h3 className="mb-2 font-semibold">Items</h3>

                {formData.items.length === 0 ? (
                    <div>No items</div>
                ) : (
                    <table className="table table-zebra w-full">
                        <thead>
                            <tr>
                                <th className="w-5">#</th>
                                <th>Description</th>
                                <th className="w-10 text-center">Qty</th>
                                <th className="w-20 text-center">Price</th>
                            </tr>
                        </thead>

                        <tbody>
                            {formData.items.map((item, i) => (
                                <tr key={i}>
                                    <td>{item.lineNumber}</td>
                                    <td>{item.description}</td>
                                    <td className="text-center">{item.quantity}</td>
                                    <td className="text-right tabular-nums whitespace-nowrap">
                                        {item.unitPrice != null ? item.unitPrice.toFixed(2) : ""}
                                    </td>
                                </tr>
                            ))}
                        </tbody>

                        <tfoot>
                            <tr>
                                <td colSpan={3} className="text-right font-semibold">
                                    Total:
                                </td>
                                <td className="text-right font-semibold whitespace-nowrap">
                                    {total.toFixed(2)}
                                </td>
                            </tr>
                        </tfoot>
                    </table>
                )}
            </div>
        </div>
    );
}
