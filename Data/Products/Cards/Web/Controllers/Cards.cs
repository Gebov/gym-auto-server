using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Data.Products.Cards.Web.Controllers
{
    [AuthorizeRolesAttribute(RoleConstants.Administrator, RoleConstants.Teacher)]
    [ApiRoute("[controller]")]
    public class CardsController : Controller
    {
        private GymContext context;
        public CardsController(GymContext context)
        {
            this.context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var card = this.context.Cards.FirstOrDefault(x => x.Id == id);

            if (card == null)
                return this.NotFound();
            
            return this.Ok(card);
        }
    }
}