using System;
using System.ComponentModel.DataAnnotations;

namespace Gym.Data.Products.Cards.Model
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
        public decimal Price { get; set; }
    }
}