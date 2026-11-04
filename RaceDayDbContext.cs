using Microsoft.EntityFrameworkCore; using RaceDay.Api.Models;
namespace RaceDay.Api.Data;
public class RaceDayDbContext(DbContextOptions<RaceDayDbContext> options):DbContext(options) {
 public DbSet<User> Users=>Set<User>(); public DbSet<Role> Roles=>Set<Role>(); public DbSet<Event> Events=>Set<Event>();
 public DbSet<Category> Categories=>Set<Category>(); public DbSet<Enrolment> Enrolments=>Set<Enrolment>(); public DbSet<Result> Results=>Set<Result>();
 protected override void OnModelCreating(ModelBuilder b){
  b.Entity<User>().HasIndex(x=>x.Email).IsUnique(); b.Entity<Enrolment>().HasIndex(x=>new{x.UserID,x.CategoryID}).IsUnique();
  b.Entity<Result>().HasIndex(x=>x.EnrolmentID).IsUnique();
  b.Entity<User>().HasOne(x=>x.Role).WithMany(x=>x.Users).HasForeignKey(x=>x.RoleID);
  b.Entity<Event>().HasOne(x=>x.Organiser).WithMany(x=>x.Events).HasForeignKey(x=>x.OrganiserID).OnDelete(DeleteBehavior.Restrict);
  b.Entity<Category>().HasOne(x=>x.Event).WithMany(x=>x.Categories).HasForeignKey(x=>x.EventID).OnDelete(DeleteBehavior.Cascade);
  b.Entity<Enrolment>().HasOne(x=>x.User).WithMany(x=>x.Enrolments).HasForeignKey(x=>x.UserID).OnDelete(DeleteBehavior.Restrict);
  b.Entity<Enrolment>().HasOne(x=>x.Category).WithMany(x=>x.Enrolments).HasForeignKey(x=>x.CategoryID).OnDelete(DeleteBehavior.Restrict);
  b.Entity<Result>().HasOne(x=>x.Enrolment).WithOne(x=>x.Result).HasForeignKey<Result>(x=>x.EnrolmentID);
  b.Entity<Event>().Property(x=>x.DistanceKm).HasPrecision(5,2); b.Entity<Category>().Property(x=>x.DistanceKm).HasPrecision(5,2);
  b.Entity<Role>().HasData(new Role{RoleID=1,RoleName="Organiser"},new Role{RoleID=2,RoleName="Participant"});
 }
}