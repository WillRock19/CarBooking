using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.Validations;
using FluentAssertions;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Models.Validations
{
    public class CarRequestValidatorTests
    {
        private readonly CarRequestValidator _carRequestValidator;

        public CarRequestValidatorTests()
        {
            _carRequestValidator = new CarRequestValidator();
        }
        
        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Validator_WhenMakeIsNullOrEmpty_ReturnsErrorMessage(string make) 
        {
            var propertyName = nameof(CarRequest.Make);
            var carRequest = new CarRequest() { Make = make };

            var result = _carRequestValidator.Validate(carRequest);

            result.Errors
                .Should()
                .Contain(x => x.PropertyName == propertyName && x.ErrorMessage == $"{propertyName} should not be null, empty.");
        }

        [Test]
        public void Validator_WhenMakeHasLessThanTwoCharacters_ReturnsErrorMessage()
        {
            var propertyName = nameof(CarRequest.Make);
            var carRequest = new CarRequest() { Make = "A" };

            var result = _carRequestValidator.Validate(carRequest);

            result.Errors
                .Should()
                .Contain(x => x.PropertyName == propertyName && x.ErrorMessage == $"{propertyName} should have more than 2 characters.");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Validator_WhenModelIsNullOrEmpty_ReturnsErrorMessage(string model)
        {
            var propertyName = nameof(CarRequest.Model);
            var carRequest = new CarRequest() { Model = model };

            var result = _carRequestValidator.Validate(carRequest);

            result.Errors
                .Should()
                .Contain(x => x.PropertyName == propertyName && x.ErrorMessage == $"{propertyName} should not be null, empty.");
        }

        [Test]
        public void Validator_WhenModelHasLessThanTwoCharacters_ReturnsErrorMessage()
        {
            var propertyName = nameof(CarRequest.Model);
            var carRequest = new CarRequest() { Model = "A" };

            var result = _carRequestValidator.Validate(carRequest);

            result.Errors
                .Should()
                .Contain(x => x.PropertyName == propertyName && x.ErrorMessage == $"{propertyName} should have more than 2 characters.");
        }
    }
}
