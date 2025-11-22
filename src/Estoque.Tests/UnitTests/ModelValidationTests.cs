using Estoque.Domain.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Estoque.Tests.UnitTests
{
    public class ModelValidationTests
    {
        private IList<ValidationResult> Validate(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void EditUserViewModel_Should_Be_Invalid_With_Empty_Email()
        {
            var model = new EditUserViewModel { Email = "" };
            var errors = Validate(model);
            Assert.Contains(errors, e => e.MemberNames.Contains("Email"));
        }

        [Fact]
        public void ResetPasswordViewModel_Should_Be_Invalid_If_Passwords_Differ()
        {
            var model = new ResetPasswordViewModel 
            { 
                Email = "a@a.com", 
                Password = "123", 
                ConfirmPassword = "456", 
                Token = "abc" 
            };
            var errors = Validate(model);
            Assert.Contains(errors, e => e.MemberNames.Contains("ConfirmPassword"));
        }
    }
}