using System.ComponentModel.DataAnnotations;

namespace Gym.Data.Products.Cards.Web.Dtos
{
    public class CardRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public bool Paid { get; set; }

        [Required]
        public string TemplateId { get; set; }

        public decimal? Price { get; set; }
    }
}