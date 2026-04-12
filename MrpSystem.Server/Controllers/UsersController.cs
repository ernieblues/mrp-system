using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using MrpSystem.Server.DTOs;
using MrpSystem.Server.Models;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("lookup")]
    [Authorize]
    public IActionResult Lookup()
    {
        var users = _userManager.Users
            .Select(u => new UserLookupDto
            {
                Id = u.Id,
                UserName = u.UserName ?? ""
            })
            .OrderBy(u => u.UserName)
            .ToList();

        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(new UserLookupDto
        {
            Id = user.Id,
            UserName = user.UserName ?? ""
        });
    }
}
