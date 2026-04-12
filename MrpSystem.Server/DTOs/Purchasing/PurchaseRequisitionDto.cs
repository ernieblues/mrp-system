using MrpSystem.Server.Models.Purchasing.Enums;

namespace MrpSystem.Server.DTOs.Purchasing
{
    public class PurchaseRequisitionDto
    {
        public int Id { get; set; }
        public int PurchaseRequisitionNumber { get; set; }
        public DateOnly DateRequested { get; set; }
        public DateOnly DateRequired { get; set; }
        public string RequestedById { get; set; } = string.Empty;
        public int CostCenterId { get; set; }
        public int VendorId { get; set; }
        public string? Comments { get; set; }
        public decimal TotalCost { get; set; }
        public RequisitionStatus Status { get; set; }
        public List<PurchaseRequisitionItemDto> Items { get; set; } = new();
    }
}
