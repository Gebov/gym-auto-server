using System;
using System.Net;
using System.Threading.Tasks;
using Gym.Auth.Controllers;
using Gym.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Gym.Core.Exceptions
{
    internal class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILoggerFactory factory)
        {
            this.next = next;
            this.logger = factory.CreateLogger("Exceptions");
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                this.logger.LogError(1, ex, ex.Message);
                await this.HandleExceptionAsync(context, ex);
            }
        }

        private async Task WriteExceptionAsync(HttpContext context, Exception exception, HttpStatusCode code)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)code;
            
            await response.WriteAsync(JsonConvert.SerializeObject(new
            {
                error = new
                {
                    message = exception.Message
                }
            })).ConfigureAwait(false);
            this.logger.LogCritical("error", exception);
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            
            if (exception is InvalidModelException || 
                exception is InvalidOperationException ||
                exception is IdentityException)
                code = HttpStatusCode.BadRequest; 

            await this.WriteExceptionAsync(context, exception, code).ConfigureAwait(false);
        }
    }
}