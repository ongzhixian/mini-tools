// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>", Scope = "namespace", Target = "~N:MiniTools.Web.Helpers.HttpMessageLogging")]
[assembly: SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped", Justification = "<Pending>", Scope = "member", Target = "~M:MiniTools.Web.Helpers.HttpMessageLogging.CustomLoggingHttpMessageHandler.SendAsync(System.Net.Http.HttpRequestMessage,System.Threading.CancellationToken)~System.Threading.Tasks.Task{System.Net.Http.HttpResponseMessage}")]
[assembly: SuppressMessage("Major Code Smell", "S112:General exceptions should never be thrown", Justification = "<Pending>", Scope = "member", Target = "~P:MiniTools.Web.Helpers.HttpMessageLogging.CustomHttpHeadersLogValue.Item(System.Int32)")]
[assembly: SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>", Scope = "member", Target = "~M:MiniTools.Web.Api.UserIdentityController.Authenticate(MiniTools.Web.Api.UserIdentityModel)~Microsoft.AspNetCore.Mvc.IActionResult")]
[assembly: SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>", Scope = "member", Target = "~M:MiniTools.Web.Controllers.AuthenticationController.Login(MiniTools.Web.Models.LoginViewModel)~Microsoft.AspNetCore.Mvc.IActionResult")]
[assembly: SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>", Scope = "type", Target = "~T:MiniTools.Web.Controllers.AuthenticationController")]
