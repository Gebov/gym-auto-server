namespace Gym.Data
{
    interface ISellable : IPriced
    {
        bool Paid { get; set; }
    }
}