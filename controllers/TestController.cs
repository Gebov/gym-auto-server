using Gym.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Gym.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly GymContext _context;

        public TestController(GymContext context)
        {
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            
            this._context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            // var user = this._context.Users.First(x => x.Email == "admin@admin.com");
            // var response = new
            // {
            //     message = $"Hello {user.Email}!"
            // };

            var response = new
            {
                message = $"Hello world!"
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }
    }
}