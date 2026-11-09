using System.Net.Http.Headers; 
using System.Net.Http.Json; 
using System.Net;
using System.Text;
using System.Text.Json;
#r "System.Text.Json"
using System.Text.Json.Nodes;
using System.Net.Http;

HttpClient http = new HttpClient
{
    BaseAddress = new Uri("https://your-api-url/")
};
public class RaceDayApiClient(HttpClient http, IHttpContextAccessor accessor) {
 static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web){PropertyNameCaseInsensitive=true};
 void Authorise(){ var token=accessor.HttpContext?.Session.GetString("Token"); http.DefaultRequestHeaders.Authorization=string.IsNullOrWhiteSpace(token)?null:new AuthenticationHeaderValue("Bearer",token); }
 public async Task<JsonNode?> Get(string path){Authorise();var r=await http.GetAsync(path);return await Read(r);}
 public async Task<JsonNode?> Send(string path,HttpMethod method,object? body=null){Authorise();using var req=new HttpRequestMessage(method,path);if(body!=null)req.Content=new StringContent(JsonSerializer.Serialize(body,Json),Encoding.UTF8,"application/json");var r=await http.SendAsync(req);return await Read(r);}
 public async Task<JsonNode?> Upload(string path,IFormFile? file){if(file==null||file.Length==0)return null;Authorise();using var form=new MultipartFormDataContent();await using var stream=file.OpenReadStream();using var content=new StreamContent(stream);content.Headers.ContentType=new MediaTypeHeaderValue(file.ContentType??"application/octet-stream");form.Add(content,"file",Path.GetFileName(file.FileName));var r=await http.PostAsync(path,form);return await Read(r);}
 async Task<JsonNode?> Read(HttpResponseMessage response){var raw=await response.Content.ReadAsStringAsync();if(!response.IsSuccessStatusCode){string msg="The request could not be completed.";try{msg=JsonNode.Parse(raw)?["message"]?.ToString()??JsonNode.Parse(raw)?["title"]?.ToString()??msg;}catch{}throw new ApiException((int)response.StatusCode,msg);}if(string.IsNullOrWhiteSpace(raw))return null;try{return JsonNode.Parse(raw);}catch{return null;}}
 public async Task<JsonNode?> Login(string email,string password){var r=await http.PostAsJsonAsync("api/auth/login",new{email,password});return await Read(r);}
 public async Task<JsonNode?> Register(string fullName,string email,string password,string role){var r=await http.PostAsJsonAsync("api/auth/register",new{fullName,email,password,role});return await Read(r);}
}
public sealed class ApiException(int statusCode,string message):Exception(message){public int StatusCode{get;}=statusCode; }


