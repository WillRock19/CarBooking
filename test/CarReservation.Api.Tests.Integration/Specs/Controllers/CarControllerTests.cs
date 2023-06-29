using CarReservation.Api.Models.DTO.Response;
using CarReservation.Api.Tests.Integration.Builders;
using CarReservation.Api.Tests.Integration.Config;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;
using System.Text;
using System.Text.Json;

namespace CarReservation.Api.Tests.Api.Specs.Controllers
{
    public class CarControllerTests : GlobalSetup
    {
        private const string EndpointBaseRoute = "api/v1/car";
        private const string AcceptedContentType = "application/json";

        [Test]
        public async Task Client_ShouldBeAbleToAddAndRetrieveSpecificRegister() 
        {
            // Arrange
            var carRequest = new CarRequestBuilder().WithMake("First Make").Build();
            var postContent = new StringContent(JsonConvert.SerializeObject(carRequest), Encoding.UTF8, AcceptedContentType);

            var result = await HttpClient.PostAsync(EndpointBaseRoute, postContent);
            result.EnsureSuccessStatusCode();

            var carCreatedResponse = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
            var cardCreatedId = carCreatedResponse.RootElement.GetProperty("carId").ToString();

            // Act
            var carFromDatabase = await HttpClient.GetAsync($"{EndpointBaseRoute}/{cardCreatedId}");

            // Assert
            var resultDeserialized = JsonConvert.DeserializeObject<CarResponse>(await carFromDatabase.Content.ReadAsStringAsync());
            resultDeserialized?.Id.Should().Be(cardCreatedId);
            resultDeserialized?.Make.Should().Be(carRequest.Make);
            resultDeserialized?.Model.Should().Be(carRequest.Model);
        }

        [Test]
        public async Task Client_ShouldBeAbleToAddAndRetrieveMultipleRegisters()
        {
            // Arrange
            var carRequest1 = new CarRequestBuilder().WithMake("GettingAll Make 1").WithModel("GettingAll Model 1").Build();
            var carRequest2 = new CarRequestBuilder().WithMake("GettingAll Make 2").WithModel("GettingAll Model 2").Build();
            var postContent1 = new StringContent(JsonConvert.SerializeObject(carRequest1), Encoding.UTF8, AcceptedContentType);
            var postContent2 = new StringContent(JsonConvert.SerializeObject(carRequest2), Encoding.UTF8, AcceptedContentType);

            var result1 = await HttpClient.PostAsync(EndpointBaseRoute, postContent1);
            var carCreatedResponse1 = JsonDocument.Parse(await result1.Content.ReadAsStringAsync());
            var cardCreatedId1 = carCreatedResponse1.RootElement.GetProperty("carId").ToString();

            var result2 = await HttpClient.PostAsync(EndpointBaseRoute, postContent2);
            var carCreatedResponse2 = JsonDocument.Parse(await result1.Content.ReadAsStringAsync());
            var cardCreatedId2 = carCreatedResponse2.RootElement.GetProperty("carId").ToString();

            // Act
            var carFromDatabase = await HttpClient.GetAsync(EndpointBaseRoute);

            // Assert
            var resultDeserialized = JsonConvert.DeserializeObject<IEnumerable<CarResponse>>(await carFromDatabase.Content.ReadAsStringAsync());
            resultDeserialized.Should().NotBeEmpty().And.NotHaveCount(1);
            resultDeserialized.Should().Contain(x => x.Make == carRequest1.Make && x.Model == carRequest1.Model);
            resultDeserialized.Should().Contain(x => x.Make == carRequest2.Make && x.Model == carRequest2.Model);
        }

        [Test]
        public async Task Client_ShouldBeAbleToAddAndUpdateSpecificRegister()
        {
            // Arrange
            var carRequest = new CarRequestBuilder().WithMake("Updating First Make").WithModel("Updating First Model").Build();
            var postContent = new StringContent(JsonConvert.SerializeObject(carRequest), Encoding.UTF8, AcceptedContentType);

            var result = await HttpClient.PostAsync(EndpointBaseRoute, postContent);
            result.EnsureSuccessStatusCode();

            var carAddedResponse = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
            var cardCreatedId = carAddedResponse.RootElement.GetProperty("carId").ToString();

            var updatedCarRequest = new CarRequestBuilder().WithMake("Updating Second Make").WithModel("Updating Second Model").Build();
            var putContent = new StringContent(JsonConvert.SerializeObject(updatedCarRequest), Encoding.UTF8, AcceptedContentType);

            // Act
            var carUpdatedResponse = await HttpClient.PutAsync($"{EndpointBaseRoute}/{cardCreatedId}", putContent);

            // Assert
            carUpdatedResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);

