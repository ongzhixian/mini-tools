using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.Models;
using MiniTools.Web.MongoEntities;
using MiniTools.Web.Services;

namespace MiniTools.Web.Controllers
{
    public class BookmarkController : Controller
    {
        private readonly ILogger<BookmarkController> logger;

        private readonly IBookmarkLinkService bookmarkLinkService;

        public BookmarkController(ILogger<BookmarkController> logger, IBookmarkLinkService bookmarkLinkService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.bookmarkLinkService = bookmarkLinkService ??
                                            throw new ArgumentNullException(nameof(bookmarkLinkService));

        }

        // GET: BookmarkController
        public async Task<ActionResult> IndexAsync([FromQuery] ushort page, ushort pageSize)
        {
            long documentCount = await bookmarkLinkService.CountTotalLinksAsync();

            //PageData<Bookmark>? result = await bookmarkLinkService.GetBookmarkListAsync(
            //    new Options.DataPageOption
            //    {
            //        PageSize = pageSize,
            //        Page = page,
            //    });

            List<Bookmark>? result = await bookmarkLinkService.GetBookmarkListAsync();

            BookmarkLinksViewModel model = new BookmarkLinksViewModel();
            model.DocumentCount = documentCount;
            model.BookmarkList = result;

            return View(model);
        }

        // GET: BookmarkController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BookmarkController/Create
        public ActionResult Create()
        {
            return View(new AddBookmarkViewModel());
        }

        // POST: BookmarkController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AddBookmarkViewModel model)
        {
            try
            {
                bookmarkLinkService.AddLinksAsync(model.BookmarkLinks);

                ModelState.Clear();

                ViewBag.SuccessMessage = "Links saved.";
                
                return View(new AddBookmarkViewModel());
            }
            catch
            {
                ViewBag.ErrorMessage = "Unable to save links.";
                return View(model);
            }
        }

        // GET: BookmarkController/Edit/5
        public ActionResult Edit(string id)
        {
            return View();
        }

        // POST: BookmarkController/Edit/5
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

        // POST: BookmarkController/Tag/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Tag(string id, string newTags)
        {
            try
            {
                bookmarkLinkService.AddTags(id, newTags);

                //if (collection.ContainsKey("newTags"))
                //{
                //    bookmarkLinkService.AddTags(id, newTags);
                //}

                return Redirect(Request.Headers["Referer"].ToString());
                // collection.ContainsKey("")
                //return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // POST: BookmarkController/RemoveTag/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveTag(string id, string tag)
        {
            try
            {
                //bookmarkLinkService.AddTags(id, newTags);

                //if (collection.ContainsKey("newTags"))
                //{
                //    bookmarkLinkService.AddTags(id, newTags);
                //}

                bookmarkLinkService.RemoveTag(id, tag);

                return Redirect(Request.Headers["Referer"].ToString());
                // collection.ContainsKey("")
                //return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET: BookmarkController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BookmarkController/Delete/5
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
    }
}
