using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MrpSystem.Server.Models.Purchasing.Enums;

namespace MrpSystem.Server.Models.Purchasing
{
    public class PurchaseRequisition
    {
        public int Id { get; set; }
        public int PurchaseRequisitionNumber { get; set; }

        public DateOnly DateRequested { get; set; }
        public DateOnly DateRequired { get; set; }

        [Required]
        public string RequestedById { get; set; } = string.Empty;

        public int CostCenterId { get; set; }
        public int VendorId { get; set; }

        public string? Comments { get; set; }

        [NotMapped]
        public decimal TotalCost => Items.Sum(i => i.Quantity * i.UnitPrice);

        public RequisitionStatus Status { get; set; } = RequisitionStatus.Pending;

        // Navigation
        public ApplicationUser RequestedBy { get; set; } = null!;
        public CostCenter CostCenter { get; set; } = null!;
        public List<PurchaseRequisitionApproval> Approvals { get; set; } = new();
        public List<PurchaseRequisitionItem> Items { get; set; } = new();
        public Vendor Vendor { get; set; } = null!;

        // Helpers
        public RequisitionStatus CalculateStatus()
        {
            if (Approvals.Any(a => a.ApprovalStatus == ApprovalStatus.Rejected))
                return RequisitionStatus.Rejected;

            if (Approvals.Any() &&
                Approvals.All(a => a.ApprovalStatus == ApprovalStatus.Approved))
                return RequisitionStatus.Approved;

            return RequisitionStatus.Pending;
        }
    }
}
