using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using RaceDay.Mvc.Models;
using Xunit;
namespace RaceDay.Tests;
public class ViewModelValidationTests {
 [Fact] public void EventWithRequiredFields_IsValid(){var model=new EventVm{Name="City 10K",EventDate=DateTime.Today.AddDays(10),Location="Johannesburg",DistanceKm=10,EventType="Run"};Assert.Empty(Validate(model));}
 [Fact] public void EventWithZeroDistance_IsInvalid(){var model=new EventVm{Name="City 10K",EventDate=DateTime.Today.AddDays(10),Location="Johannesburg",DistanceKm=0,EventType="Run"};Assert.Contains(Validate(model),x=>x.MemberNames.Contains("DistanceKm"));}
 [Fact] public void RegistrationRequiresValidEmailAndPassword(){var model=new RegisterVm{FullName="Test Runner",Email="not-an-email",Password="123",Role="Participant"};var errors=Validate(model);Assert.Contains(errors,x=>x.MemberNames.Contains("Email"));Assert.Contains(errors,x=>x.MemberNames.Contains("Password"));}
 static List<ValidationResult> Validate(object model){var results=new List<ValidationResult>();Validator.TryValidateObject(model,new ValidationContext(model),results,true);return results;}
}
