using Gym.Data.Products.Cards.Model;

namespace Gym.Data.Products.Cards.Web.Dtos
{
    class CardTypeResponse: CardTemplate 
    {
        public CardTypeResponse(CardTemplate source, bool hasCards)
        {
            Properties.Copy(source, this);
            this.HasCards = hasCards;
        }

        public bool HasCards { get; private set; }
    }
}