using MrpSystem.Server.Models.Purchasing;
using System.ComponentModel.DataAnnotations;

namespace MrpSystem.Server.Models
{
    public class CostCenter
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public List<PurchaseRequisition> PurchaseRequisitions { get; set; } = new();
    }
}
