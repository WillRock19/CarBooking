using CarReservation.Api.Models.DTO.Request;

namespace CarReservation.Api.Tests.Unit.Builders.DTO
{
    internal class CreateCarRequestBuilder
    {
        private string? model;
        private string? make;

        internal CreateCarRequestBuilder WithModel(string model)
        {
            this.model = model;
            return this;
        }

        internal CreateCarRequestBuilder WithMake(string make)
        {
            this.make = make;
            return this;
        }

        internal CreateCarRequest Build() => new()
        {
            Make = make ?? "Some make",
            Model = model ?? "Some model"
        };
    }
}
