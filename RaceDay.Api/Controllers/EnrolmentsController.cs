using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using RaceDay.Api.Data;using RaceDay.Api.Models;
namespace RaceDay.Api.Controllers;
[ApiController][Route("api")]
public class EnrolmentsController(RaceDayDbContext db):ControllerBase{
 int UserId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
 [Authorize(Roles="Participant")][HttpPost("categories/{categoryId:int}/enrol")]public async Task<IActionResult> Enrol(int categoryId){var c=await db.Categories.Include(x=>x.Enrolments).FirstOrDefaultAsync(x=>x.CategoryID==categoryId);if(c==null)return NotFound();if(c.Enrolments.Count>=c.MaxParticipants)return Conflict(new{message="Category is full"});if(c.Enrolments.Any(x=>x.UserID==UserId))return Conflict(new{message="Already enrolled"});var e=new Enrolment{UserID=UserId,CategoryID=categoryId};db.Enrolments.Add(e);await db.SaveChangesAsync();return Created("",e);}
 [Authorize(Roles="Participant")][HttpGet("enrolments/me")]public async Task<IActionResult> Mine()=>Ok(await db.Enrolments.Include(x=>x.Category).ThenInclude(x=>x!.Event).Where(x=>x.UserID==UserId).ToListAsync());
 [Authorize(Roles="Organiser")][HttpGet("events/{eventId:int}/enrolments")]public async Task<IActionResult> ForEvent(int eventId){var ev=await db.Events.FindAsync(eventId);if(ev==null)return NotFound();if(ev.OrganiserID!=UserId)return Forbid();return Ok(await db.Enrolments.Include(x=>x.User).Include(x=>x.Category).Where(x=>x.Category!.EventID==eventId).ToListAsync());}
 [Authorize(Roles="Participant")][HttpDelete("enrolments/{id:int}")]public async Task<IActionResult> Cancel(int id){var e=await db.Enrolments.FindAsync(id);if(e==null)return NotFound();if(e.UserID!=UserId)return Forbid();if(await db.Results.AnyAsync(x=>x.EnrolmentID==id))return Conflict(new{message="An enrolment with a result cannot be cancelled"});db.Enrolments.Remove(e);await db.SaveChangesAsync();return NoContent();}
}
