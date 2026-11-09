using Microsoft.AspNetCore.Mvc; using RaceDay.Mvc.Models; using RaceDay.Mvc.Services;
namespace RaceDay.Mvc.Controllers;
public class AccountController(RaceDayApiClient api):Controller {
 [HttpGet] public IActionResult Login()=>View(new LoginVm());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Login(LoginVm m){if(!ModelState.IsValid)return View(m);try{var x=await api.Login(m.Email,m.Password);var token=x?["token"]?.ToString();if(string.IsNullOrWhiteSpace(token)){ModelState.AddModelError("","The API did not return a login token.");return View(m);}HttpContext.Session.SetString("Token",token);HttpContext.Session.SetString("UserId",x?["userId"]?.ToString()??"");var role=x?["role"]?.ToString()??"Participant";HttpContext.Session.SetString("Role",role);HttpContext.Session.SetString("FullName",x?["fullName"]?.ToString()??"RaceDay user");return RedirectToAction("Index",role=="Organiser"?"Organiser":"Participant");}catch(Exception e){ModelState.AddModelError("",e.Message);return View(m);}}
 [HttpGet] public IActionResult Register()=>View(new RegisterVm());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Register(RegisterVm m){if(!ModelState.IsValid)return View(m);try{await api.Register(m.FullName,m.Email,m.Password,m.Role);TempData["Success"]="Registration successful. Please log in.";return RedirectToAction(nameof(Login));}catch(Exception e){ModelState.AddModelError("",e.Message);return View(m);}}
 public IActionResult Logout(){HttpContext.Session.Clear();return RedirectToAction("Index","Home");}
}
