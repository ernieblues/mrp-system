using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace MrpSystem.Server.Models.Purchasing
{
    public class PurchaseRequisitionItem
    {
        public int Id { get; set; }

        public int PurchaseRequisitionId { get; set; }

        public int LineNumber { get; set; }

        public int? ProductId { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        public string? VendorPartNumber { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string UnitOfMeasure { get; set; } = "EA";

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal TotalPrice => Quantity * UnitPrice;

        // Navigation                       
        public PurchaseRequisition PurchaseRequisition { get; set; } = null!;
    }
}
