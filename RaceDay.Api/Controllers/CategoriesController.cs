using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using RaceDay.Api.Data;using RaceDay.Api.DTOs;using RaceDay.Api.Models;
namespace RaceDay.Api.Controllers;
[ApiController][Route("api")]
public class CategoriesController(RaceDayDbContext db):ControllerBase{
 int UserId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
 [HttpGet("events/{eventId:int}/categories")]public async Task<IActionResult> List(int eventId){if(!await db.Events.AnyAsync(x=>x.EventID==eventId))return NotFound();return Ok(await db.Categories.Where(x=>x.EventID==eventId).ToListAsync());}
 [Authorize(Roles="Organiser")][HttpPost("events/{eventId:int}/categories")]public async Task<IActionResult> Create(int eventId,CategoryRequest r){var e=await db.Events.FindAsync(eventId);if(e==null)return NotFound();if(e.OrganiserID!=UserId)return Forbid();var c=new Category{EventID=eventId,Name=r.Name,DistanceKm=r.DistanceKm,MaxParticipants=r.MaxParticipants};db.Categories.Add(c);await db.SaveChangesAsync();return Created("",c);}
 [Authorize(Roles="Organiser")][HttpPut("categories/{id:int}")]public async Task<IActionResult> Update(int id,CategoryRequest r){var c=await db.Categories.Include(x=>x.Event).FirstOrDefaultAsync(x=>x.CategoryID==id);if(c==null)return NotFound();if(c.Event!.OrganiserID!=UserId)return Forbid();c.Name=r.Name;c.DistanceKm=r.DistanceKm;c.MaxParticipants=r.MaxParticipants;await db.SaveChangesAsync();return Ok(c);}
 [Authorize(Roles="Organiser")][HttpDelete("categories/{id:int}")]public async Task<IActionResult> Delete(int id){var c=await db.Categories.Include(x=>x.Event).FirstOrDefaultAsync(x=>x.CategoryID==id);if(c==null)return NotFound();if(c.Event!.OrganiserID!=UserId)return Forbid();if(await db.Enrolments.AnyAsync(x=>x.CategoryID==id))return Conflict(new{message="Category has enrolments"});db.Categories.Remove(c);await db.SaveChangesAsync();return NoContent();}
}
