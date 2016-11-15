using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Gym.Auth.Model;
using Microsoft.AspNetCore.Identity;

namespace WebApiJwtAuthDemo.Controllers
{
    // [Route("api/[controller]")]
    public class JwtController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly SignInManager<ApplicationUser> signInManager;
        
        private readonly UserManager<ApplicationUser> userManager;
        public JwtController(
            IOptions<JwtIssuerOptions> jwtOptions, 
            ILoggerFactory loggerFactory, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager)
        {
            this._jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            this._logger = loggerFactory.CreateLogger<JwtController>();
            
            this._serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginViewModel applicationUser)
        {
            var identity = await this.GetClaimsIdentity(applicationUser);
            if (identity == null)
            {
                // _logger.LogInformation($"Invalid username ({applicationUser.Email}) or password ({applicationUser.Password})");
                return base.BadRequest("Invalid credentials");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64)
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Serialize and return the response
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }

        private void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        /// <summary>
        /// IMAGINE BIG RED WARNING SIGNS HERE!
        /// You'd want to retrieve claims through your claims provider
        /// in whatever way suits you, the below is purely for demo purposes!
        /// </summary>
        private async Task<ClaimsIdentity> GetClaimsIdentity(LoginViewModel model)
        {
            var appUser = await this.userManager.FindByEmailAsync(model.Email);
            if (appUser == null)
            {
                appUser = new ApplicationUser()
                {
                    Email = model.Email,
                    UserName = model.Email
                };

                var createResult = await this.userManager.CreateAsync(appUser, model.Password);
                if (!createResult.Succeeded) 
                {
                    throw new InvalidOperationException($"Unable to create user - {createResult.Errors.First().Description}");
                }
            };
            
            var result = await this.signInManager.PasswordSignInAsync(appUser, model.Password, false, false);
            if (!result.Succeeded)
                return null;

            return (ClaimsIdentity)this.User.Identity;
        }
    }
}