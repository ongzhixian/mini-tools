using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.DataEntities;
using MiniTools.Web.Models;
using MiniTools.Web.Services;

namespace MiniTools.Web.Controllers;

//[Route("[controller]")]
public class UserController : Controller
{
    private readonly ILogger<UserController> logger;
    private readonly UserApiService userApi;

    public UserController(ILogger<UserController> logger, UserApiService userApi)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userApi = userApi ?? throw new ArgumentNullException(nameof(userApi));
    }

    //[Route(Name ="UserIndex")]
    //[HttpGet("User")]
    //[Route("")]
    // GET: UserController
    public async Task<ActionResult> IndexAsync(ushort page, ushort pageSize)
    {
        // https://localhost:7001/User?page=1&pageSize=25
        
        page = (page <= 0) ? (ushort)1 : page;
        pageSize = (pageSize <= 0) ? (ushort)15 : pageSize;

        PageData<UserAccount>? userList = await userApi.GetUserListAsync(page, pageSize);

        if (userList == null)
            userList = new PageData<UserAccount>();

        PageDataViewModel<UserAccount> result = new PageDataViewModel<UserAccount>(userList);

        return View(result);
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
            Password = "SomePassword",
            //FirstName = "SomeFirstName",
            //LastName = "SomeLastName",
            //Email = "SomeUsername@local.local"
            
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
            await userApi.AddUserAsync(model);
            return RedirectToAction(nameof(IndexAsync));
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
            return RedirectToAction(nameof(IndexAsync));
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
            return RedirectToAction(nameof(IndexAsync));
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
