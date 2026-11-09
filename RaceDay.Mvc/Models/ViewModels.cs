using System.ComponentModel.DataAnnotations;
namespace RaceDay.Mvc.Models;
public class LoginVm { [Required,EmailAddress] public string Email {get;set;}=""; [Required,DataType(DataType.Password)] public string Password {get;set;}=""; }
public class RegisterVm { [Required,MaxLength(100)] public string FullName {get;set;}=""; [Required,EmailAddress] public string Email {get;set;}=""; [Required,MinLength(8),DataType(DataType.Password)] public string Password {get;set;}=""; [Required] public string Role {get;set;}="Participant"; }
public class EventVm { public int EventID {get;set;} [Required,MaxLength(150)] public string Name {get;set;}=""; public string? Description {get;set;} [Required] public DateTime EventDate {get;set;}=DateTime.Today.AddDays(14); [Required] public string Location {get;set;}=""; [Range(0.1,9999)] public decimal DistanceKm {get;set;}=5; [Required] public string EventType {get;set;}="Run"; public string? BannerImageURL {get;set;} public IFormFile? BannerFile {get;set;} }
public class CategoryVm { public int CategoryID {get;set;} public int EventID {get;set;} [Required] public string Name {get;set;}=""; public decimal? DistanceKm {get;set;} [Range(1,100000)] public int MaxParticipants {get;set;}=100; }
public class ResultVm { public int EnrolmentID {get;set;} public TimeSpan? FinishTime {get;set;} [Range(1,100000)] public int? Position {get;set;} }
