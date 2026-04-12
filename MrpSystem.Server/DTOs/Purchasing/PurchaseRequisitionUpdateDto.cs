using MrpSystem.Server.Models.Purchasing.Enums;

namespace MrpSystem.Server.DTOs.Purchasing
{
    public class PurchaseRequisitionUpdateDto
    {
        public DateOnly DateRequired { get; set; }
        public int CostCenterId { get; set; }
        public int VendorId { get; set; }
        public string? Comments { get; set; }
    }
}
