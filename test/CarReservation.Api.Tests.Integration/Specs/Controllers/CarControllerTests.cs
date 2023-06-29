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

        internal class CarCrud : CarControllerTests 
        {
            [Test]
            public async Task Client_ShouldBeAbleToAddAndRetrieveSpecificRegister()
            {
                // Arrange
                var carRequest = new CarRequestBuilder().WithMake("First Make").Build();
                var postContent = new StringContent(JsonConvert.SerializeObject(carRequest), Encoding.UTF8, AcceptedContentType);

                var result = await GlobalHttpClient.PostAsync(EndpointBaseRoute, postContent);
                result.EnsureSuccessStatusCode();

                var carCreatedResponse = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
                var cardCreatedId = carCreatedResponse.RootElement.GetProperty("carId").ToString();

                // Act
                var carFromDatabase = await GlobalHttpClient.GetAsync($"{EndpointBaseRoute}/{cardCreatedId}");

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

                var result1 = await GlobalHttpClient.PostAsync(EndpointBaseRoute, postContent1);
                var carCreatedResponse1 = JsonDocument.Parse(await result1.Content.ReadAsStringAsync());
                var cardCreatedId1 = carCreatedResponse1.RootElement.GetProperty("carId").ToString();

                var result2 = await GlobalHttpClient.PostAsync(EndpointBaseRoute, postContent2);
                var carCreatedResponse2 = JsonDocument.Parse(await result1.Content.ReadAsStringAsync());
                var cardCreatedId2 = carCreatedResponse2.RootElement.GetProperty("carId").ToString();

                // Act
                var carFromDatabase = await GlobalHttpClient.GetAsync(EndpointBaseRoute);

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

                var result = await GlobalHttpClient.PostAsync(EndpointBaseRoute, postContent);
                result.EnsureSuccessStatusCode();

                var carAddedResponse = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
                var cardCreatedId = carAddedResponse.RootElement.GetProperty("carId").ToString();

                var updatedCarRequest = new CarRequestBuilder().WithMake("Updating Second Make").WithModel("Updating Second Model").Build();
                var putContent = new StringContent(JsonConvert.SerializeObject(updatedCarRequest), Encoding.UTF8, AcceptedContentType);

                // Act
                var carUpdatedResponse = await GlobalHttpClient.PutAsync($"{EndpointBaseRoute}/{cardCreatedId}", putContent);

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

                var createResponse = await GlobalHttpClient.PostAsync(EndpointBaseRoute, postContent);
                createResponse.EnsureSuccessStatusCode();

                var carCreatedResponse = JsonDocument.Parse(await createResponse.Content.ReadAsStringAsync());
                var cardCreatedId = carCreatedResponse.RootElement.GetProperty("carId").ToString();

                // Act
                var deleteResponse = await GlobalHttpClient.DeleteAsync($"{EndpointBaseRoute}/{cardCreatedId}");

                // Assert
                deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
                (await deleteResponse.Content.ReadAsStringAsync()).Should().BeEmpty();

                var carFromDatabase = await GlobalHttpClient.GetAsync($"{EndpointBaseRoute}/{cardCreatedId}");
                carFromDatabase.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }

            [Test]
            public async Task Client_WhenTryingToReserveCarForSpecificDate_ShouldBeAbleToMakeTheReservion()
            {
                // Assert
                var carRequest = new CarRequestBuilder().WithMake("First Make").Build();
                var createCarPostContent = new StringContent(JsonConvert.SerializeObject(carRequest), Encoding.UTF8, AcceptedContentType);

                var createCarResult = await GlobalHttpClient.PostAsync(EndpointBaseRoute, createCarPostContent);
                createCarResult.EnsureSuccessStatusCode();

                var reservationDate = DateTime.UtcNow.AddHours(5);
                var reservationRequest = new ReservationRequestBuilder()
                    .WithReservationDate(reservationDate)
                    .WithDurationInMinutes(60)
                    .Build();

                var createReservationPostContent = new StringContent(JsonConvert.SerializeObject(reservationRequest), Encoding.UTF8, AcceptedContentType);

                // Act
                var createReservationResult = await GlobalHttpClient.PostAsync($"{EndpointBaseRoute}/reservations", createReservationPostContent);
                var resultContent = JsonConvert.DeserializeObject<ReserveCarResponse>(await createReservationResult.Content.ReadAsStringAsync());

                // Assert
                createReservationResult.StatusCode.Should().Be(HttpStatusCode.OK);
                resultContent?.ReservationId.Should().NotBeNull();
                resultContent?.Message.Should().Contain($"Reservation successfully created for date {reservationDate}. Your reservation ID is: ");
            }

        }

        internal class CarReservation : CarControllerTests 
        {
            private TestServer reservationTestServer;
            private HttpClient clientTestServer;

            [SetUp]
            public void SetUp() 
            {
                reservationTestServer = new WebApplicationFactory<Program>()
                    .WithWebHostBuilder(builder =>
                    {
                        builder.UseEnvironment("Testing");
                    })
                    .Server;

                clientTestServer = reservationTestServer.CreateClient();
            }

            [Test]
            public async Task Client_WhenTryingToMakeMultipleReservationsAndThereAreAvailableCars_ShouldBeAbleToMakeTheReservations()
            {
                // Assert
                var carRequest1 = new CarRequestBuilder().WithMake("Maybe car to be reserved 1").Build();
                var carRequest2 = new CarRequestBuilder().WithMake("Maybe car to be reserved 2").Build();

                var createCarPostContent1 = new StringContent(JsonConvert.SerializeObject(carRequest1), Encoding.UTF8, AcceptedContentType);
                var createCarPostContent2 = new StringContent(JsonConvert.SerializeObject(carRequest2), Encoding.UTF8, AcceptedContentType);

                var createCarResult1 = await GlobalHttpClient.PostAsync(EndpointBaseRoute, createCarPostContent1);
                var createCarResult2 = await GlobalHttpClient.PostAsync(EndpointBaseRoute, createCarPostContent2);

                createCarResult1.EnsureSuccessStatusCode();
                createCarResult2.EnsureSuccessStatusCode();

                var reservationDate1 = DateTime.UtcNow.AddHours(5);
                var reservationDate2 = reservationDate1.AddHours(5);

                var reservationRequest1 = new ReservationRequestBuilder().WithReservationDate(reservationDate1).WithDurationInMinutes(55).Build();
                var reservationRequest2 = new ReservationRequestBuilder().WithReservationDate(reservationDate2).WithDurationInMinutes(87).Build();

                var createReservationPostContent1 = new StringContent(JsonConvert.SerializeObject(reservationRequest1), Encoding.UTF8, AcceptedContentType);
                var createReservationPostContent2 = new StringContent(JsonConvert.SerializeObject(reservationRequest2), Encoding.UTF8, AcceptedContentType);

                // Act
                var createReservationResult1 = await GlobalHttpClient.PostAsync($"{EndpointBaseRoute}/reservations", createReservationPostContent1);
                var createReservationResult2 = await GlobalHttpClient.PostAsync($"{EndpointBaseRoute}/reservations", createReservationPostContent2);

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
                var reservationDate = DateTime.UtcNow.AddHours(5);
                var reservationRequest = new ReservationRequestBuilder()
                    .WithReservationDate(reservationDate)
                    .WithDurationInMinutes(60)
                    .Build();

                var createReservationPostContent = new StringContent(JsonConvert.SerializeObject(reservationRequest), Encoding.UTF8, AcceptedContentType);

                // Act
                var createReservationResult = await clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", createReservationPostContent);
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
                var carRequest = new CarRequestBuilder().WithMake("First Make").Build();
                var createCarPostContent = new StringContent(JsonConvert.SerializeObject(carRequest), Encoding.UTF8, AcceptedContentType);

                var createCarResult = await clientTestServer.PostAsync(EndpointBaseRoute, createCarPostContent);
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

                var firstReservationResult = await clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", firstReservationPostContent);
                firstReservationResult.EnsureSuccessStatusCode();

                // Act
                var secondReservationResult = await clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", secondReservationPostContent);
                var resultContent = JsonConvert.DeserializeObject<ReserveCarResponse>(await secondReservationResult.Content.ReadAsStringAsync());

                // Assert
                secondReservationResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                resultContent?.ReservationId.Should().BeNull();
                resultContent?.Message.Should().Contain($"There's no car available for the desired date and time.");
            }

            [Test]
            public async Task Client_ShouldBeAbleToRetrieveAllUpcomingReservationsWithoutInformingTheDate()
            {
                // Assert
                var reservationIds = await SetUpFourCarsAndReservations();

                // Act
                var result = await clientTestServer.GetAsync($"{EndpointBaseRoute}/reservations");
                result.EnsureSuccessStatusCode();

                // Assert
                var allUpcomingReservations = await result.Content.ReadAsStringAsync();

                false.Should().BeTrue();
            }

            [Test]
            public async Task Client_ShouldBeAbleToRetrieveAllUpcomingReservationsUntilSpecificDate()
            {
                false.Should().BeTrue();
            }

            private async Task<List<Guid?>> SetUpFourCarsAndReservations()
            {
                var carRequest1 = new CarRequestBuilder().WithMake("First Car Make").WithModel("First Car Model").Build();
                var createCarPostContent1 = new StringContent(JsonConvert.SerializeObject(carRequest1), Encoding.UTF8, AcceptedContentType);

                var carRequest2 = new CarRequestBuilder().WithMake("Second Car Make").WithModel("Second Car Model").Build();
                var createCarPostContent2 = new StringContent(JsonConvert.SerializeObject(carRequest2), Encoding.UTF8, AcceptedContentType);

                var carRequest3 = new CarRequestBuilder().WithMake("Third Car Make").WithModel("Third Car Model").Build();
                var createCarPostContent3 = new StringContent(JsonConvert.SerializeObject(carRequest3), Encoding.UTF8, AcceptedContentType);

                var carRequest4 = new CarRequestBuilder().WithMake("Fourth Car Make").WithModel("Fourth Car Model").Build();
                var createCarPostContent4 = new StringContent(JsonConvert.SerializeObject(carRequest4), Encoding.UTF8, AcceptedContentType);

                (await clientTestServer.PostAsync(EndpointBaseRoute, createCarPostContent1)).EnsureSuccessStatusCode();
                (await clientTestServer.PostAsync(EndpointBaseRoute, createCarPostContent2)).EnsureSuccessStatusCode();
                (await clientTestServer.PostAsync(EndpointBaseRoute, createCarPostContent3)).EnsureSuccessStatusCode();
                (await clientTestServer.PostAsync(EndpointBaseRoute, createCarPostContent4)).EnsureSuccessStatusCode();

                var reservationDate1 = DateTime.UtcNow.AddHours(5);
                var reservationDate2 = DateTime.UtcNow.AddHours(10);
                var reservationDate3 = DateTime.UtcNow.AddHours(22);

                var firstReservationRequest = new ReservationRequestBuilder()
                    .WithReservationDate(reservationDate1)
                    .WithDurationInMinutes(60)
                    .Build();

                var secondReservationRequest = new ReservationRequestBuilder()
                    .WithReservationDate(reservationDate2)
                    .WithDurationInMinutes(120)
                    .Build();

                var thirdReservationRequest = new ReservationRequestBuilder()
                    .WithReservationDate(reservationDate2)
                    .WithDurationInMinutes(60)
                    .Build();

                var fourthReservationRequest = new ReservationRequestBuilder()
                    .WithReservationDate(reservationDate3)
                    .WithDurationInMinutes(120)
                    .Build();

                var firstReservationPostContent = new StringContent(JsonConvert.SerializeObject(firstReservationRequest), Encoding.UTF8, AcceptedContentType);
                var secondReservationPostContent = new StringContent(JsonConvert.SerializeObject(secondReservationRequest), Encoding.UTF8, AcceptedContentType);
                var thirdReservationPostContent = new StringContent(JsonConvert.SerializeObject(thirdReservationRequest), Encoding.UTF8, AcceptedContentType);
                var fourthReservationPostContent = new StringContent(JsonConvert.SerializeObject(fourthReservationRequest), Encoding.UTF8, AcceptedContentType);

                var firstReservationResult = await clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", firstReservationPostContent);
                var secondReservationResult = await clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", secondReservationPostContent);
                var thirdReservationResult = await clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", thirdReservationPostContent);
                var fourthReservationResult = await clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", fourthReservationPostContent);

                return new List<Guid?>
                {
                    JsonConvert.DeserializeObject<ReserveCarResponse>(await firstReservationResult.Content.ReadAsStringAsync())?.ReservationId,
                    JsonConvert.DeserializeObject<ReserveCarResponse>(await secondReservationResult.Content.ReadAsStringAsync())?.ReservationId,
                    JsonConvert.DeserializeObject<ReserveCarResponse>(await thirdReservationResult.Content.ReadAsStringAsync())?.ReservationId,
                    JsonConvert.DeserializeObject<ReserveCarResponse>(await fourthReservationResult.Content.ReadAsStringAsync())?.ReservationId,
                };
            }

            [TearDown]
            public void TearDown() 
            {
                clientTestServer?.Dispose();
                reservationTestServer?.Dispose();
            }            
        }       
    }
}
