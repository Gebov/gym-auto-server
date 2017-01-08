using System;
using Gym.Data.Users.Model;

namespace Gym.Data.Products.Cards.Model
{
    public class Card : IDataItem, ISellable, IOwned
    {
        public Guid Id { get; set; }

        public decimal Price { get; set; }

        public bool Paid { get; set; }

        public ApplicationUser User { get; set; }

        public DateTime DateCreated { get; set; }

        public TimeSpan Validity { get; set; }
        
        public CardType Type { get; set; }
    }
}