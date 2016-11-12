using System.Text;
using Microsoft.AspNetCore.Builder;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Gym.Auth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using System;
using Microsoft.AspNetCore.Mvc.Cors.Internal;

namespace WebApplication1
{
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

            app.UseMvc();
            app.UseCors("AllowFrontend");
        }

        public void ConfigureServices(IServiceCollection services)
        {
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

            // services.AddMvc(config =>
            // {
            //     var policy = new AuthorizationPolicyBuilder()
            //                     .RequireAuthenticatedUser()
            //                     .Build();

            //     config.Filters.Add(new AuthorizeFilter(policy));
            // });
        }
    }
}
