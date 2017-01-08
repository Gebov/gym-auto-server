using Gym.Data.Products.Cards.Model;
using Gym.Data.Users.Model;
using Gym.Data.Visits.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// using System.Reflection;

namespace Gym.Data
{
    public class GymContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Card> Cards { get; set; }

        public DbSet<CardType> CardTypes { get; set; }

        public DbSet<CardVisit> Visits { get; set; }

        public DbSet<SingleVisit> SingleVisits { get; set; }

        public GymContext(DbContextOptions<GymContext> options)
            : base(options)
        {
        }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     // var entityType = modelBuilder.Model.AddEntityType("test");
        //     // var prop = entityType.AddProperty("", typeof(int), false);
        //     // entityType.AddKey(prop);

        //     // var entityTypes = modelBuilder.Model.GetEntityTypes();

        //     // foreach (var entry in entityTypes)
        //     // {
        //     //     if (typeof(IDataItem).IsAssignableFrom(entry.ClrType)) 
        //     //     {
        //     //         var property = entry.FindProperty(nameof(IDataItem.Id));
        //     //         entry.AddKey(property);
        //     //     }
        //     // }
        // }
    }
}