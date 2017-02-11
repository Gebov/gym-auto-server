using System;

namespace Gym.Data.Products.Cards.Model
{
    public class Card
    {
        public Guid Id { get; set; }

        public decimal? Price { get; set; }

        public string UserId { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DayPaid { get; set; }

        public bool Paid { get; set; }
       
        public Guid TemplateId { get; set; }
    }
}