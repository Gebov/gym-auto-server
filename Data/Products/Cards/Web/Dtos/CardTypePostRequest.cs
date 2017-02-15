using System;
using System.ComponentModel.DataAnnotations;

namespace Gym.Data.Products.Cards.Web.Dtos
{
    public class CardTypeRequest: IPriced
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [Range(1, UInt16.MaxValue)]
        public UInt16 VisitCount { get; set; }

        [Required]
        [Range(1, byte.MaxValue)]
        public byte Validity { get; set; }

        [Required]
        [DataType(DataType.Currency)] 
        [Range(0, long.MaxValue)]  
        public decimal Price { get; set; }
    }
}