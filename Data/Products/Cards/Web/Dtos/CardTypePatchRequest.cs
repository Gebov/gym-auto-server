using System.ComponentModel.DataAnnotations;

namespace Gym.Data.Products.Cards.Model
{
    public class CardTypePatchRequest: IArchived
    {
        [Required]
        public bool Archived { get; set; }
    }
}