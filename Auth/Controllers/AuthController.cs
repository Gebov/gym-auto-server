using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Gym.Auth.Model;
using Microsoft.AspNetCore.Identity;
using Gym.Mvc.Filters;
using System.Security;
using System.Collections.Generic;

namespace Gym.Auth.Controllers
{
    [ValidateModel]
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var appUser = await this.userManager.FindByEmailAsync(model.Email);
            if (appUser == null)
                throw new InvalidOperationException("Wrong email or password");

            var result = await this.signInManager.PasswordSignInAsync(appUser, model.Password, false, false);
            if (!result.Succeeded)
                throw new InvalidOperationException("Wrong email or password");

            return this.Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();

            return this.Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var createResult = await this.userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
                throw new IdentityException("Unable to register user.", createResult.Errors);

            await this.signInManager.SignInAsync(user, false);

            return this.Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Current()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            
            var response = new 
            {
                username = user.UserName,
                email = user.Email
            };

            return this.Ok(response);
        }
    }

    internal class IdentityException : SecurityException
    {
        private IEnumerable<IdentityError> errors;
        public IdentityException(string message, IEnumerable<IdentityError> errors)
            : base(message)
        {
            this.errors = errors;
        }
    }
}