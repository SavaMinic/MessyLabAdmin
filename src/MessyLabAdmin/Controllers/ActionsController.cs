using System.Linq;
using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MessyLabAdmin.Models;
using MessyLabAdmin.Util;
using System.Collections.Generic;
using System.Globalization;
using Action = MessyLabAdmin.Models.Action;

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
        public IActionResult Index(int? page, int? studentId, int? actionType, string createdFrom, string createdUntil)
        {
            IQueryable<Action> actions = _context.Actions.Include(a => a.Student);
            CultureInfo enUS = new CultureInfo("en-US");
            const string format = "dd.MM.yyyy HH:mm:ss";

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
            DateTime from;
            if (createdFrom != null && createdFrom != "" && DateTime.TryParseExact(createdFrom, format, enUS, DateTimeStyles.None, out from))
            {
                actions = actions.Where(a => a.CreatedTime >= from);
                ViewBag.createdFrom = createdFrom;
            }
            DateTime until;
            if (createdUntil != null && createdUntil != "" && DateTime.TryParseExact(createdUntil, format, enUS, DateTimeStyles.None, out until))
            {
                actions = actions.Where(a => a.CreatedTime <= until);
                ViewBag.createdUntil = createdUntil;
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
