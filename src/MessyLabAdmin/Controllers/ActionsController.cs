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
        public IActionResult Index(int? page, int? studentId, int? actionType)
        {
            IQueryable<Action> actions = _context.Actions.Include(a => a.Student);

            if (studentId != null)
            {
                actions = actions.Where(a => a.StudentID == studentId);
                ViewData["filteredStudent"] = _context.Students.SingleOrDefault(s => s.ID == studentId);
            }
            if (actionType != null)
            {
                actions = actions.Where(a => (int)a.Type == actionType);
                ViewData["filteredAction"] = (Action.ActionType)actionType;
            }

            ViewData["currentPage"] = page ?? 1;
            ViewData["totalPages"] = actions.Count() / 10 + 1;

            return View(actions.ToPagedList(page ?? 1, 10));
        }

    }
}
