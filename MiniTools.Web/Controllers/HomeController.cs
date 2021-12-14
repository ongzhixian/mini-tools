using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.Models;

namespace MiniTools.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    //[HttpGet(Name ="Login")]
    //[Route("/login")]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    //[Route("/login")]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        try
        {
            _logger.LogInformation(model.Username);
            _logger.LogInformation(model.Password);
            return View(model);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
