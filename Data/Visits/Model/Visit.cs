using System;

namespace Gym.Data.Visits.Model
{
    public class Visit : IDataItem
    {
        public Guid Id { get; set; }

        public Guid? CardId { get; set; }

        public DateTime DateCreated { get; set; }
    }
}