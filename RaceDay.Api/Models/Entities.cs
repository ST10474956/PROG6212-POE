using System.ComponentModel.DataAnnotations;
namespace RaceDay.Api.Models;
public class User {
 public int UserID {get;set;} [MaxLength(100)] public string FullName {get;set;}="";
 [MaxLength(150)] public string Email {get;set;}="";
 [MaxLength(255)] public string PasswordHash {get;set;}="";
 public int RoleID {get;set;} public Role? Role {get;set;}
 [MaxLength(500)] public string? ProfilePictureURL {get;set;} public DateTime CreatedAt {get;set;}=DateTime.Now;
 public ICollection<Event> Events {get;set;}=new List<Event>(); public ICollection<Enrolment> Enrolments {get;set;}=new List<Enrolment>();
}
public class Role { public int RoleID {get;set;} [MaxLength(20)] public string RoleName {get;set;}=""; public ICollection<User> Users {get;set;}=new List<User>();}
public class Event {
 public int EventID {get;set;} [MaxLength(150)] public string Name {get;set;}="";
 [MaxLength(500)] public string? Description {get;set;} public DateTime EventDate {get;set;}
 [MaxLength(150)] public string Location {get;set;}=""; public decimal DistanceKm {get;set;}
 [MaxLength(10)] public string EventType {get;set;}="Run"; [MaxLength(500)] public string? BannerImageURL {get;set;}
 public int OrganiserID {get;set;} public User? Organiser {get;set;} public DateTime CreatedAt {get;set;}=DateTime.Now;
 public ICollection<Category> Categories {get;set;}=new List<Category>();
}
public class Category {
 public int CategoryID {get;set;} public int EventID {get;set;} public Event? Event {get;set;}
 [MaxLength(100)] public string Name {get;set;}=""; public decimal? DistanceKm {get;set;}
 public int MaxParticipants {get;set;}=100; public ICollection<Enrolment> Enrolments {get;set;}=new List<Enrolment>();
}
public class Enrolment {
 public int EnrolmentID {get;set;} public int UserID {get;set;} public User? User {get;set;}
 public int CategoryID {get;set;} public Category? Category {get;set;} public DateTime EnrolmentDate {get;set;}=DateTime.Now;
 [MaxLength(20)] public string Status {get;set;}="Confirmed"; public Result? Result {get;set;}
}
public class Result {
 public int ResultID {get;set;} public int EnrolmentID {get;set;} public Enrolment? Enrolment {get;set;}
 public TimeSpan? FinishTime {get;set;} public int? Position {get;set;} [MaxLength(20)] public string Status {get;set;}="Finished";
}
