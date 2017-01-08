using System;

namespace Gym.Data.Visits.Model
{
    public class VisitBase : IDataItem
    {
        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }
    }
}