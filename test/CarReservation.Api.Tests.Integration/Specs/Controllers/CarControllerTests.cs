using CarReservation.Api.Models.DTO.Response;
using CarReservation.Api.Tests.Common.Builders;
using CarReservation.Api.Tests.Integration.Config;
using FluentAssertions;
using Newtonsoft.Json;
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
    }
}
