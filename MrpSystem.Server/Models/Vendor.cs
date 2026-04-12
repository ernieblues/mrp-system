using MrpSystem.Server.Models.Purchasing;
using System.ComponentModel.DataAnnotations;

namespace MrpSystem.Server.Models
{
    public class Vendor
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ContactName { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(25)]
        [Phone]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? AddressLine1 { get; set; }

        [StringLength(200)]
        public string? AddressLine2 { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public List<PurchaseRequisition> PurchaseRequisitions { get; set; } = new();
    }
}