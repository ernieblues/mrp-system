namespace MrpSystem.Server.DTOs
{
    public class VendorDto
    {
        public int Id { get; set; }

        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        public bool IsActive { get; set; }
    }
}
