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
    public class VendorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VendorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --------------------------------------------------
        //                       CREATE
        // --------------------------------------------------

        [HttpPost]
        public async Task<ActionResult<VendorDto>> Create([FromBody] VendorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vendor = new Vendor
            {
                Name = dto.Name,
                Code = dto.Code,
                ContactName = dto.ContactName,
                Email = dto.Email,
                Phone = dto.Phone,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                IsActive = dto.IsActive
            };

            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = vendor.Id },
                MapToDto(vendor));
        }

        // --------------------------------------------------
        //                        READ
        // --------------------------------------------------

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorDto>>> GetAll()
        {
            var vendors = await _context.Vendors
                .OrderBy(v => v.Name)
                .ToListAsync();

            return Ok(vendors.Select(MapToDto));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VendorDto>> GetById(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);

            if (vendor == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(vendor));
        }

        // --------------------------------------------------
        //                       UPDATE
        // --------------------------------------------------

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] VendorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vendor = await _context.Vendors.FindAsync(id);

            if (vendor == null)
            {
                return NotFound();
            }

            vendor.Name = dto.Name;
            vendor.Code = dto.Code;
            vendor.ContactName = dto.ContactName;
            vendor.Email = dto.Email;
            vendor.Phone = dto.Phone;
            vendor.AddressLine1 = dto.AddressLine1;
            vendor.AddressLine2 = dto.AddressLine2;
            vendor.City = dto.City;
            vendor.State = dto.State;
            vendor.PostalCode = dto.PostalCode;
            vendor.Country = dto.Country;
            vendor.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --------------------------------------------------
        //                       DELETE
        // --------------------------------------------------

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);

            if (vendor == null)
            {
                return NotFound();
            }

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --------------------------------------------------
        //                      HELPERS
        // --------------------------------------------------

        [HttpGet("lookup")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<VendorLookupDto>>> Lookup()
        {
            var items = await _context.Vendors
                .AsNoTracking()
                .Where(v => v.IsActive)
                .OrderBy(v => v.Code)
                .Select(v => new VendorLookupDto
                {
                    Id = v.Id,
                    Display = string.IsNullOrWhiteSpace(v.Code)
                        ? v.Name
                        : v.Code + " - " + v.Name
                })
                .ToListAsync();

            return Ok(items);
        }

        private static VendorDto MapToDto(Vendor vendor) =>
            new VendorDto
            {
                Id = vendor.Id,
                Name = vendor.Name,
                Code = vendor.Code,
                ContactName = vendor.ContactName,
                Email = vendor.Email,
                Phone = vendor.Phone,
                AddressLine1 = vendor.AddressLine1,
                AddressLine2 = vendor.AddressLine2,
                City = vendor.City,
                State = vendor.State,
                PostalCode = vendor.PostalCode,
                Country = vendor.Country,
                IsActive = vendor.IsActive
            };
    }
}