using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MessyLabAdmin.Models;
using MessyLabAdmin.Util;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System;
using System.Collections.Generic;

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
            ViewBag.StudentCount = SelectStudents().Count();
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

                // create Student assignments
                var students = SelectStudents(assignment.SelectEnrollmentNumberDiv, assignment.SelectEnrollmentYear, assignment.SelectStatus);
                foreach(Student s in students)
                {
                    _context.StudentAssignments.Add(new StudentAssignment()
                    {
                        StudentID = s.ID,
                        AssignmentID = assignment.ID,
                    });
                }
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

            Assignment assignment = _context.Assignments.Include(a => a.StudentAssignments).Single(m => m.ID == id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            ViewBag.StudentAssignmentsCount = assignment.StudentAssignments.Count;
            ViewBag.SolvedStudentAssignmentsCount = assignment.StudentAssignments.Where(sa => sa.SolutionID != null).Count();
            ViewBag.StudentCount = SelectStudents(assignment.SelectEnrollmentNumberDiv, assignment.SelectEnrollmentYear, assignment.SelectStatus).Count();
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
                    .Include(a => a.StudentAssignments)
                    .Single(a => a.ID == assignment.ID);
                assignment.CreatedTime = old.CreatedTime;
                assignment.CreatedBy = old.CreatedBy;

                _context.Update(assignment);
                _context.SaveChanges();

                // delete old student assignments, without solution
                var oldStudentAssignments = _context.StudentAssignments.Where(sa => sa.AssignmentID == assignment.ID && sa.SolutionID == null);
                _context.StudentAssignments.RemoveRange(oldStudentAssignments);
                _context.SaveChanges();

                // create Student assignments (for those who don't exist)
                var students = SelectStudents(assignment.SelectEnrollmentNumberDiv, assignment.SelectEnrollmentYear, assignment.SelectStatus);
                students = students.Include(s => s.StudentAssignments);
                foreach (Student s in students)
                {
                    var existAssignement = _context.StudentAssignments.Any(sa => sa.StudentID == s.ID && sa.AssignmentID == assignment.ID);
                    if (!existAssignement)
                    {
                        _context.StudentAssignments.Add(new StudentAssignment()
                        {
                            StudentID = s.ID,
                            AssignmentID = assignment.ID,
                        });
                    }
                }
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

        [HttpGet]
        public IActionResult StudentsCount(int? EnrollmentNumberDiv, int? EnrollmentYear, int? Status)
        {
            var students = SelectStudents(EnrollmentNumberDiv, EnrollmentYear, Status);
            return Ok(students.Count());
        }

        private IQueryable<Student> SelectStudents(int? EnrollmentNumberDiv = null, int? EnrollmentYear = null, int? Status = null)
        {
            IQueryable<Student> students = _context.Students;
            if (EnrollmentNumberDiv != null && EnrollmentNumberDiv != 0)
            {
                students = students.Where(s => s.EnrollmentNumber % EnrollmentNumberDiv == 0);
            }
            if (EnrollmentYear != null)
            {
                students = students.Where(s => s.EnrollmentYear == EnrollmentYear);
            }
            if (Status != null && Status != 0)
            {
                bool shouldBeActive = Status == 1;
                students = students.Where(s => s.IsActive == shouldBeActive);
            }
            return students;
        }
    }
}
