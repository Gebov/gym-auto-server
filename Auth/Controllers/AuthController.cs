using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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
            var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
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
            var roles = await this.userManager.GetRolesAsync(user);
            
            var response = new 
            {
                username = user.UserName,
                email = user.Email,
                roles = roles
            };

            return this.Ok(response);
        }

        [HttpGet]
        [AuthorizeRolesAttribute(RoleConstants.Administrator)]
        public async Task<IActionResult> Users()
        {
            // TODO: should return 403
            var users = this.userManager.Users;
            var totalCount = users.Count();

            // TODO: optimize
            var roleMap = new Dictionary<string, IList<string>>();
            foreach (var user in users)
            {
                var roles = await this.userManager.GetRolesAsync(user);
                roleMap.Add(user.Id, roles);
            }
            
            var response = new UsersResponse(users, roleMap, totalCount);
            
            return this.Ok(response);
        }

        public class UsersResponse
        {
            public UsersResponse(IEnumerable<ApplicationUser> users, IDictionary<string, IList<string>> roles, int totalCount)
            {
                this.TotalCount = totalCount;
                this.Data = users.Select(x => new UserResponse(x, roles[x.Id]));
            }

            public IEnumerable<UserResponse> Data { get; private set; } 
            public int TotalCount { get; private set; }

            public class UserResponse
            {
                public UserResponse(ApplicationUser user, IList<string> roles)
                {
                    this.Username = user.UserName;
                    this.Email = user.Email;
                    this.Roles = roles;
                }

                public string Username { get; private set; }

                public string Email { get; private set; }

                public IList<string> Roles { get; private set; }
            }
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