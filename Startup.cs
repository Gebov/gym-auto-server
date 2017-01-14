using Microsoft.AspNetCore.Builder;
using Gym.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Gym.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Gym.Mvc;
using Gym.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Gym.Data.Users.Model;
using Gym.Data.Products.Cards.Model;
using System;

namespace Gym
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(true);

            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));

            app.UseIdentity();

            app.UseMvc(routes =>
            {
                // routes.MapRoute(
                //     name: "default",
                //     template: "api/{controller}/{action}");

                // routes.MapRoute(
                //     name: "api",
                //     template: "api/{controller}");
            });

            app.UseCors("AllowFrontend");

            this.SeedDatabase(app);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // TODO: should be configured for production
                options.Password.RequiredLength = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;

                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;

                options.Cookies.ApplicationCookie.AutomaticChallenge = false;
            }).AddEntityFrameworkStores<GymContext>();

            services.AddEntityFrameworkInMemoryDatabase();
            services.AddDbContext<GymContext>(x => x.UseInMemoryDatabase());
            services.AddOptions();
            services.AddCors(options =>
            {
                // TODO: fix policies
                options.AddPolicy("AllowFrontend",
                    builder => builder.WithOrigins("http://localhost:3000", "http://localhost").AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();

                config.Filters.Add(new CorsAuthorizationFilterFactory("AllowFrontend"));
                config.Filters.Add(new AuthorizeFilter(policy));
                config.Filters.Add(new ValidateModelFilter());
                config.Conventions.Add(new FromBodyModelBindingConvention());
            });
        }

        private void SeedDatabase(IApplicationBuilder app)
        {
            var roleManager = app.ApplicationServices.GetRequiredService<RoleManager<IdentityRole>>();
            var rolesArr = new[] { RoleConstants.Administrator, RoleConstants.Teacher, RoleConstants.Student };

            foreach (var roleName in rolesArr)
            {
                var roleObj = new IdentityRole(roleName);
                roleManager.CreateAsync(roleObj).Wait();
            }

            var userManager = app.ApplicationServices.GetRequiredService<UserManager<ApplicationUser>>();

            var adminUser = new ApplicationUser()
            {
                Email = "admin@admin.com",
                UserName = "admin"
            };

            userManager.CreateAsync(adminUser, "admin@2").Wait();

            var teacherUser = new ApplicationUser()
            {
                Email = "teacher@admin.com",
                UserName = "teacher"
            };

            userManager.CreateAsync(teacherUser, "admin@2").Wait();


            var studentUser = new ApplicationUser()
            {
                Email = "student@admin.com",
                UserName = "student"
            };

            userManager.CreateAsync(studentUser, "admin@2").Wait();

            userManager.AddToRoleAsync(adminUser, RoleConstants.Administrator).Wait(); // admin
            userManager.AddToRoleAsync(teacherUser, RoleConstants.Teacher).Wait();
            userManager.AddToRoleAsync(studentUser, RoleConstants.Student).Wait();


            var cardTypeRegular = new CardType()
            {
                Title = "Regular",
                VisitCount = 15,
                Price = 100
            };

            var dbContext = app.ApplicationServices.GetRequiredService<GymContext>();

            dbContext.CardTypes.Add(cardTypeRegular);

            var cardTypesChildren = new CardType()
            {
                Title = "Children",
                VisitCount = 15,
                Price = 100
            };

            dbContext.CardTypes.Add(cardTypesChildren);

            var card = new Card()
            {
                User = studentUser,
                Type = cardTypesChildren,
                DateCreated = DateTime.UtcNow,
                Validity = TimeSpan.FromDays(30 * 3)
            };

            dbContext.Cards.Add(card);

            dbContext.SaveChanges();
        }
    }
}
