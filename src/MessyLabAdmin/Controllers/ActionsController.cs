using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MessyLabAdmin.Models;
using MessyLabAdmin.Util;

namespace MessyLabAdmin.Controllers
{
    public class ActionsController : Controller
    {
        private ApplicationDbContext _context;

        public ActionsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Actions
        public IActionResult Index(int? page)
        {
            var actions = _context.Actions.Include(a => a.Student);

            ViewData["currentPage"] = page ?? 1;
            ViewData["totalPages"] = actions.Count() / 10 + 1;

            return View(actions.ToPagedList(page ?? 1, 10));
        }

        // POST: Actions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Action action)
        {
            if (ModelState.IsValid)
            {
                _context.Actions.Add(action);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "Student", action.StudentID);
            return RedirectToAction("Index");
        }

    }
}
