using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Gym.Auth.Model;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

// TODO: should return 403
// TODO: support for paging and filtering
// TODO: optimize
// TODO: should not allow to delete the current user

namespace Gym.Auth.Controllers
{
    [AuthorizeRolesAttribute(RoleConstants.Administrator)]
    [ApiRoute("[controller]")]
    public class UsersController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        public UsersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = this.userManager.Users;
            var totalCount = users.Count();

            var roleMap = new Dictionary<string, IList<string>>();
            foreach (var user in users)
            {
                var roles = await this.userManager.GetRolesAsync(user);
                roleMap.Add(user.Id, roles);
            }

            var response = new UsersResponse(users, roleMap, totalCount);
            return this.Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await this.userManager.FindByEmailAsync(id);
            if (user == null) 
                return this.NotFound();

            var roles = await this.userManager.GetRolesAsync(user);
            var response = new UserResponse(user, roles);
            return this.Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await this.userManager.FindByEmailAsync(id);
            if (user == null) 
                return this.NotFound();

            await this.userManager.DeleteAsync(user);
            return this.NoContent();         
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(string id, UserResponse data)
        {
            var user = await this.userManager.FindByEmailAsync(id);
            if (user == null)
                return this.NotFound();

            var userRoles = await this.userManager.GetRolesAsync(user);

            foreach (var role in userRoles) 
            {
                if (!data.Roles.Contains(role)) 
                    await this.userManager.RemoveFromRoleAsync(user, role);
            }

            foreach (var role in data.Roles)
            {
                if (!userRoles.Contains(role))
                    await this.userManager.AddToRoleAsync(user, role);
            }
            
            return this.NoContent();
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
        }

        public class UserResponse
        {
            public UserResponse()
            {    
            }

            public UserResponse(ApplicationUser user, IList<string> roles)
            {
                this.Username = user.UserName;
                this.Email = user.Email;
                this.Roles = roles;
            }

            public string Username { get; set; }

            public string Email { get; set; }

            public IList<string> Roles { get; set; }
        }
    }
}