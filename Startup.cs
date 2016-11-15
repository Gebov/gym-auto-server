using System.Text;
using Microsoft.AspNetCore.Builder;

using Microsoft.IdentityModel.Tokens;
using Gym.Auth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Gym.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1
{
    public class ErrorDto
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Startup
    {
        private static readonly string secretKey = "mysupersecret_secretkey!123";
        private SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            // TODO: make configurations
            // app.UseCors("AllowSpecificOrigin");
            app.UseExceptionHandler(errorApp => 
            {
                errorApp.Run(async context => 
                {
                    context.Response.StatusCode = 500; // or another Status accordingly to Exception Type
                    context.Response.ContentType = "application/json";

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var ex = error.Error;

                        await context.Response.WriteAsync(new ErrorDto()
                        {
                            Code = 2,
                            Message = ex.Message
                        }.ToString(), Encoding.UTF8);
                    }
                });
            });

            app.UseIdentity();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "ExampleIssuer",

                ValidateAudience = true,
                ValidAudience = "ExampleAudience",

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller}/{action}");
            });

            app.UseCors("AllowFrontend");

            this.SeedDatabase(app);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options => 
            {
               options.Password.RequiredLength = 7;
               options.Password.RequireUppercase = false;
               options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<GymContext>();

            services.AddEntityFrameworkInMemoryDatabase();
            services.AddDbContext<GymContext>(x => x.UseInMemoryDatabase());
            services.AddOptions();
            services.AddCors(options =>
            {
                // TODO: fix policies
                options.AddPolicy("AllowFrontend",
                    builder => builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                                
                config.Filters.Add(new CorsAuthorizationFilterFactory("AllowFrontend"));
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = "ExampleIssuer";
                options.Audience = "ExampleAudience";
                options.SigningCredentials = new SigningCredentials(this.signingKey, SecurityAlgorithms.HmacSha256);
            });
        }

        private void SeedDatabase(IApplicationBuilder app) 
        {
            var context = app.ApplicationServices.GetRequiredService<UserManager<ApplicationUser>>();
            {
                var adminUser = new ApplicationUser()
                {
                    Email = "admin@admin.com",
                    UserName = "admin"
                };

                context.CreateAsync(adminUser, "admin@2");
            }
        }
    }
}
