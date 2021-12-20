using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniTools.Web.Controllers
{
    //[Route("[controller]")]
    public class UserController : Controller
    {
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
            return View();
        }

        // We need this here because we have an WebApi with exact same controller-action
        // By adding the route attribute here, we wrestle back to make this the priority
        [Route("[controller]/Add")]
        // GET: UserController/Add
        public ActionResult Add()
        {
            Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper a;
            return View();
        }

        //[Route("Create")]
        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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
    }
}
