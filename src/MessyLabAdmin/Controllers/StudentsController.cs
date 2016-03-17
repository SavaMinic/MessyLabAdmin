using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MessyLabAdmin.Models;
using MessyLabAdmin.Util;

namespace MessyLabAdmin.Controllers
{
    public class StudentsController : Controller
    {
        private ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Students
        public IActionResult Index(int? page)
        {
            IQueryable<Student> students = _context.Students
                .Include(u => u.StudentAssignments)
                .Include(u => u.Actions);

            ViewData["currentPage"] = page ?? 1;
            ViewData["totalPages"] = students.Count() / 10  + 1;

            return View(students.ToPagedList(page ?? 1, 10));
        }

        // GET: Students/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Student student = _context.Students
                .Include(u => u.Actions)
                // need to manually initialize many-to-many link
                    .Include(u => u.StudentAssignments)
                        .ThenInclude(sa => sa.Assignment)
                    .Include(u => u.StudentAssignments)
                        .ThenInclude(sa => sa.Solution)
                .Single(m => m.ID == id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Students.Add(student);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Student student = _context.Students.Single(m => m.ID == id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Update(student);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Student student = _context.Students.Single(m => m.ID == id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Student student = _context.Students.Single(m => m.ID == id);
            _context.Students.Remove(student);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
