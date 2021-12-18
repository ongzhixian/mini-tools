
using Microsoft.AspNetCore.Mvc.Filters;

namespace MiniTools.Web.ActionFilters;

// public class LogActionFilterAttribute : ActionFilterAttribute
// {
//     private class On
//     {
//         internal static EventId NEW = new EventId(1, "New");
//         internal static EventId VIEW = new EventId(100, "View");
//         // internal static EventId VIEW_LOGIN = new EventId(101, "View login");
//         // internal static EventId VIEW_LOGIN_POST = new EventId(102, "View login (POST)");
//     }

//     // private readonly ILogger<LogActionFilterAttribute> logger;
//     // public LogActionFilterAttribute(ILogger<LogActionFilterAttribute> logger)
//     // {
//     //     this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
//     //     logger.LogInformation(On.NEW, "{onEvent}", On.NEW);
//     // }
//     public override void OnActionExecuting(ActionExecutingContext filterContext)
//     {
//         ILogger<LogActionFilterAttribute> logger = 
//             filterContext.HttpContext.RequestServices.GetService<ILogger<LogActionFilterAttribute>>();

//         // filterContext.ActionDescriptor.
//         var controller = filterContext.RouteData.Values["Controller"];
//         var action = filterContext.RouteData.Values["Action"];

//         // var controller = filterContext.RequestContext.RouteData.Values["Controller"];
//         // var action = filterContext.RequestContext.RouteData.Values["Action"];

//         //
//         // Perform logging here
//         //
//         if (logger != null)
//             logger.LogInformation(On.VIEW, "XXXXXXXX {onEvent} -- on action executing {controller}, {action}", On.VIEW, controller, action);

//         base.OnActionExecuting(filterContext);
//     }
// }
