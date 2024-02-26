using System.Reflection; // Done without Mvc library due to incompatibility with Blazor WebAssembly

namespace Doorfail.Core.Client;

public class RouteInformation
{
    public string RouteTemplate { get; set; }
    public string[] HttpMethods { get; set; }
}

public static class RouteReader
{
    public static RouteInformation GetRouteInformation(Type controllerType, string actionName)
    {
        var routeInfo = new RouteInformation();

        // Get route template from the controller level attributes
        var routeAttribute = controllerType.GetCustomAttributes()
            .FirstOrDefault(w => w.TypeId is not null);//<Microsoft.AspNetCore.Mvc.RouteAttribute>();
        if(routeAttribute != null)
            routeInfo.RouteTemplate = (string)routeAttribute!.GetType().GetProperty("Template")?.GetValue(routeAttribute);

        // Get HTTP methods and route template from action method attributes
        var actionMethod = controllerType.GetMethod(actionName);
        if(actionMethod == null)
        { return routeInfo; }

        var httpMethods = actionMethod.GetCustomAttributes()
                .Where(w => w.TypeId is not null)//HttpMethodAttribute
                .Select(attr => attr.GetType().GetProperty("HttpMethods")?.GetValue(attr))
                .FirstOrDefault();
        if(httpMethods != null)
            routeInfo.HttpMethods = Array.Empty<string>();//httpMethods.ToArray();

        var httpRouteAttribute = actionMethod.GetCustomAttributes()
            .FirstOrDefault(w => w.TypeId is not null);//<Microsoft.AspNetCore.Mvc.RouteAttribute>();
        if(httpRouteAttribute != null)
            routeInfo.RouteTemplate = (string)routeAttribute!.GetType().GetProperty("Template")?.GetValue(routeAttribute);

        return routeInfo;
    }
}