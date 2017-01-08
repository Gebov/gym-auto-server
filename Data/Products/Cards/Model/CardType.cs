using System;

namespace Gym.Data.Products.Cards.Model
{
    public class CardType : IDataItem, IPriced, IArchived
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public UInt16 VisitCount { get; set; }

        // in months
        public byte Validity { get; set; }

        public decimal Price { get; set; }

        public bool Archived { get; set; }
    }
}