using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using RaceDay.Api.Data;using RaceDay.Api.DTOs;
namespace RaceDay.Api.Controllers;
[ApiController][Authorize][Route("api/users")]
public class UsersController(RaceDayDbContext db):ControllerBase{
 int Id=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
 [HttpGet("me")]public async Task<IActionResult> Me()=>Ok(await db.Users.Include(x=>x.Role).Where(x=>x.UserID==Id).Select(x=>new{x.UserID,x.FullName,x.Email,x.ProfilePictureURL,x.CreatedAt,role=x.Role!.RoleName}).SingleOrDefaultAsync());
 [HttpPut("me")]public async Task<IActionResult> Update(ProfileUpdate r){var u=await db.Users.FindAsync(Id);if(u==null)return NotFound();u.FullName=r.FullName;u.ProfilePictureURL=r.ProfilePictureURL;await db.SaveChangesAsync();return Ok(new{u.UserID,u.FullName,u.Email,u.ProfilePictureURL});}
 [HttpDelete("me")]public async Task<IActionResult> Delete(){var u=await db.Users.FindAsync(Id);if(u==null)return NotFound();if(await db.Enrolments.AnyAsync(x=>x.UserID==Id))return Conflict(new{message="Cancel enrolments before deleting account"});db.Users.Remove(u);await db.SaveChangesAsync();return NoContent();}
}
