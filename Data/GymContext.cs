using Gym.Auth.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gym.Data
{
    public class GymContext : IdentityDbContext<ApplicationUser>
    {
        public GymContext(DbContextOptions<GymContext> options)
            : base(options)
        {
        }
    }
}