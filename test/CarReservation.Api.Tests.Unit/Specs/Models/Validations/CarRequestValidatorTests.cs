using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.Validations;
using CarReservation.Api.Tests.Unit.Builders.DTO;
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
        public void Validator_WhenCarRequestIsValid_ShouldNotReturnErrorMessage()
        {
            var carRequest = new CreateCarRequestBuilder().WithMake("Some make").WithModel("Some model").Build();

            var result = _carRequestValidator.Validate(carRequest);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Test]
        public void Validator_WhenMakeIsEmpty_ReturnsErrorMessage() 
        {
            var propertyName = nameof(CreateCarRequest.Make);
            var carRequest = new CreateCarRequestBuilder().WithMake(string.Empty).Build();

            var result = _carRequestValidator.Validate(carRequest);

            result.Errors
                .Should()
                .Contain(x => x.PropertyName == propertyName && x.ErrorMessage == $"{propertyName} should not be empty.");
        }

        [Test]
        public void Validator_WhenMakeHasLessThanTwoCharacters_ReturnsErrorMessage()
        {
            var propertyName = nameof(CreateCarRequest.Make);
            var carRequest = new CreateCarRequestBuilder().WithMake("A").Build();

            var result = _carRequestValidator.Validate(carRequest);

            result.Errors
                .Should()
                .Contain(x => x.PropertyName == propertyName && x.ErrorMessage == $"{propertyName} should have more than 2 characters.");
        }

        [Test]
        public void Validator_WhenModelIsEmpty_ReturnsErrorMessage()
        {
            var propertyName = nameof(CreateCarRequest.Model);
            var carRequest = new CreateCarRequestBuilder().WithModel(string.Empty).Build();

            var result = _carRequestValidator.Validate(carRequest);

            result.Errors
                .Should()
                .Contain(x => x.PropertyName == propertyName && x.ErrorMessage == $"{propertyName} should not be empty.");
        }

        [Test]
        public void Validator_WhenModelHasLessThanTwoCharacters_ReturnsErrorMessage()
        {
            var propertyName = nameof(CreateCarRequest.Model);
            var carRequest = new CreateCarRequestBuilder().WithModel("A").Build();

            var result = _carRequestValidator.Validate(carRequest);

            result.Errors
                .Should()
                .Contain(x => x.PropertyName == propertyName && x.ErrorMessage == $"{propertyName} should have more than 2 characters.");
        }
    }
}
