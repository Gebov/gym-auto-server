using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gym.Data.Contracts.Dto;
using Gym.Data.Products.Cards.Model;
using Gym.Data.Products.Cards.Web.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Data.Products.Cards.Web.Controllers
{
    [AuthorizeRolesAttribute(RoleConstants.Administrator)]
    [ApiRoute("[controller]")]
    public class CardTemplatesController : Controller
    {
        private GymContext context;
        public CardTemplatesController(GymContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cardTypes = this.context.CardTemplates;
            var responseArr = new List<CardTypeResponse>();
            foreach (var cardType in cardTypes)
            {
                var hasCards = this.context.Cards.Any(y => y.TemplateId == cardType.Id);
                responseArr.Add(new CardTypeResponse(cardType, hasCards));
            }

            var response = new CollectionResponse<CardTemplate>(responseArr, responseArr.Count);

            return await Task.FromResult(this.Ok(response));
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var cardType = this.context.CardTemplates.FirstOrDefault(x => x.Id == id);
            var hasCards = this.context.Cards.Any(x => x.TemplateId == cardType.Id);
            if (cardType == null)
                return this.NotFound();
            
            return this.Ok(new CardTypeResponse(cardType, hasCards));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, CardTypePatchRequest data)
        {
            var cardType = this.context.CardTemplates.FirstOrDefault(x => x.Id == id);

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
            var cardType = new CardTemplate();
            Properties.Copy(data, cardType);
            this.context.Add(cardType);
            
            await this.context.SaveChangesAsync();
            
            return this.Created(this.GetLocation(this.Request, cardType), cardType);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var cardType = this.context.CardTemplates.FirstOrDefault(x => x.Id == id);
            if (cardType == null) 
                return this.NotFound();

            this.context.Remove(cardType);
            await this.context.SaveChangesAsync();

            return this.NoContent();         
        }

        private Uri GetLocation(HttpRequest request, IDataItem item)
        {
            var url = $"{request.Scheme}://{request.Host}{request.Path}/{item.Id}";
            
            return new Uri(url);
        }
    }
}