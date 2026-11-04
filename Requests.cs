using System.ComponentModel.DataAnnotations;
namespace RaceDay.Api.DTOs;
public record RegisterRequest([Required][MaxLength(100)] string FullName,[Required][EmailAddress] string Email,[Required][MinLength(8)] string Password,[Required] string Role);
public record LoginRequest([Required][EmailAddress] string Email,[Required] string Password);
public record ProfileUpdate([Required][MaxLength(100)] string FullName,string? ProfilePictureURL);
public record EventRequest([Required][MaxLength(150)] string Name,string? Description,DateTime EventDate,[Required][MaxLength(150)] string Location,[Range(0.1,9999)] decimal DistanceKm,[RegularExpression("Run|Walk|Cycle")] string EventType,string? BannerImageURL);
public record CategoryRequest([Required][MaxLength(100)] string Name,decimal? DistanceKm,[Range(1,100000)] int MaxParticipants);
public record ResultRequest(TimeSpan? FinishTime,[Range(1,100000)] int? Position);
