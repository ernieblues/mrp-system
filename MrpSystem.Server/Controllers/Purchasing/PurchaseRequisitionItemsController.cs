using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MrpSystem.Server.Authorization;
using MrpSystem.Server.Data;
using MrpSystem.Server.DTOs.Purchasing;
using MrpSystem.Server.Models.Purchasing;

namespace MrpSystem.Server.Controllers.Purchasing
{
    [Route("api/purchasing/purchaserequisitions/{requisitionId:int}/items")]
    [ApiController]
    [Authorize]
    public class PurchaseRequisitionItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _auth;

        public PurchaseRequisitionItemsController(
            ApplicationDbContext context,
            IAuthorizationService auth)
        {
            _context = context;
            _auth = auth;
        }

        // --------------------------------------------------
        //                       CREATE
        // --------------------------------------------------

        [HttpPost]
        public async Task<ActionResult<PurchaseRequisitionItemDto>> Create(
            int requisitionId,
            [FromBody] PurchaseRequisitionItemCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var requisition = await _context.PurchaseRequisitions
                .AsNoTracking()
                .FirstOrDefaultAsync(pr => pr.Id == requisitionId);

            if (requisition == null)
                return NotFound();

            var authResult = await _auth.AuthorizeAsync(User, requisition, Operations.Update);
            if (!authResult.Succeeded)
                return Forbid();

            var items = await GetItems(requisitionId);

            var hasRequestedLineNumber = dto.LineNumber.HasValue && dto.LineNumber.Value > 0;

            int lineNumber = hasRequestedLineNumber
                ? dto.LineNumber!.Value
                : (items.Any() ? items.Max(i => i.LineNumber) + 1 : 1);

            var item = new PurchaseRequisitionItem
            {
                PurchaseRequisitionId = requisitionId,
                LineNumber = lineNumber,
                ProductId = dto.ProductId,
                Description = dto.Description,
                VendorPartNumber = dto.VendorPartNumber,
                Quantity = dto.Quantity,
                UnitOfMeasure = string.IsNullOrWhiteSpace(dto.UnitOfMeasure)
                    ? "EA"
                    : dto.UnitOfMeasure.Trim(),
                UnitPrice = dto.UnitPrice
            };

            _context.PurchaseRequisitionItems.Add(item);
            items.Add(item);

            if (hasRequestedLineNumber)
            {
                SortItems(items, item.Id);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { requisitionId, id = item.Id }, MapToDto(item));
        }

        // --------------------------------------------------
        //                        READ
        // --------------------------------------------------

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseRequisitionItemDto>>> GetAll(int requisitionId)
        {
            var requisition = await _context.PurchaseRequisitions
                .AsNoTracking()
                .Include(pr => pr.Items)
                .FirstOrDefaultAsync(pr => pr.Id == requisitionId);

            if (requisition == null)
                return NotFound();

            var authResult = await _auth.AuthorizeAsync(User, requisition, Operations.Read);
            if (!authResult.Succeeded)
                return Forbid();

            var results = requisition.Items
                .OrderBy(i => i.LineNumber)
                .Select(MapToDto)
                .ToList();

            return Ok(results);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PurchaseRequisitionItemDto>> GetById(int requisitionId, int id)
        {
            var requisition = await _context.PurchaseRequisitions
                .AsNoTracking()
                .FirstOrDefaultAsync(pr => pr.Id == requisitionId);

            if (requisition == null)
                return NotFound();

            var authResult = await _auth.AuthorizeAsync(User, requisition, Operations.Read);
            if (!authResult.Succeeded)
                return Forbid();

            var item = await _context.PurchaseRequisitionItems
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id && i.PurchaseRequisitionId == requisitionId);

            if (item == null)
                return NotFound();

            return Ok(MapToDto(item));
        }

        // --------------------------------------------------
        //                       UPDATE
        // --------------------------------------------------

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int requisitionId,
            int id,
            [FromBody] PurchaseRequisitionItemUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var requisition = await _context.PurchaseRequisitions
                .AsNoTracking()
                .FirstOrDefaultAsync(pr => pr.Id == requisitionId);

            if (requisition == null)
                return NotFound();

            var authResult = await _auth.AuthorizeAsync(User, requisition, Operations.Update);
            if (!authResult.Succeeded)
                return Forbid();

            var item = await _context.PurchaseRequisitionItems
                .FirstOrDefaultAsync(i => i.Id == id && i.PurchaseRequisitionId == requisitionId);

            if (item == null)
                return NotFound();

            item.ProductId = dto.ProductId;
            item.Description = dto.Description;
            item.VendorPartNumber = dto.VendorPartNumber;
            item.Quantity = dto.Quantity;
            item.UnitOfMeasure = string.IsNullOrWhiteSpace(dto.UnitOfMeasure)
                ? "EA"
                : dto.UnitOfMeasure.Trim();
            item.UnitPrice = dto.UnitPrice;

            if (dto.LineNumber.HasValue && dto.LineNumber.Value > 0)
            {
                item.LineNumber = dto.LineNumber!.Value;
                var items = await GetItems(requisitionId);
                SortItems(items, item.Id);
            }

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

            var items = await GetItems(requisitionId);

            var item = items.FirstOrDefault(i => i.Id == id);
            if (item == null)
                return NotFound();

            _context.PurchaseRequisitionItems.Remove(item);
            items.Remove(item);

            SortItems(items);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --------------------------------------------------
        //                      HELPERS
        // --------------------------------------------------

        private async Task<List<PurchaseRequisitionItem>> GetItems(int requisitionId)
        {
            return await _context.PurchaseRequisitionItems
                .Where(i => i.PurchaseRequisitionId == requisitionId)
                .OrderBy(i => i.LineNumber)
                .ThenBy(i => i.Id)
                .ToListAsync();
        }

        private void SortItems(List<PurchaseRequisitionItem> items, int? preferredItemId = null)
        {
            var sorted = items
                .OrderBy(i => i.LineNumber)
                .ThenByDescending(i => i.Id == preferredItemId)
                .ToList();

            for (int i = 0; i < sorted.Count; i++)
            {
                sorted[i].LineNumber = i + 1;
            }
        }

        private static PurchaseRequisitionItemDto MapToDto(PurchaseRequisitionItem item) =>
            new PurchaseRequisitionItemDto
            {
                Id = item.Id,
                LineNumber = item.LineNumber,
                ProductId = item.ProductId,
                Description = item.Description,
                VendorPartNumber = item.VendorPartNumber,
                Quantity = item.Quantity,
                UnitOfMeasure = item.UnitOfMeasure,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            };
    }
}