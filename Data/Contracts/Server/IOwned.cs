using Gym.Data.Users.Model;

namespace Gym.Data
{
    interface IOwned
    {
         ApplicationUser User { get; set; }
    }
}