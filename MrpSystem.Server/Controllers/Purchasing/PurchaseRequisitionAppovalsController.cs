using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MrpSystem.Server.Authorization;
using MrpSystem.Server.Data;
using MrpSystem.Server.DTOs.Purchasing;
using MrpSystem.Server.Models;
using MrpSystem.Server.Models.Purchasing;
using MrpSystem.Server.Models.Purchasing.Enums;

namespace MrpSystem.Server.Controllers.Purchasing
{
    [Route("api/purchasing/purchaserequisitions/{requisitionId:int}/approvals")]
    [ApiController]
    [Authorize]
    public class PurchaseRequisitionApprovalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _auth;
        private readonly UserManager<ApplicationUser> _userManager;

        public PurchaseRequisitionApprovalsController(
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
        public async Task<ActionResult<PurchaseRequisitionApprovalDto>> Create(
            int requisitionId,
            [FromBody] PurchaseRequisitionApprovalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var requisition = await _context.PurchaseRequisitions
                .AsNoTracking()
                .FirstOrDefaultAsync(pr => pr.Id == requisitionId);

            if (requisition == null)
                return NotFound();

            var authResult = await _auth.AuthorizeAsync(User, requisition, Operations.Create);
            if (!authResult.Succeeded)
                return Forbid();

            var approval = new PurchaseRequisitionApproval
            {
                PurchaseRequisitionId = requisitionId,
                ApprovedById = dto.ApprovedById,
                ApprovalStatus = ApprovalStatus.Pending,
                Comments = dto.Comments
            };

            _context.PurchaseRequisitionApprovals.Add(approval);
            await _context.SaveChangesAsync();

            var result = MapToDto(approval);

            return CreatedAtAction(nameof(GetById), new { requisitionId, id = approval.Id }, result);
        }

        // --------------------------------------------------
        //                        READ
        // --------------------------------------------------

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseRequisitionApprovalDto>>> GetAll(int requisitionId)
        {
            var requisition = await _context.PurchaseRequisitions
                .AsNoTracking()
                .FirstOrDefaultAsync(pr => pr.Id == requisitionId);

            if (requisition == null)
                return NotFound();

            var authResult = await _auth.AuthorizeAsync(User, requisition, Operations.Read);
            if (!authResult.Succeeded)
                return Forbid();

            var approvals = await _context.PurchaseRequisitionApprovals
                .AsNoTracking()
                .Where(a => a.PurchaseRequisitionId == requisitionId)
                .ToListAsync();

            return Ok(approvals.Select(MapToDto));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PurchaseRequisitionApprovalDto>> GetById(int requisitionId, int id)
        {
            var requisition = await _context.PurchaseRequisitions
                .AsNoTracking()
                .FirstOrDefaultAsync(pr => pr.Id == requisitionId);

            if (requisition == null)
                return NotFound();

            var authResult = await _auth.AuthorizeAsync(User, requisition, Operations.Read);
            if (!authResult.Succeeded)
                return Forbid();

            var approval = await _context.PurchaseRequisitionApprovals
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.PurchaseRequisitionId == requisitionId);

            if (approval == null)
                return NotFound();

            return Ok(MapToDto(approval));
        }

        // --------------------------------------------------
        //                       UPDATE
        // --------------------------------------------------

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int requisitionId,
            int id,
            [FromBody] PurchaseRequisitionApprovalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var approval = await _context.PurchaseRequisitionApprovals
                .FirstOrDefaultAsync(a => a.Id == id && a.PurchaseRequisitionId == requisitionId);

            if (approval == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            if (approval.ApprovedById != userId)
                return Forbid();

            approval.ApprovalStatus = dto.ApprovalStatus;
            approval.Comments = dto.Comments;
            approval.DateApproved =
                dto.ApprovalStatus == ApprovalStatus.Approved ||
                dto.ApprovalStatus == ApprovalStatus.Rejected
                    ? DateTime.UtcNow
                    : null;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --------------------------------------------------
        //                       DELETE
        // --------------------------------------------------

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int requisitionId, int id)
        {
            var requisition = await _context.PurchaseRequisitions
                .AsNoTracking()
                .FirstOrDefaultAsync(pr => pr.Id == requisitionId);

            if (requisition == null)
                return NotFound();

            var authResult = await _auth.AuthorizeAsync(User, requisition, Operations.Delete);
            if (!authResult.Succeeded)
                return Forbid();

            var approval = await _context.PurchaseRequisitionApprovals
                .FirstOrDefaultAsync(a => a.Id == id && a.PurchaseRequisitionId == requisitionId);

            if (approval == null)
                return NotFound();

            _context.PurchaseRequisitionApprovals.Remove(approval);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --------------------------------------------------
        //                      HELPERS
        // --------------------------------------------------

        private static PurchaseRequisitionApprovalDto MapToDto(PurchaseRequisitionApproval approval) =>
            new PurchaseRequisitionApprovalDto
            {
                Id = approval.Id,
                ApprovedById = approval.ApprovedById,
                DateApproved = approval.DateApproved,
                ApprovalStatus = approval.ApprovalStatus,
                Comments = approval.Comments
            };
    }
}