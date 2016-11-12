using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Gym.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public TestController()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = new
            {
                message = "Hello world!"
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }
    }
}