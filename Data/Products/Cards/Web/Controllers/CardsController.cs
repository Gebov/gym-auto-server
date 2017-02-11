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
        public async Task<IActionResult> Get()
        {
            var cards = this.context.Cards.ToList();
            var responseList = new List<CardResponse>();
            foreach (var card in cards)
            {
                var template = this.context.CardTemplates.FirstOrDefault(x => x.Id == card.TemplateId);
                var user = this.context.Users.FirstOrDefault(x => x.Id == card.UserId);
                var visits = this.context.Visits.Where(x => x.CardId == card.Id).ToList();

                var dto = new CardResponse(card, visits, template, user);    
                responseList.Add(dto);
            }
            
            var response = new CollectionResponse<CardResponse>(responseList, responseList.Count);

            return await Task.FromResult(this.Ok(response));
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var card = this.context.Cards.FirstOrDefault(x => x.Id == id);
            if (card == null) 
                return this.NotFound();

            this.context.Remove(card);
            await this.context.SaveChangesAsync();

            return this.NoContent();         
        }

        [HttpPost]
        public async Task<IActionResult> Post(CardRequest data)
        {
            var card = new Card();
            Properties.Copy(data, card);
            card.DateCreated = DateTime.UtcNow;

            if (card.Paid)
                card.DayPaid = DateTime.UtcNow;

            this.context.Add(card);
            
            await this.context.SaveChangesAsync();

            var template = this.context.CardTemplates.FirstOrDefault(x => x.Id == card.TemplateId);
            var user = this.context.Users.FirstOrDefault(x => x.Id == card.UserId);
            var visits = this.context.Visits.Where(x => x.CardId == card.Id).ToList();

            var dto = new CardResponse(card, visits, template, user);
            
            return this.Created(this.GetLocation(this.Request, dto), dto);
        }

        private Uri GetLocation(HttpRequest request, CardResponse item)
        {
            var url = $"{request.Scheme}://{request.Host}{request.Path}/{item.Id}";
            
            return new Uri(url);
        }
    }
}