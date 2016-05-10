using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MessyLabAdmin.Models;
using System;
using Microsoft.AspNet.Authorization;

namespace MessyLabAdmin.Controllers
{
    [Authorize]
    public class SolutionsController : Controller
    {
        private ApplicationDbContext _context;

        public SolutionsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Solutions/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Solution solution = _context.Solutions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .Single(m => m.ID == id);
            if (solution == null)
            {
                return HttpNotFound();
            }

            return View(solution);
        }

        // POST: Solutions/Create
        [HttpPost]
        public IActionResult Create(Solution solution)
        {
            if (ModelState.IsValid)
            {
                solution.CreatedTime = DateTime.Now;
                _context.Solutions.Add(solution);
                _context.SaveChanges();
                return Ok(new { success = "ok" });
            }
            return HttpBadRequest(new { error = "invalid data" });
        }

        // POST: Solutions/Edit/5
        [HttpPost]
        public IActionResult Edit(Solution solution)
        {
            if (ModelState.IsValid)
            {
                solution.CreatedTime = DateTime.Now;
                _context.Update(solution);
                _context.SaveChanges();
                return Ok(new { success = "ok" });
            }
            return HttpBadRequest(new { error = "invalid data" });
        }

        public IActionResult ToggleEvaluated(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Solution solution = _context.Solutions.Single(m => m.ID == id);
            if (solution == null)
            {
                return HttpNotFound();
            }

            solution.IsEvaluated = !solution.IsEvaluated;
            _context.Update(solution);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }
    }
}
