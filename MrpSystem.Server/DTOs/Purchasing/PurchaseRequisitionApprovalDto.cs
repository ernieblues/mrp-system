using MrpSystem.Server.Models.Purchasing.Enums;

namespace MrpSystem.Server.DTOs.Purchasing
{
    public class PurchaseRequisitionApprovalDto
    {
        public int Id { get; set; }

        public string ApprovedById { get; set; } = string.Empty;

        public DateTime? DateApproved { get; set; }

        public ApprovalStatus ApprovalStatus { get; set; }

        public string? Comments { get; set; }
    }
}
