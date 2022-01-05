using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiniTools.Web.Controllers;

public class LogoutController : Controller
{
    // GET: LogoutController
    [AllowAnonymous]
    public async Task<ActionResult> IndexAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        TempData["SignoutFooter"] = $"You have signed out at {DateTime.Now}";

        return RedirectToAction("Index", "Home");
    }

    //// GET: LogoutController/Details/5
    //public ActionResult Details(int id)
    //{
    //    return View();
    //}

    //// GET: LogoutController/Create
    //public ActionResult Create()
    //{
    //    return View();
    //}

    //// POST: LogoutController/Create
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public ActionResult Create(IFormCollection collection)
    //{
    //    try
    //    {
    //        return RedirectToAction(nameof(IndexAsync));
    //    }
    //    catch
    //    {
    //        return View();
    //    }
    //}

    //// GET: LogoutController/Edit/5
    //public ActionResult Edit(int id)
    //{
    //    return View();
    //}

    //// POST: LogoutController/Edit/5
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public ActionResult Edit(int id, IFormCollection collection)
    //{
    //    try
    //    {
    //        return RedirectToAction(nameof(IndexAsync));
    //    }
    //    catch
    //    {
    //        return View();
    //    }
    //}

    //// GET: LogoutController/Delete/5
    //public ActionResult Delete(int id)
    //{
    //    return View();
    //}

    //// POST: LogoutController/Delete/5
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public ActionResult Delete(int id, IFormCollection collection)
    //{
    //    try
    //    {
    //        return RedirectToAction(nameof(IndexAsync));
    //    }
    //    catch
    //    {
    //        return View();
    //    }
    //}
}