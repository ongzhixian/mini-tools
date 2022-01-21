using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.Models;
using MiniTools.Web.Services;

namespace MiniTools.Web.Controllers
{
    public class NoteController : Controller
    {
        private readonly ILogger<NoteController> logger;

        private readonly INoteService noteService;

        public NoteController(ILogger<NoteController> logger, INoteService noteService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.noteService = noteService ??
                                            throw new ArgumentNullException(nameof(noteService));

        }

        // GET: NoteController
        public ActionResult Index()
        {
            List<MongoEntities.Note>? notes = noteService.GetNotes();
            return View(notes);
        }

        // GET: NoteController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NoteController/Create
        public ActionResult Create()
        {
            return View(new AddNoteViewModel());
        }

        // POST: NoteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AddNoteViewModel model)
        {
            try
            {

                noteService.AddNote(model.Content);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NoteController/Edit/5
        public ActionResult Edit(string id)
        {
            var note = noteService.GetNotes(id);

            return View(note);
        }

        // POST: NoteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, AddNoteViewModel model)
        {
            try
            {
                noteService.UpdateNote(id, model.Content);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NoteController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NoteController/Delete/5
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
