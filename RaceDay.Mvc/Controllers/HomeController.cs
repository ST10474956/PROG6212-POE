using Microsoft.AspNetCore.Mvc; using RaceDay.Mvc.Services;
namespace RaceDay.Mvc.Controllers;
public class HomeController(RaceDayApiClient api):Controller {
 public async Task<IActionResult> Index(string? location,DateTime? date){try{var q="api/events"+(string.IsNullOrWhiteSpace(location)&&!date.HasValue?"":"?"+string.Join("&",new[]{string.IsNullOrWhiteSpace(location)?null:"location="+Uri.EscapeDataString(location),date.HasValue?"date="+date.Value.ToString("yyyy-MM-dd"):null}.Where(x=>x!=null)));ViewBag.Events=await api.Get(q);ViewBag.Location=location;ViewBag.Date=date;return View();}catch(Exception e){ViewBag.Error=e.Message;return View();}}
 public IActionResult Error()=>View();
}
