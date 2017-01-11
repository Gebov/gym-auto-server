using System;
using System.Linq;
using System.Threading.Tasks;
using Gym.Data.Contracts.Dto;
using Gym.Data.Products.Cards.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Data.Products.Cards.Web.Controllers
{
    [AuthorizeRolesAttribute(RoleConstants.Administrator)]
    [ApiRoute("[controller]")]
    public class CardTypesController : Controller
    {
        private GymContext context;
        public CardTypesController(GymContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cards = this.context.CardTypes;
            var response = new CollectionResponse<CardType>(cards, cards.Count());

            return await Task.FromResult(this.Ok(response));
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var cardType = this.context.CardTypes.FirstOrDefault(x => x.Id == id);

            if (cardType == null)
                return this.NotFound();
            
            return this.Ok(cardType);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, CardTypePatchRequest data)
        {
            var cardType = this.context.CardTypes.FirstOrDefault(x => x.Id == id);

            if (cardType == null)
                return this.NotFound();
            
            if (data.Archived != cardType.Archived)
            {
                cardType.Archived = data.Archived;
                
                this.context.Update(cardType);
                await this.context.SaveChangesAsync();
            }           

            return this.NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Post(CardTypeRequest data)
        {
            var cardType = new CardType();
            Properties.Copy(data, cardType);
            this.context.Add(cardType);
            
            await this.context.SaveChangesAsync();
            
            return this.Created(this.GetLocation(this.Request, cardType), cardType);
        }

        private Uri GetLocation(HttpRequest request, IDataItem item)
        {
            var url = $"{request.Scheme}://{request.Host}{request.Path}/{item.Id}";
            
            return new Uri(url);
        }
    }
}