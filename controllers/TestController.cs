using Gym.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Gym.Controllers
{
    public class TestController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Date()
        {
            return this.Ok(new
            {
                DateProp = DateTime.UtcNow
            });
        }

        [HttpGet]
        public IActionResult Hello()
        {
            var message = $"Hello {this.User.Identity.Name}";
            return this.Ok(message);
        }
    }
}