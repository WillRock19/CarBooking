using CarReservation.Api.Models.Domain;

namespace CarReservation.Api.Tests.Unit.Builders
{
    internal class CarBuilder
    {
        private string? model;
        private string? make;
        private string? id;

        internal CarBuilder WithId(string id)
        {
            this.id = id;
            return this;
        }

        internal CarBuilder WithModel(string model) 
        {
            this.model = model;
            return this;
        }

        internal CarBuilder WithMake(string make)
        {
            this.make = make;
            return this;
        }

        internal Car Build() => new(id ?? "", make ?? "Some make", model ?? "Some model");
    }
}
