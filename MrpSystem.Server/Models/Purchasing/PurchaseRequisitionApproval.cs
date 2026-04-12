using MrpSystem.Server.Models.Purchasing.Enums;

namespace MrpSystem.Server.Models.Purchasing
{
    public class PurchaseRequisitionApproval
    {
        public int Id { get; set; }

        public int PurchaseRequisitionId { get; set; }

        public string ApprovedById { get; set; } = string.Empty;

        public DateTime? DateApproved { get; set; }

        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;

        public string? Comments { get  ; set; }

        // Navigation
        public PurchaseRequisition PurchaseRequisition { get; set; } = null!;
        public ApplicationUser ApprovedBy { get; set; } = null!;
    }
}
