using Microsoft.AspNetCore.Mvc;

internal class ApiRouteAttribute : RouteAttribute
{
    public ApiRouteAttribute(string template) 
        : base($"api/{template}")
    {
    }
}