using CarReservation.Api.Controllers;
using CarReservation.Api.Interfaces;
using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Tests.Unit.Builders;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Controllers
{
    public class CarControllerTests
    {
        private readonly Mock<IValidator<CarRequest>> carRequestValidatorMock;
        private readonly Mock<IValidator<ReservationRequest>> reservationRequestValidatorMock;
        private readonly Mock<ICarService> carServiceMock;
        private readonly CarController carController;

        public CarControllerTests()
        {
            carRequestValidatorMock = new Mock<IValidator<CarRequest>>();
            reservationRequestValidatorMock = new Mock<IValidator<ReservationRequest>>();
            carServiceMock = new Mock<ICarService>();

            carController = new CarController(carServiceMock.Object, carRequestValidatorMock.Object, reservationRequestValidatorMock.Object);
        }

        internal class GetById : CarControllerTests
        {
            [Test]
            [TestCase(null)]
            [TestCase("")]
            public void WhenCarIdIsNullOrEmpty_ReturnsBadRequestWithMessage(string carId)
            {
                var result = carController.GetById(carId);
                var resultAsBadRequest = (BadRequestObjectResult)result;

                resultAsBadRequest.StatusCode.Should().Be(400);
                resultAsBadRequest.Value.Should().Be("Query parameter car_id cannot be null or empty.");
            }
        }

        internal class AddCar : CarControllerTests 
        {
            [Test]
            public async Task WhenCarRequestIsNull_ReturnsUnprocessableEntityWithMessage()
            {
                var result = await carController.CreateCar(null!);
                var resultAsBadRequest = (UnprocessableEntityObjectResult)result;

                resultAsBadRequest.StatusCode.Should().Be(422);
                resultAsBadRequest.Value.Should().Be("Request content cannot be null.");
            }

            [Test]
            public async Task WhenCarRequestIsInvalid_ReturnsBadRequestWithValidationErrorsInMessage()
            {
                var carRequest = new CarRequestBuilder().Build();
                var error1 = "Error that happened with SomeProperty";
                var error2 = "Error that happened with SomeProperty2";
                var validationFail = new List<ValidationFailure>()
                {
                    new ValidationFailure("SomeProperty", error1),
                    new ValidationFailure("SomeProperty2", error2)
                };
                var validationResultWithError = new ValidationResult(validationFail);
                var expectedListOfErrors = new List<string>() { error1, error2 };

                carRequestValidatorMock.Setup(x => x.ValidateAsync(carRequest, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(validationResultWithError);

                var result = await carController.CreateCar(carRequest);
                
                var resultAsBadRequest = (BadRequestObjectResult)result;
                resultAsBadRequest.StatusCode.Should().Be(400);
                resultAsBadRequest.Value.As<IEnumerable<string>>().ToList().Should().BeEquivalentTo(expectedListOfErrors);
            }
        }

        internal class UpdateCar : CarControllerTests
        {
            [Test]
            [TestCase(null)]
            [TestCase("")]
            public async Task WhenCarIdIsNullOrEmpty_ReturnsBadRequestWithMessage(string carId)
            {
                var result = await carController.UpdateCar(carId, new CarRequestBuilder().Build());
                var resultAsBadRequest = (BadRequestObjectResult)result;

                resultAsBadRequest.StatusCode.Should().Be(400);
                resultAsBadRequest.Value.Should().Be("Query parameter car_id cannot be null or empty.");
            }

            [Test]
            public async Task WhenCarRequestIsNull_ReturnsUnprocessableEntityWithMessage()
            {
                var result = await carController.UpdateCar("C10", null!);
                var resultAsBadRequest = (UnprocessableEntityObjectResult)result;

                resultAsBadRequest.StatusCode.Should().Be(422);
                resultAsBadRequest.Value.Should().Be("Request content cannot be null.");
            }

            [Test]
            public async Task WhenCarRequestIsInvalid_ReturnsBadRequestWithValidationErrorsInMessage()
            {
                var carRequest = new CarRequestBuilder().Build();
                var error1 = "Error that happened with SomeProperty";
                var error2 = "Error that happened with SomeProperty2";
                var validationFail = new List<ValidationFailure>()
                {
                    new ValidationFailure("SomeProperty", error1),
                    new ValidationFailure("SomeProperty2", error2)
                };
                var validationResultWithError = new ValidationResult(validationFail);
                var expectedListOfErrors = new List<string>() { error1, error2 };

                carRequestValidatorMock.Setup(x => x.ValidateAsync(carRequest, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(validationResultWithError);

                var result = await carController.UpdateCar("C10", carRequest);

                var resultAsBadRequest = (BadRequestObjectResult)result;
                resultAsBadRequest.StatusCode.Should().Be(400);
                resultAsBadRequest.Value.As<IEnumerable<string>>().ToList().Should().BeEquivalentTo(expectedListOfErrors);
            }
        }

        internal class RemoveById : CarControllerTests
        {
            [Test]
            [TestCase(null)]
            [TestCase("")]
            public void WhenCarIdIsNullOrEmpty_ReturnsBadRequestWithMessage(string carId)
            {
                var result = carController.RemoveById(carId);
                var resultAsBadRequest = (BadRequestObjectResult)result;

                resultAsBadRequest.StatusCode.Should().Be(400);
                resultAsBadRequest.Value.Should().Be("Query parameter car_id cannot be null or empty.");
            }
        }
    }
}
