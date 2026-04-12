using MrpSystem.Server.Models.Purchasing.Enums;

namespace MrpSystem.Server.DTOs.Purchasing
{
    public class PurchaseRequisitionDetailDto
    {
        public int Id { get; set; }
        public int PurchaseRequisitionNumber { get; set; }
        public DateOnly DateRequested { get; set; }
        public DateOnly DateRequired { get; set; }

        public ApplicationUserDto RequestedBy { get; set; } = new();
        public CostCenterDto CostCenter { get; set; } = new();
        public VendorDto Vendor { get; set; } = new();

        public string? Comments { get; set; }
        public decimal TotalCost { get; set; }
        public RequisitionStatus Status { get; set; }

        public List<PurchaseRequisitionItemDto> Items { get; set; } = new();
    }
}
