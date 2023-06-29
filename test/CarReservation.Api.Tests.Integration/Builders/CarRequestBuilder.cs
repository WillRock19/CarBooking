using CarReservation.Api.Models.DTO.Request;

namespace CarReservation.Api.Tests.Integration.Builders
{
    internal class CarRequestBuilder
    {
        private string? model;
        private string? make;

        internal CarRequestBuilder WithModel(string model)
        {
            this.model = model;
            return this;
        }

        internal CarRequestBuilder WithMake(string make)
        {
            this.make = make;
            return this;
        }

        internal CarRequest Build() => new() 
        {
            Make = make ?? "Some make", 
            Model = model ?? "Some model"
        };
    }
}
