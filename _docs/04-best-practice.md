# Best Practices

## Status pages

`UseStatusCodePages` with a lambda isn't typically used in production because it returns a message that isn't useful to users.

`app.UseStatusCodePagesWithRedirects("/StatusCode/{0}");`
- Sends HTTP 302
- Redirects the client to the error handling endpoint provided in the URL template. 
  The error handling endpoint typically displays error information and returns HTTP 200.
Commonly used when the app

    1.  Should redirect the client to a different endpoint, usually in cases where a different app processes the error. For web apps, the client's browser address bar reflects the redirected endpoint.
    2.  Shouldn't preserve and return the original status code with the initial redirect response.

`app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");`
- Returns the original status code to the client.
- Generates the response body by re-executing the request pipeline using an alternate path.
Commonly used when the app:
1.  Should process the request without redirecting to a different endpoint. 
    For web apps, the client's browser address bar reflects the originally requested endpoint.
2.  Preserve and return the original status code with the response.

Original request url in browser url bar: https://localhost:7001/authentication/login
app.UseStatusCodePages              -- original status code ; HTTP status text  ; url unchanged               ; Minimal
app.UseStatusCodePagesWithRedirects -- return HTTP 302      ; custom page       ; https://.../http-status/404 ; Redirect
app.UseStatusCodePagesWithReExecute -- original status code ; custom page       ; url unchanged               ; Custom


See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-6.0#usestatuscodepages

See: https://www.c-sharpcorner.com/article/global-error-handling-in-asp-net-core-5/

See: https://www.c-sharpcorner.com/article/exception-handling-3-in-asp-net-core-mvc/




# References

ASP.NET Core Performance Best Practices
https://docs.microsoft.com/en-us/aspnet/core/performance/performance-best-practices?view=aspnetcore-6.0