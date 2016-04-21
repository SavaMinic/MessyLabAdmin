using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MessyLabAdmin.Models;
using MessyLabAdmin.Util;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System;

namespace MessyLabAdmin.Controllers
{
    public class AssignmentsController : Controller
    {
        private ApplicationDbContext _context;

        public AssignmentsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Assignments
        public IActionResult Index(int? page)
        {
            IQueryable<Assignment> assignments = _context.Assignments
                .Include(a => a.CreatedBy)
                .Include(a => a.StudentAssignments)
                .ThenInclude(sa => sa.Student);

            ViewBag.currentPage = page ?? 1;
            ViewBag.totalPages = assignments.Count() / 10 + 1;

            return View(assignments.ToPagedList(page ?? 1, 10));
        }

        // GET: Assignments/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Assignment assignment = _context.Assignments.Single(m => m.ID == id);
            if (assignment == null)
            {
                return HttpNotFound();
            }

            return View(assignment);
        }

        // GET: Assignments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Assignments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                assignment.CreatedTime = DateTime.Now;
                assignment.CreatedBy = _context.Users.Single(u => u.Id == User.GetUserId());

                _context.Assignments.Add(assignment);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(assignment);
        }

        // GET: Assignments/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Assignment assignment = _context.Assignments.Single(m => m.ID == id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        // POST: Assignments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                // properties that are not in request, are null ?!?
                // so we load them from disk
                var old = _context.Assignments.AsNoTracking()
                    .Include(a => a.CreatedBy)
                    .Single(a => a.ID == assignment.ID);
                assignment.CreatedTime = old.CreatedTime;
                assignment.CreatedBy = old.CreatedBy;

                _context.Update(assignment);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(assignment);
        }

        // GET: Assignments/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Assignment assignment = _context.Assignments.Single(m => m.ID == id);
            if (assignment == null)
            {
                return HttpNotFound();
            }

            return View(assignment);
        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Assignment assignment = _context.Assignments.Single(m => m.ID == id);
            _context.Assignments.Remove(assignment);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}