            var resultDeserialized = JsonConvert.DeserializeObject<CarResponse>(await carUpdatedResponse.Content.ReadAsStringAsync());
            resultDeserialized?.Id.Should().Be(cardCreatedId);
            resultDeserialized?.Make.Should().Be(updatedCarRequest.Make);
            resultDeserialized?.Model.Should().Be(updatedCarRequest.Model);
        }

        [Test]
        public async Task Client_ShouldBeAbleToAddAndDeleteSpecificRegister()
        {
            // Arrange
            var carRequest = new CarRequestBuilder().WithMake("Delete Make").WithModel("Delete Model").Build();
            var postContent = new StringContent(JsonConvert.SerializeObject(carRequest), Encoding.UTF8, AcceptedContentType);

            var createResponse = await HttpClient.PostAsync(EndpointBaseRoute, postContent);
            createResponse.EnsureSuccessStatusCode();

            var carCreatedResponse = JsonDocument.Parse(await createResponse.Content.ReadAsStringAsync());
            var cardCreatedId = carCreatedResponse.RootElement.GetProperty("carId").ToString();

            // Act
            var deleteResponse = await HttpClient.DeleteAsync($"{EndpointBaseRoute}/{cardCreatedId}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            (await deleteResponse.Content.ReadAsStringAsync()).Should().BeEmpty();

            var carFromDatabase = await HttpClient.GetAsync($"{EndpointBaseRoute}/{cardCreatedId}");
            carFromDatabase.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Client_WhenTryingToReserveCarForSpecificDate_ShouldBeAbleToMakeTheReservion() 
        {
            // Assert
            var carRequest = new CarRequestBuilder().WithMake("First Make").Build();
            var createCarPostContent = new StringContent(JsonConvert.SerializeObject(carRequest), Encoding.UTF8, AcceptedContentType);

            var createCarResult = await HttpClient.PostAsync(EndpointBaseRoute, createCarPostContent);
            createCarResult.EnsureSuccessStatusCode();

            var reservationDate = DateTime.UtcNow.AddHours(5);
            var reservationRequest = new ReservationRequestBuilder()
                .WithReservationDate(reservationDate)
                .WithDurationInMinutes(60)
                .Build();

            var createReservationPostContent = new StringContent(JsonConvert.SerializeObject(reservationRequest), Encoding.UTF8, AcceptedContentType);

            // Act
            var createReservationResult = await HttpClient.PostAsync($"{EndpointBaseRoute}/reservations", createReservationPostContent);
            var resultContent = JsonConvert.DeserializeObject<ReserveCarResponse>(await createReservationResult.Content.ReadAsStringAsync());

            // Assert
            createReservationResult.StatusCode.Should().Be(HttpStatusCode.OK);
            resultContent?.ReservationId.Should().NotBeNull();
            resultContent?.Message.Should().Contain($"Reservation successfully created for date {reservationDate}. Your reservation ID is: ");
        }

        [Test]
        public async Task Client_WhenTryingToMakeMultipleReservationsAndThereAreAvailableCars_ShouldBeAbleToMakeTheReservations()
        {
            // Assert
            var carRequest1 = new CarRequestBuilder().WithMake("Maybe car to be reserved 1").Build();
            var carRequest2 = new CarRequestBuilder().WithMake("Maybe car to be reserved 2").Build();

            var createCarPostContent1 = new StringContent(JsonConvert.SerializeObject(carRequest1), Encoding.UTF8, AcceptedContentType);
            var createCarPostContent2 = new StringContent(JsonConvert.SerializeObject(carRequest2), Encoding.UTF8, AcceptedContentType);

            var createCarResult1 = await HttpClient.PostAsync(EndpointBaseRoute, createCarPostContent1);
            var createCarResult2 = await HttpClient.PostAsync(EndpointBaseRoute, createCarPostContent2);

            createCarResult1.EnsureSuccessStatusCode();
            createCarResult2.EnsureSuccessStatusCode();

            var reservationDate1 = DateTime.UtcNow.AddHours(5);
            var reservationDate2 = reservationDate1.AddHours(5);

            var reservationRequest1 = new ReservationRequestBuilder().WithReservationDate(reservationDate1).WithDurationInMinutes(55).Build();
            var reservationRequest2 = new ReservationRequestBuilder().WithReservationDate(reservationDate2).WithDurationInMinutes(87).Build();

            var createReservationPostContent1 = new StringContent(JsonConvert.SerializeObject(reservationRequest1), Encoding.UTF8, AcceptedContentType);
            var createReservationPostContent2 = new StringContent(JsonConvert.SerializeObject(reservationRequest2), Encoding.UTF8, AcceptedContentType);

            // Act
            var createReservationResult1 = await HttpClient.PostAsync($"{EndpointBaseRoute}/reservations", createReservationPostContent1);
            var createReservationResult2 = await HttpClient.PostAsync($"{EndpointBaseRoute}/reservations", createReservationPostContent2);

            var resultContent1 = JsonConvert.DeserializeObject<ReserveCarResponse>(await createReservationResult1.Content.ReadAsStringAsync());
            var resultContent2 = JsonConvert.DeserializeObject<ReserveCarResponse>(await createReservationResult2.Content.ReadAsStringAsync());

            // Assert
            createReservationResult1.EnsureSuccessStatusCode();
            createReservationResult2.EnsureSuccessStatusCode();

            resultContent1?.ReservationId.Should().NotBeNull();
            resultContent1?.Message.Should().Contain($"Reservation successfully created for date {reservationDate1}. Your reservation ID is: ");

            resultContent2?.ReservationId.Should().NotBeNull();
            resultContent2?.Message.Should().Contain($"Reservation successfully created for date {reservationDate2}. Your reservation ID is: ");
        }

        [Test]
        public async Task Client_WhenTryingToMakeReservationButNoCarsWhereRegistered_ShouldReceiveAnErrorMessage()
        {
            // Assert
            var newTestServerWithEmptyDatabase = CreateNewTestServerToGuaranteeDatabaseIsEmpty();
            var newTestClient = newTestServerWithEmptyDatabase.CreateClient();

            var reservationDate = DateTime.UtcNow.AddHours(5);
            var reservationRequest = new ReservationRequestBuilder()
                .WithReservationDate(reservationDate)
                .WithDurationInMinutes(60)
                .Build();

            var createReservationPostContent = new StringContent(JsonConvert.SerializeObject(reservationRequest), Encoding.UTF8, AcceptedContentType);

            // Act
            var createReservationResult = await newTestClient.PostAsync($"{EndpointBaseRoute}/reservations", createReservationPostContent);
            var resultContent = JsonConvert.DeserializeObject<ReserveCarResponse>(await createReservationResult.Content.ReadAsStringAsync());

            // Assert
            createReservationResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            resultContent?.ReservationId.Should().BeNull();
            resultContent?.Message.Should().Contain($"There's no car available for the desired date and time.");
        }

        [Test]
        public async Task Client_WhenTryingToMakeReservationButNoCarsAreAvailable_ShouldReceiveErrorMessage()
        {
            // Assert
            var newTestServerWithEmptyDatabase = CreateNewTestServerToGuaranteeDatabaseIsEmpty();
            var newTestClient = newTestServerWithEmptyDatabase.CreateClient();

            var carRequest = new CarRequestBuilder().WithMake("First Make").Build();
            var createCarPostContent = new StringContent(JsonConvert.SerializeObject(carRequest), Encoding.UTF8, AcceptedContentType);

            var createCarResult = await newTestClient.PostAsync(EndpointBaseRoute, createCarPostContent);
            createCarResult.EnsureSuccessStatusCode();

            var reservationDate = DateTime.UtcNow.AddHours(5);
            var firstReservationRequest = new ReservationRequestBuilder()
                .WithReservationDate(reservationDate)
                .WithDurationInMinutes(60)
                .Build();

            var secondReservationRequest = new ReservationRequestBuilder()
                .WithReservationDate(reservationDate)
                .WithDurationInMinutes(120)
                .Build();

            var firstReservationPostContent = new StringContent(JsonConvert.SerializeObject(firstReservationRequest), Encoding.UTF8, AcceptedContentType);
            var secondReservationPostContent = new StringContent(JsonConvert.SerializeObject(secondReservationRequest), Encoding.UTF8, AcceptedContentType);

            var firstReservationResult = await newTestClient.PostAsync($"{EndpointBaseRoute}/reservations", firstReservationPostContent);
            firstReservationResult.EnsureSuccessStatusCode();

            // Act
            var secondReservationResult = await newTestClient.PostAsync($"{EndpointBaseRoute}/reservations", secondReservationPostContent);
            var resultContent = JsonConvert.DeserializeObject<ReserveCarResponse>(await secondReservationResult.Content.ReadAsStringAsync());

            // Assert
            secondReservationResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            resultContent?.ReservationId.Should().BeNull();
            resultContent?.Message.Should().Contain($"There's no car available for the desired date and time.");
        }

        [Test]
        public async Task Client_ShouldBeAbleToRetrieveAllUpcomingReservationsWithoutInformingTheDate()
        {
            false.Should().BeTrue();
        }

        [Test]
        public async Task Client_ShouldBeAbleToRetrieveAllUpcomingReservationsUntilSpecificDate()
        {
            false.Should().BeTrue();
        }

        private TestServer CreateNewTestServerToGuaranteeDatabaseIsEmpty() => 
            new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                })
                .Server;
    }
}
