using CarReservation.Api.Interfaces;
using CarReservation.Api.Models.Domain;

namespace CarReservation.Api.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private int LatestRegisterCount;
        private readonly Dictionary<int, Reservation> Database;

        public ReservationRepository()
        {
            LatestRegisterCount = 0;
            Database = new();
        }

        public int Add(Reservation entity) 
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var entityId = LatestRegisterCount + 1;

            Database.Add(entityId, entity with { Id = entityId });
            LatestRegisterCount = entityId;

            return entityId;
        }

        public Reservation? GetById(int reservationId)
        {
            Database.TryGetValue(reservationId, out var entity);
            return entity;
        }

        public IEnumerable<string> FindCarsReservedInInterval(DateTime reservationBeginning, DateTime reservationEnding) =>
             Database.Values
                     .Where(x => reservationBeginning <= x.InitialDate && x.EndDate <= reservationEnding)
                     .Select(x => x.CarId);  
    }
}
