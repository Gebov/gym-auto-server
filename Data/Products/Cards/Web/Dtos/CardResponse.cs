using System;
using System.Collections.Generic;
using Gym.Data.Products.Cards.Model;
using Gym.Data.Users.Model;
using Gym.Data.Visits.Model;

namespace Gym.Data.Products.Cards.Web.Dtos
{
    class VisitResponse
    {
        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }
    }

    class CardResponse
    {
        public Guid Id { get; set; }

        public decimal Price { get; set; }

        public ShortEntityResponse User { get; set; }

        public DateTime DateCreated { get; set; }

        public bool Paid { get; set; }
       
        public ShortEntityResponse Template { get; set; }

        public List<VisitResponse> Visits { get; set; }

        public CardResponse(Card source, IEnumerable<Visit> visits, CardTemplate template, ApplicationUser user)
        {
            this.Id = source.Id;
            if (source.Price == null && template != null)
                this.Price = template.Price;
            else if (source.Price != null)
                this.Price = source.Price.Value;
            
            if (user != null)
            {
                this.User = new ShortEntityResponse()
                {
                    Id = user.Id,
                    Title = user.Email
                };
            }

            this.DateCreated = source.DateCreated;
            this.Paid = source.Paid;
            if (template != null)
            {
                this.Template = new ShortEntityResponse()
                {
                    Id = template.Id.ToString(),
                    Title = template.Title
                };
            }

            if (visits != null)
            {
                this.Visits = new List<VisitResponse>();
                foreach (var entry in visits)
                {
                    this.Visits.Add(new VisitResponse()
                    {
                        Id = entry.Id,
                        DateCreated = entry.DateCreated
                    });
                }
            }
        }
    }

    class ShortEntityResponse
    {
        public string Id { get; set; }

        public string Title { get; set; }
    }
}