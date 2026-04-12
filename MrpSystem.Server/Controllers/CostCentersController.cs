using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MrpSystem.Server.Data;
using MrpSystem.Server.DTOs;
using MrpSystem.Server.Models;

namespace MrpSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CostCentersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CostCentersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --------------------------------------------------
        //                       CREATE
        // --------------------------------------------------

        [HttpPost]
        public async Task<ActionResult<CostCenterDto>> Create([FromBody] CostCenterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var costCenter = new CostCenter
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                IsActive = dto.IsActive
            };

            _context.CostCenters.Add(costCenter);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = costCenter.Id },
                MapToDto(costCenter));
        }

        // --------------------------------------------------
        //                        READ
        // --------------------------------------------------

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CostCenterDto>>> GetAll()
        {
            var costCenters = await _context.CostCenters
                .OrderBy(c => c.Name)
                .ToListAsync();

            return Ok(costCenters.Select(MapToDto));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CostCenterDto>> GetById(int id)
        {
            var costCenter = await _context.CostCenters.FindAsync(id);

            if (costCenter == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(costCenter));
        }

        // --------------------------------------------------
        //                       UPDATE
        // --------------------------------------------------

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CostCenterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var costCenter = await _context.CostCenters.FindAsync(id);

            if (costCenter == null)
            {
                return NotFound();
            }

            costCenter.Name = dto.Name;
            costCenter.Code = dto.Code;
            costCenter.Description = dto.Description;
            costCenter.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --------------------------------------------------
        //                       DELETE
        // --------------------------------------------------

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var costCenter = await _context.CostCenters.FindAsync(id);

            if (costCenter == null)
            {
                return NotFound();
            }

            _context.CostCenters.Remove(costCenter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --------------------------------------------------
        //                      HELPERS
        // --------------------------------------------------

        [HttpGet("lookup")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CostCenterLookupDto>>> Lookup()
        {
            var items = await _context.CostCenters
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Code)
                .Select(c => new CostCenterLookupDto
                {
                    Id = c.Id,
                    Display = string.IsNullOrWhiteSpace(c.Code)
                        ? c.Name
                        : c.Code + " - " + c.Name
                })
                .ToListAsync();

            return Ok(items);
        }

        private static CostCenterDto MapToDto(CostCenter c) =>
            new CostCenterDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                Description = c.Description,
                IsActive = c.IsActive
            };
    }
}