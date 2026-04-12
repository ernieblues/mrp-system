using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MrpSystem.Server.Authorization;
using MrpSystem.Server.Data;
using MrpSystem.Server.DTOs;
using MrpSystem.Server.DTOs.Purchasing;
using MrpSystem.Server.Models;
using MrpSystem.Server.Models.Purchasing;
using MrpSystem.Server.Models.Purchasing.Enums;

namespace MrpSystem.Server.Controllers.Purchasing
{
    [Route("api/purchasing/purchaserequisitions")]
    [ApiController]
    [Authorize]
    public class PurchaseRequisitionsController : ControllerBase
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IAuthorizationService _auth;
        protected readonly UserManager<ApplicationUser> _userManager;

        public PurchaseRequisitionsController(
            ApplicationDbContext context,
            IAuthorizationService auth,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _auth = auth;
            _userManager = userManager;
        }

        // --------------------------------------------------
        //                       CREATE
        // --------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseRequisitionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var dateRequested = DateOnly.FromDateTime(DateTime.Now);

            if (dto.DateRequired < dateRequested)
            {
                return BadRequest("DateRequired must be today or later.");
            }

            var requisition = new PurchaseRequisition
            {
                RequestedById = user.Id,
                DateRequested = dateRequested,
                DateRequired = dto.DateRequired,
                CostCenterId = dto.CostCenterId,
                VendorId = dto.VendorId,
                Comments = dto.Comments,
            };

            requisition.Items = dto.Items.Select(i => new PurchaseRequisitionItem
            {
                Description = i.Description,
                VendorPartNumber = i.VendorPartNumber,
                Quantity = i.Quantity,
                UnitOfMeasure = i.UnitOfMeasure,
                UnitPrice = i.UnitPrice,
            }).ToList();

            var authResult = await _auth.AuthorizeAsync(User, requisition, Operations.Create);
            if (!authResult.Succeeded)
                return Forbid();

            _context.PurchaseRequisitions.Add(requisition);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = requisition.Id },
                MapToDto(requisition, fullItems: true)
            );
        }

        // --------------------------------------------------
        //                        READ
        // --------------------------------------------------

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseRequisitionDto>>> GetAll()
        {
            var requisitions = await _context.PurchaseRequisitions
                .Include(pr => pr.Items)
                .OrderByDescending(pr => pr.DateRequested)
                .ToListAsync();

            var results = new List<PurchaseRequisitionDto>();

            foreach (var pr in requisitions)
            {
                var authResult = await _auth.AuthorizeAsync(User, pr, Operations.Read);
                if (!authResult.Succeeded)
                    continue;

                results.Add(MapToDto(pr));
            }

            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseRequisitionDto>> GetById(int id)
        {
            var requisition = await _context.PurchaseRequisitions
                .Include(pr => pr.RequestedBy)
                .Include(pr => pr.Vendor)
                .Include(pr => pr.CostCenter)
                .Include(pr => pr.Items)
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (requisition == null)
                return NotFound();

            var authResult = await _auth.AuthorizeAsync(User, requisition, Operations.Read);
            if (!authResult.Succeeded)
                return Forbid();

            return Ok(MapToDetailDto(requisition));
        }

        // --------------------------------------------------
        //                       UPDATE
        // --------------------------------------------------

        [HttpPost("{id}/approval-status")]
        public async Task<IActionResult> SetApprovalStatus(int id, [FromBody] ApprovalStatus status)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var pr = await _context.PurchaseRequisitions
                .Include(p => p.Approvals)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pr == null)
                return NotFound();

            var approval = pr.Approvals.FirstOrDefault(a => a.ApprovedById == user.Id);

            if (approval == null)
                return Forbid();

            approval.ApprovalStatus = status;
            approval.DateApproved = status == ApprovalStatus.Pending
                ? null
                : DateTime.UtcNow;

            pr.Status = pr.CalculateStatus();

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PurchaseRequisitionUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var requisition = await _context.PurchaseRequisitions
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (requisition == null)
                return NotFound();

            var authResult = await _auth.AuthorizeAsync(User, requisition, Operations.Update);
            if (!authResult.Succeeded)
                return Forbid();

            if (dto.DateRequired < DateOnly.FromDateTime(DateTime.Now))
            {
                return BadRequest("DateRequired must be today or later");
            }

            requisition.DateRequired = dto.DateRequired;
            requisition.CostCenterId = dto.CostCenterId;
            requisition.VendorId = dto.VendorId;
            requisition.Comments = dto.Comments;

            await _context.SaveChangesAsync();

            return Ok(MapToDto(requisition, fullItems: false));
        }

        // --------------------------------------------------
        //                       DELETE
        // --------------------------------------------------

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var requisition = await _context.PurchaseRequisitions
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (requisition == null)
                return NotFound();

            var authResult = await _auth.AuthorizeAsync(User, requisition, Operations.Delete);
            if (!authResult.Succeeded)
                return Forbid();

            _context.PurchaseRequisitions.Remove(requisition);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --------------------------------------------------
        //                      HELPERS
        // --------------------------------------------------

        private static PurchaseRequisitionDetailDto MapToDetailDto(PurchaseRequisition pr)
        {
            return new PurchaseRequisitionDetailDto
            {
                Id = pr.Id,
                PurchaseRequisitionNumber = pr.PurchaseRequisitionNumber,
                DateRequested = pr.DateRequested,
                DateRequired = pr.DateRequired,

                RequestedBy = new ApplicationUserDto
                {
                    Id = pr.RequestedBy?.Id ?? string.Empty,
                    UserName = pr.RequestedBy?.UserName ?? "Unknown"
                },

                Vendor = new VendorDto
                {
                    Id = pr.Vendor?.Id ?? 0,
                    Code = pr.Vendor?.Code ?? "Unknown",
                    Name = pr.Vendor?.Name ?? "Unknown"
                },

                CostCenter = new CostCenterDto
                {
                    Id = pr.CostCenter?.Id ?? 0,
                    Code = pr.CostCenter?.Code ?? "Unknown",
                    Name = pr.CostCenter?.Name ?? "Unknown"
                },

                Comments = pr.Comments,
                TotalCost = pr.TotalCost,
                Status = pr.Status,

                Items = pr.Items.Select(i => new PurchaseRequisitionItemDto
                {
                    Id = i.Id,
                    LineNumber = i.LineNumber,
                    ProductId = i.ProductId,
                    Description = i.Description,
                    VendorPartNumber = i.VendorPartNumber,
                    Quantity = i.Quantity,
                    UnitOfMeasure = i.UnitOfMeasure,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList()
            };
        }

        private static PurchaseRequisitionDto MapToDto(PurchaseRequisition pr, bool fullItems = false) =>
            new PurchaseRequisitionDto
            {
                Id = pr.Id,
                PurchaseRequisitionNumber = pr.PurchaseRequisitionNumber,
                DateRequested = pr.DateRequested,
                RequestedById = pr.RequestedById,
                CostCenterId = pr.CostCenterId,
                VendorId = pr.VendorId,
                Comments = pr.Comments,
                TotalCost = pr.TotalCost,
                Status = pr.Status,
                Items = pr.Items.Select(i => fullItems
                    ? new PurchaseRequisitionItemDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        Description = i.Description,
                        VendorPartNumber = i.VendorPartNumber,
                        Quantity = i.Quantity,
                        UnitOfMeasure = i.UnitOfMeasure,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.TotalPrice
                    }
                    : new PurchaseRequisitionItemDto { Id = i.Id }
                ).ToList()
            };
    }
}