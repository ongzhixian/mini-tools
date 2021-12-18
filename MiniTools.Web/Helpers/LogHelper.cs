using Microsoft.AspNetCore.Mvc;

namespace MiniTools.Web.Helpers;

// public static class LogHelper
// {
//     private class On
//     {
//         internal static EventId NEW = new EventId(1, "New");
//         internal static EventId VIEW = new EventId(100, "View");
//         // internal static EventId VIEW_LOGIN = new EventId(101, "View login");
//         // internal static EventId VIEW_LOGIN_POST = new EventId(102, "View login (POST)");
//     }
//     public void LogReturnView(ILogger logger)
//     {
//         logger.LogInformation(On.VIEW, "{onEvent} - Controller [{controller}], Action [{action}]", 
//         On.VIEW, 
//         ControllerContext.ActionDescriptor.ControllerName, 
//         ControllerContext.ActionDescriptor.ActionName);
//     }
// }