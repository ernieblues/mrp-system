namespace MrpSystem.Server.DTOs.Purchasing
{
    public class PurchaseRequisitionItemUpdateDto
    {
        public int? LineNumber { get; set; }
        public int? ProductId { get; set; }
        public string Description { get; set; } = null!;
        public string? VendorPartNumber { get; set; }
        public int Quantity { get; set; }
        public string UnitOfMeasure { get; set; } = null!;
        public decimal UnitPrice { get; set; }
    }
}
