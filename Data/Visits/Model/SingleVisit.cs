namespace Gym.Data.Visits.Model
{
    public class SingleVisit : VisitBase, ISellable
    {
        public decimal Price { get; set; }

        public bool Paid { get; set; }

        public string VisitorName { get; set; }
    }
}