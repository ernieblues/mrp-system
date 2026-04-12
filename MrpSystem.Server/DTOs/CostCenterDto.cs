namespace MrpSystem.Server.DTOs
{
    public class CostCenterDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
