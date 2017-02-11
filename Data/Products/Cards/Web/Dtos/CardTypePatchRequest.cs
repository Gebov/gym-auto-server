using System.ComponentModel.DataAnnotations;

namespace Gym.Data.Products.Cards.Web.Dtos
{
    public class CardTypePatchRequest: IArchived
    {
        [Required]
        public bool Archived { get; set; }
    }
}