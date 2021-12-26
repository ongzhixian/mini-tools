using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.Models;
using MiniTools.Web.Services;

namespace MiniTools.Web.Controllers;

//[Route("[controller]")]
public class UserController : Controller
{
    private readonly ILogger<UserController> logger;
    private readonly UserService userService;

    public UserController(ILogger<UserController> logger, UserService userService)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    //[Route(Name ="UserIndex")]
    //[HttpGet("User")]
    //[Route("")]
    // GET: UserController
    public ActionResult Index()
    {
        return View();
    }

    //[Route("Details")]
    // GET: UserController/Details/5
    public ActionResult Details(int id)
    {
        return View();
    }

    //[Route("Create")]
    // GET: UserController/Create
    public ActionResult Create()
    {
        return View(new AddUserViewModel
        {
            Username = "SomeUsername",
            FirstName = "SomeFirstName",
            LastName = "SomeLastName",
            Email = "SomeUsername@local.local",
            Password = "SomePassword"
        });
    }

    //[Route("Create")]
    // POST: UserController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CreateAsync(AddUserViewModel model)
    {
        try
        {
            await userService.AddUserAsync(model);
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    [Route("Edit")]
    // GET: UserController/Edit/5
    public ActionResult Edit(int id)
    {
        return View();
    }

    [Route("Edit")]
    // POST: UserController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    [Route("Delete")]
    // GET: UserController/Delete/5
    public ActionResult Delete(int id)
    {
        return View();
    }

    [Route("Delete")]
    // POST: UserController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // Reminder note:
    // We need this here because we have an WebApi with exact same controller-action
    // By adding the route attribute here, we wrestle back to make this the priority
    // But this is found to poor naming.
    // Remember Web API is for REST-ful HTTP services for resources (which should use HTTP verbs) 
    // [Route("[controller]/Add")]
    // // GET: UserController/Add
    // public ActionResult Add()
    // {
    //     Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper a;
    //     return View();
    // }
}
