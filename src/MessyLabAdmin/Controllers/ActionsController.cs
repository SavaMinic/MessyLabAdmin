using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MessyLabAdmin.Models;
using MessyLabAdmin.Util;
using System.Collections.Generic;

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
                ViewBag.filteredStudent = _context.Students.SingleOrDefault(s => s.ID == studentId);
            }
            if (actionType != null)
            {
                actions = actions.Where(a => (int)a.Type == actionType);
                ViewBag.filteredAction = actionType;
            }
            
            ViewBag.currentPage = page ?? 1;
            ViewBag.totalPages = actions.Count() / 10 + 1;
            ViewBag.allActionTypes = GetAllActionTypes();

            return View(actions.ToPagedList(page ?? 1, 10));
        }

        private IEnumerable<SelectListItem> GetAllActionTypes()
        {
            var ret = new List<SelectListItem>();
            foreach (var item in System.Enum.GetValues(typeof(Action.ActionType)).Cast<Action.ActionType>())
            {
                ret.Add(new SelectListItem() { Value = ((int)item).ToString(), Text = item.ToString()});
            }
            return ret;
        }

    }
}
