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
                var carRequest = new CreateCarRequestBuilder().WithMake("First Make").Build();
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
                var carRequest1 = new CreateCarRequestBuilder().WithMake("GettingAll Make 1").WithModel("GettingAll Model 1").Build();
                var carRequest2 = new CreateCarRequestBuilder().WithMake("GettingAll Make 2").WithModel("GettingAll Model 2").Build();
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
                var carRequest = new CreateCarRequestBuilder().WithMake("Updating First Make").WithModel("Updating First Model").Build();
                var postContent = new StringContent(JsonConvert.SerializeObject(carRequest), Encoding.UTF8, AcceptedContentType);

                var result = await GlobalHttpClient.PostAsync(EndpointBaseRoute, postContent);
                result.EnsureSuccessStatusCode();

                var carAddedResponse = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
                var cardCreatedId = carAddedResponse.RootElement.GetProperty("carId").ToString();

                var updatedCarRequest = new CreateCarRequestBuilder().WithMake("Updating Second Make").WithModel("Updating Second Model").Build();
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
                var carRequest = new CreateCarRequestBuilder().WithMake("Delete Make").WithModel("Delete Model").Build();
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
                var carRequest = new CreateCarRequestBuilder().WithMake("First Make").Build();
                var createCarPostContent = new StringContent(JsonConvert.SerializeObject(carRequest), Encoding.UTF8, AcceptedContentType);

                var createCarResult = await GlobalHttpClient.PostAsync(EndpointBaseRoute, createCarPostContent);
                createCarResult.EnsureSuccessStatusCode();

                var reservationDate = DateTime.UtcNow.AddHours(5);
                var reservationRequest = new CreateReservationRequestBuilder()
                    .WithReservationDate(reservationDate)
                    .WithDurationInMinutes(60)
                    .Build();

                var createReservationPostContent = new StringContent(JsonConvert.SerializeObject(reservationRequest), Encoding.UTF8, AcceptedContentType);

                // Act
                var createReservationResult = await GlobalHttpClient.PostAsync($"{EndpointBaseRoute}/reservations", createReservationPostContent);
                var resultContent = JsonConvert.DeserializeObject<CreateReservationResponse>(await createReservationResult.Content.ReadAsStringAsync());

                // Assert
                createReservationResult.StatusCode.Should().Be(HttpStatusCode.OK);
                resultContent?.ReservationId.Should().NotBeNull();
                resultContent?.Message.Should().Contain($"Reservation successfully created for {reservationDate}. Your reservation ID is: ");
            }

        }

        internal class CarReservation : CarControllerTests 
        {
            private TestServer _reservationTestServer;
            private HttpClient _clientTestServer;

            [SetUp]
            public void SetUp() 
            {
                _reservationTestServer = new WebApplicationFactory<Program>()
                    .WithWebHostBuilder(builder =>
                    {
                        builder.UseEnvironment("Testing");
                    })
                    .Server;

                _clientTestServer = _reservationTestServer.CreateClient();
            }

            [Test]
            public async Task Client_WhenTryingToMakeMultipleReservationsAndThereAreAvailableCars_ShouldBeAbleToMakeTheReservations()
            {
                // Assert
                var carRequest1 = new CreateCarRequestBuilder().WithMake("Maybe car to be reserved 1").Build();
                var carRequest2 = new CreateCarRequestBuilder().WithMake("Maybe car to be reserved 2").Build();

                var createCarPostContent1 = new StringContent(JsonConvert.SerializeObject(carRequest1), Encoding.UTF8, AcceptedContentType);
                var createCarPostContent2 = new StringContent(JsonConvert.SerializeObject(carRequest2), Encoding.UTF8, AcceptedContentType);

                var createCarResult1 = await GlobalHttpClient.PostAsync(EndpointBaseRoute, createCarPostContent1);
                var createCarResult2 = await GlobalHttpClient.PostAsync(EndpointBaseRoute, createCarPostContent2);

                createCarResult1.EnsureSuccessStatusCode();
                createCarResult2.EnsureSuccessStatusCode();

                var reservationDate1 = DateTime.UtcNow.AddHours(5);
                var reservationDate2 = reservationDate1.AddHours(5);

                var reservationRequest1 = new CreateReservationRequestBuilder().WithReservationDate(reservationDate1).WithDurationInMinutes(55).Build();
                var reservationRequest2 = new CreateReservationRequestBuilder().WithReservationDate(reservationDate2).WithDurationInMinutes(87).Build();

                var createReservationPostContent1 = new StringContent(JsonConvert.SerializeObject(reservationRequest1), Encoding.UTF8, AcceptedContentType);
                var createReservationPostContent2 = new StringContent(JsonConvert.SerializeObject(reservationRequest2), Encoding.UTF8, AcceptedContentType);

                // Act
                var createReservationResult1 = await GlobalHttpClient.PostAsync($"{EndpointBaseRoute}/reservations", createReservationPostContent1);
                var createReservationResult2 = await GlobalHttpClient.PostAsync($"{EndpointBaseRoute}/reservations", createReservationPostContent2);

                var resultContent1 = JsonConvert.DeserializeObject<CreateReservationResponse>(await createReservationResult1.Content.ReadAsStringAsync());
                var resultContent2 = JsonConvert.DeserializeObject<CreateReservationResponse>(await createReservationResult2.Content.ReadAsStringAsync());

                // Assert
                createReservationResult1.EnsureSuccessStatusCode();
                createReservationResult2.EnsureSuccessStatusCode();

                resultContent1?.ReservationId.Should().NotBeNull();
                resultContent1?.Message.Should().Contain($"Reservation successfully created for {reservationDate1}. Your reservation ID is: ");

                resultContent2?.ReservationId.Should().NotBeNull();
                resultContent2?.Message.Should().Contain($"Reservation successfully created for {reservationDate2}. Your reservation ID is: ");
            }

            [Test]
            public async Task Client_WhenTryingToMakeReservationButNoCarsWhereRegistered_ShouldReceiveAnErrorMessage()
            {
                // Assert
                var reservationDate = DateTime.UtcNow.AddHours(5);
                var reservationRequest = new CreateReservationRequestBuilder()
                    .WithReservationDate(reservationDate)
                    .WithDurationInMinutes(60)
                    .Build();

                var createReservationPostContent = new StringContent(JsonConvert.SerializeObject(reservationRequest), Encoding.UTF8, AcceptedContentType);

                // Act
                var createReservationResult = await _clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", createReservationPostContent);
                var resultContent = JsonConvert.DeserializeObject<CreateReservationResponse>(await createReservationResult.Content.ReadAsStringAsync());

                // Assert
                createReservationResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                resultContent?.ReservationId.Should().BeNull();
                resultContent?.Message.Should().Contain($"There's no car available for the desired date and time.");
            }

            [Test]
            public async Task Client_WhenTryingToMakeReservationButNoCarsAreAvailable_ShouldReceiveErrorMessage()
            {
                // Assert
                var carRequest = new CreateCarRequestBuilder().WithMake("First Make").Build();
                var createCarPostContent = new StringContent(JsonConvert.SerializeObject(carRequest), Encoding.UTF8, AcceptedContentType);

                var createCarResult = await _clientTestServer.PostAsync(EndpointBaseRoute, createCarPostContent);
                createCarResult.EnsureSuccessStatusCode();

                var reservationDate = DateTime.UtcNow.AddHours(5);
                var firstReservationRequest = new CreateReservationRequestBuilder()
                    .WithReservationDate(reservationDate)
                    .WithDurationInMinutes(60)
                    .Build();

                var secondReservationRequest = new CreateReservationRequestBuilder()
                    .WithReservationDate(reservationDate)
                    .WithDurationInMinutes(120)
                    .Build();

                var firstReservationPostContent = new StringContent(JsonConvert.SerializeObject(firstReservationRequest), Encoding.UTF8, AcceptedContentType);
                var secondReservationPostContent = new StringContent(JsonConvert.SerializeObject(secondReservationRequest), Encoding.UTF8, AcceptedContentType);

                var firstReservationResult = await _clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", firstReservationPostContent);
                firstReservationResult.EnsureSuccessStatusCode();

                // Act
                var secondReservationResult = await _clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", secondReservationPostContent);
                var resultContent = JsonConvert.DeserializeObject<CreateReservationResponse>(await secondReservationResult.Content.ReadAsStringAsync());

                // Assert
                secondReservationResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                resultContent?.ReservationId.Should().BeNull();
                resultContent?.Message.Should().Contain($"There's no car available for the desired date and time.");
            }

            [Test]
            public async Task Client_ShouldBeAbleToRetrieveAllUpcomingReservationsWithoutInformingTheDate()
            {
                // Assert
                var currentDate = DateTime.UtcNow;
                var dataCreated = await SetUpFourCarsAndFourReservationsForTheSameDate(currentDate.AddMinutes(6));

                // Act
                var result = await _clientTestServer.GetAsync($"{EndpointBaseRoute}/reservations");
                result.EnsureSuccessStatusCode();

                // Assert
                var allUpcomingReservations = JsonConvert.DeserializeObject<IEnumerable<ReservationResponse>>(await result.Content.ReadAsStringAsync());

                allUpcomingReservations.Should()
                    .HaveCount(dataCreated.Count())
                    .And
                    .Contain(x => dataCreated.Any(created => 
                        created.ReservationId!.Value == x.Id && created.CarId == x.CarId));
            }

            [Test]
            public async Task Client_ShouldBeAbleToRetrieveAllUpcomingReservationsUntilSpecificDate()
            {
                // Assert
                var currentDate = DateTime.UtcNow;
                var dateReservation1 = currentDate.AddMinutes(10);
                var dateReservation2 = currentDate.AddMinutes(30);
                var dateReservation3 = currentDate.AddMinutes(50);
                var dateReservation4 = currentDate.AddHours(1);
                var dateReservation5 = currentDate.AddHours(2);
                var dateReservation6 = currentDate.AddHours(5);

                var dateLimitToSearch = currentDate.AddHours(1.5);
                var expectedDatesToRetrieve = new List<DateTime>()
                {
                    dateReservation1, 
                    dateReservation2, 
                    dateReservation3, 
                    dateReservation4,
                };

                await SetUpFourCarsToBeUsed();

                var reservationResult1 = await _clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", CreateReservationRequestAsContentString(dateReservation1, 80));
                reservationResult1.EnsureSuccessStatusCode();

                var reservationResult2 = await _clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", CreateReservationRequestAsContentString(dateReservation2, 55));
                reservationResult2.EnsureSuccessStatusCode();

                var reservationResult3 = await _clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", CreateReservationRequestAsContentString(dateReservation3, 90));
                reservationResult3.EnsureSuccessStatusCode();

                var reservationResult4 = await _clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", CreateReservationRequestAsContentString(dateReservation4, 110));
                reservationResult4.EnsureSuccessStatusCode();

                var reservationResult5 = await _clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", CreateReservationRequestAsContentString(dateReservation5, 60));
                reservationResult5.EnsureSuccessStatusCode();

                var reservationResult6 = await _clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", CreateReservationRequestAsContentString(dateReservation6, 120));
                reservationResult6.EnsureSuccessStatusCode();

                // Act
                var result = await _clientTestServer.GetAsync($"{EndpointBaseRoute}/reservations?limitDate={dateLimitToSearch}");
                result.EnsureSuccessStatusCode();

                // Assert
                var allUpcomingReservationsUntilDate = JsonConvert.DeserializeObject<IEnumerable<ReservationResponse>>(await result.Content.ReadAsStringAsync());

                allUpcomingReservationsUntilDate
                    .Should()
                    .HaveCount(expectedDatesToRetrieve.Count())
                    .And
                    .Contain(upcomingReservation => expectedDatesToRetrieve.Any(date => date == upcomingReservation.InitialDate));
            }

            private async Task<List<CreateReservationResponse>> SetUpFourCarsAndFourReservationsForTheSameDate(DateTime referenceDate)
            {
                await SetUpFourCarsToBeUsed();

                var reservationPostContent1 = CreateReservationRequestAsContentString(referenceDate, 80);
                var reservationPostContent2 = CreateReservationRequestAsContentString(referenceDate, 55);
                var reservationPostContent3 = CreateReservationRequestAsContentString(referenceDate, 110);
                var reservationPostContent4 = CreateReservationRequestAsContentString(referenceDate, 120);

                var reservationResult1 = await _clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", reservationPostContent1);
                reservationResult1.EnsureSuccessStatusCode();

                var reservationResult2 = await _clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", reservationPostContent2);
                reservationResult2.EnsureSuccessStatusCode();

                var reservationResult3 = await _clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", reservationPostContent3);
                reservationResult3.EnsureSuccessStatusCode();

                var reservationResult4 = await _clientTestServer.PostAsync($"{EndpointBaseRoute}/reservations", reservationPostContent4);
                reservationResult4.EnsureSuccessStatusCode();

                var createReservationResponse1 = JsonConvert.DeserializeObject<CreateReservationResponse>(await reservationResult1.Content.ReadAsStringAsync());
                var createReservationResponse2 = JsonConvert.DeserializeObject<CreateReservationResponse>(await reservationResult2.Content.ReadAsStringAsync());
                var createReservationResponse3 = JsonConvert.DeserializeObject<CreateReservationResponse>(await reservationResult3.Content.ReadAsStringAsync());
                var createReservationResponse4 = JsonConvert.DeserializeObject<CreateReservationResponse>(await reservationResult4.Content.ReadAsStringAsync());

                return new List<CreateReservationResponse>
                {
                    createReservationResponse1!,
                    createReservationResponse2!,
                    createReservationResponse3!,
                    createReservationResponse4!
                };
            }

            private StringContent CreateReservationRequestAsContentString(DateTime reservationDate, int durationInMinutes) 
            {
                var reservationRequest = new CreateReservationRequestBuilder()
                        .WithReservationDate(reservationDate)
                        .WithDurationInMinutes(durationInMinutes)
                        .Build();

                return new StringContent(JsonConvert.SerializeObject(reservationRequest), Encoding.UTF8, AcceptedContentType);
            }

            private async Task SetUpFourCarsToBeUsed() 
            {
                var carRequest1 = new CreateCarRequestBuilder().WithMake("First Car Make").WithModel("First Car Model").Build();
                var createCarPostContent1 = new StringContent(JsonConvert.SerializeObject(carRequest1), Encoding.UTF8, AcceptedContentType);

                var carRequest2 = new CreateCarRequestBuilder().WithMake("Second Car Make").WithModel("Second Car Model").Build();
                var createCarPostContent2 = new StringContent(JsonConvert.SerializeObject(carRequest2), Encoding.UTF8, AcceptedContentType);

                var carRequest3 = new CreateCarRequestBuilder().WithMake("Third Car Make").WithModel("Third Car Model").Build();
                var createCarPostContent3 = new StringContent(JsonConvert.SerializeObject(carRequest3), Encoding.UTF8, AcceptedContentType);

                var carRequest4 = new CreateCarRequestBuilder().WithMake("Fourth Car Make").WithModel("Fourth Car Model").Build();
                var createCarPostContent4 = new StringContent(JsonConvert.SerializeObject(carRequest4), Encoding.UTF8, AcceptedContentType);

                (await _clientTestServer.PostAsync(EndpointBaseRoute, createCarPostContent1)).EnsureSuccessStatusCode();
                (await _clientTestServer.PostAsync(EndpointBaseRoute, createCarPostContent2)).EnsureSuccessStatusCode();
                (await _clientTestServer.PostAsync(EndpointBaseRoute, createCarPostContent3)).EnsureSuccessStatusCode();
                (await _clientTestServer.PostAsync(EndpointBaseRoute, createCarPostContent4)).EnsureSuccessStatusCode();
            }

            [TearDown]
            public void TearDown() 
            {
                _clientTestServer?.Dispose();
                _reservationTestServer?.Dispose();
            }            
        }       
    }
}
