using System.IdentityModel.Tokens.Jwt; using System.Security.Claims; using System.Security.Cryptography;
using System.Text; using Microsoft.IdentityModel.Tokens;
namespace RaceDay.Api.Services;
public class PasswordService {
 public string Hash(string password){var salt=RandomNumberGenerator.GetBytes(16);var hash=Rfc2898DeriveBytes.Pbkdf2(password,salt,100000,HashAlgorithmName.SHA256,32);return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";}
 public bool Verify(string password,string stored){try{var p=stored.Split('.');var salt=Convert.FromBase64String(p[0]);var expected=Convert.FromBase64String(p[1]);var actual=Rfc2898DeriveBytes.Pbkdf2(password,salt,100000,HashAlgorithmName.SHA256,32);return CryptographicOperations.FixedTimeEquals(actual,expected);}catch{return false;}}
}
public class TokenService(IConfiguration config) {
 public string Create(int id,string email,string role){var key=config["Jwt:Key"]??"DEV_ONLY_CHANGE_THIS_KEY_TO_A_LONG_RANDOM_SECRET_123456";var claims=new[]{new Claim(ClaimTypes.NameIdentifier,id.ToString()),new Claim(ClaimTypes.Email,email),new Claim(ClaimTypes.Role,role)};
 var creds=new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),SecurityAlgorithms.HmacSha256);
 return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(claims:claims,expires:DateTime.UtcNow.AddHours(4),signingCredentials:creds));}
}
