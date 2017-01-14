namespace Gym.Data.Products.Cards.Model
{
    class CardTypeResponse: CardType 
    {
        public CardTypeResponse(CardType source, bool hasCards)
        {
            Properties.Copy(source, this);
            this.HasCards = hasCards;
        }

        public bool HasCards { get; private set; }
    }
}