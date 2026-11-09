using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using RaceDay.Api.Data;using RaceDay.Api.DTOs;using RaceDay.Api.Models;using RaceDay.Api.Services;
namespace RaceDay.Api.Controllers;
[ApiController][Route("api/auth")]
public class AuthController(RaceDayDbContext db,PasswordService passwords,TokenService tokens):ControllerBase{
 [HttpPost("register")] public async Task<IActionResult> Register(RegisterRequest r){
  if(await db.Users.AnyAsync(x=>x.Email==r.Email))return Conflict(new{message="Email already registered"});
  var role=await db.Roles.SingleOrDefaultAsync(x=>x.RoleName==r.Role);if(role==null||!(r.Role=="Organiser"||r.Role=="Participant"))return BadRequest(new{message="Role must be Organiser or Participant"});
  var u=new User{FullName=r.FullName,Email=r.Email,PasswordHash=passwords.Hash(r.Password),RoleID=role.RoleID};
  db.Users.Add(u);await db.SaveChangesAsync();return Created("",new{u.UserID,u.FullName,u.Email,role=role.RoleName});
 }
 [HttpPost("login")] public async Task<IActionResult> Login(LoginRequest r){
  var u=await db.Users.Include(x=>x.Role).SingleOrDefaultAsync(x=>x.Email==r.Email);
  if(u==null||!passwords.Verify(r.Password,u.PasswordHash))return Unauthorized(new{message="Email or password is incorrect"});
  return Ok(new{token=tokens.Create(u.UserID,u.Email,u.Role!.RoleName),userId=u.UserID,u.FullName,u.Email,role=u.Role.RoleName,tokenType="Bearer"});
 }
}
