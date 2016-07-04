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
using Microsoft.AspNet.Authorization;

namespace MessyLabAdmin.Controllers
{
    [Authorize]
    public class AssignmentsController : Controller
    {
        private ApplicationDbContext _context;

        public enum AssignmentStatus
        {
            Default = 0,
            Active,
            Inactive,
            NotStarted,
            Expired,
        };

        public AssignmentsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Assignments
        public IActionResult Index(int? page, string filteredTitle, DateTime? createdFrom, DateTime? createdUntil, AssignmentStatus status, string createdById)
        {
            IQueryable<Assignment> assignments = _context.Assignments
                .Include(a => a.CreatedBy)
                .Include(a => a.StudentAssignments)
                .ThenInclude(sa => sa.Student)
                .Include(a => a.AssignmentVariants)
                .ThenInclude(av => av.AssignmentTests);

            if (filteredTitle != null && filteredTitle != "")
            {
                assignments = assignments.Where(a => a.Title.Contains(filteredTitle));
                ViewBag.filteredTitle = filteredTitle;
            }
            if (createdFrom != null)
            {
                assignments = assignments.Where(a => a.CreatedTime >= createdFrom);
                ViewBag.createdFrom = createdFrom;
            }
            if (createdUntil != null)
            {
                assignments = assignments.Where(a => a.CreatedTime <= createdUntil);
                ViewBag.createdUntil = createdUntil;
            }
            if (createdById != null && createdById != "")
            {
                assignments = assignments.Where(a => a.CreatedBy.Id == createdById);
                ViewBag.createdById = createdById;
            }
            if (status != AssignmentStatus.Default)
            {
                switch(status)
                {
                    case AssignmentStatus.Active:
                        assignments = assignments.Where(a => a.IsActive && a.StartTime <= DateTime.Now && a.EndTime >= DateTime.Now);
                        break;
                    case AssignmentStatus.Inactive:
                        assignments = assignments.Where(a => !a.IsActive);
                        break;
                    case AssignmentStatus.NotStarted:
                        assignments = assignments.Where(a => a.IsActive && a.StartTime > DateTime.Now);
                        break;
                    case AssignmentStatus.Expired:
                        assignments = assignments.Where(a => a.IsActive && a.EndTime < DateTime.Now);
                        break;
                }
                ViewBag.status = status;
            }

            ViewBag.currentPage = page ?? 1;
            ViewBag.totalPages = assignments.Count() / 10 + 1;
            ViewBag.allStatusTypes = GetAllStatusType();
            ViewBag.allCreatedByUsers = GetAllCreatedByUsers();

            return View(assignments.ToPagedList(page ?? 1, 10));
        }

        // GET: Assignments/Details/5
        public IActionResult Details(int? id, int? page)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Assignment assignment = _context.Assignments
                .Include(a => a.CreatedBy)
                .Include(a => a.StudentAssignments)
                .Include(a => a.AssignmentVariants)
                .ThenInclude(v => v.AssignmentTests)
                .Single(m => m.ID == id);

            if (assignment == null)
            {
                return HttpNotFound();
            }

            // students for this assignment are paged
            IQueryable<StudentAssignment> studentAssignments = _context.StudentAssignments
                .Include(sa => sa.Solution)
                .Include(sa => sa.Student)
                .Where(sa => sa.AssignmentID == id);

            ViewBag.currentPage = page ?? 1;
            ViewBag.totalPages = studentAssignments.Count() / 10 + 1;
            ViewBag.studentAssignments =  studentAssignments.ToPagedList(page ?? 1, 10);

            return View(assignment);
        }

        // GET: Assignments/Tests/5
        public IActionResult Tests(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Assignment assignment = _context.Assignments
                .Include(a => a.CreatedBy)
                .Include(a => a.StudentAssignments)
                .Include(a => a.AssignmentVariants)
                .ThenInclude(v => v.AssignmentTests)
                .Single(m => m.ID == id);

            if (assignment == null)
            {
                return HttpNotFound();
            }

            return View(assignment);
        }

        // POST: Assignments/Tests/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Tests(int? id, ICollection<AssignmentVariant> variants)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Assignment assignment = _context.Assignments.Single(a => a.ID == id);
            if (assignment == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                foreach (var variant in variants)
                {
                    if (variant.AssignmentTests == null)
                    {
                        variant.AssignmentTests = new List<AssignmentTest>();
                    }

                    var currentTests = variant.AssignmentTests.ToList();
                    var oldTests = _context.AssignmentTests.AsNoTracking().Where(t => t.AssignmentVariantID == variant.ID).ToList();

                    var newTests = new List<AssignmentTest>();
                    var updatedTests = new List<AssignmentTest>();
                    // go through each current test
                    foreach(var test in currentTests)
                    {
                        // if it exists in the old tests, then it needs to be updated
                        if (oldTests.Remove(test))
                        {
                            // add it to updated list
                            updatedTests.Add(test);
                        }
                        // else create it
                        else newTests.Add(test);
                    }
                    // at the end, oldTests will have only tests that are discarded
                    //TODO: izgleda kao da se nesto id-jevi pomesaju
                    // aaa, bbb -> brisemo aaa, dodamo ccc -> save -> ccc, bbb
                    // trebalo bi da bbb zadrzi isti id, al ne zadrzi ?!?
                    _context.AddRange(newTests);
                    _context.UpdateRange(updatedTests);
                    _context.RemoveRange(oldTests);
                }
                _context.SaveChanges();
                return RedirectToAction("Details", new { id = id });
            }
            return View(id);
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
        public IActionResult Create(Assignment assignment, string[] AssignmentVariants)
        {
            if (ModelState.IsValid)
            {
                assignment.CreatedTime = DateTime.Now;
                assignment.CreatedBy = _context.Users.Single(u => u.Id == User.GetUserId());

                _context.Assignments.Add(assignment);
                foreach(var variantText in AssignmentVariants)
                {
                    var variant = new AssignmentVariant() { Text = variantText };
                    _context.Add(variant);
                    assignment.AssignmentVariants.Add(variant);
                }
                _context.SaveChanges();

                // create Student assignments
                var students = SelectStudents(assignment);
                foreach(Student s in students)
                {
                    _context.StudentAssignments.Add(new StudentAssignment()
                    {
                        StudentID = s.ID,
                        AssignmentID = assignment.ID,
                        AssignmentVariantIndex = s.EnrollmentNumber % assignment.SelectEnrollmentNumberModulo
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

            Assignment assignment = _context.Assignments
                .Include(a => a.StudentAssignments)
                .Include(a => a.AssignmentVariants)
                .Single(m => m.ID == id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            ViewBag.StudentAssignmentsCount = assignment.StudentAssignments.Count;
            ViewBag.SolvedStudentAssignmentsCount = assignment.StudentAssignments.Where(sa => sa.SolutionID != null).Count();
            ViewBag.StudentCount = SelectStudents(assignment).Count();
            return View(assignment);
        }

        // POST: Assignments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Assignment assignment, string[] AssignmentVariants)
        {
            if (ModelState.IsValid)
            {
                // properties that are not in request, are null ?!?
                // so we load them from disk
                var old = _context.Assignments.AsNoTracking()
                    .Include(a => a.CreatedBy)
                    .Include(a => a.StudentAssignments)
                    .Include(a => a.AssignmentVariants)
                    .Single(a => a.ID == assignment.ID);
                assignment.CreatedTime = old.CreatedTime;
                assignment.CreatedBy = old.CreatedBy;

                _context.RemoveRange(old.AssignmentVariants);
                foreach (var variantText in AssignmentVariants)
                {
                    var variant = new AssignmentVariant() { Text = variantText };
                    _context.Add(variant);
                    assignment.AssignmentVariants.Add(variant);
                }
                _context.Update(assignment);
                _context.SaveChanges();

                // delete old student assignments, without solution
                var oldStudentAssignments = _context.StudentAssignments.Where(sa => sa.AssignmentID == assignment.ID && sa.SolutionID == null);
                _context.StudentAssignments.RemoveRange(oldStudentAssignments);
                _context.SaveChanges();

                // create Student assignments (for those who don't exist)
                var students = SelectStudents(assignment);
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
                            AssignmentVariantIndex = s.EnrollmentNumber % assignment.SelectEnrollmentNumberModulo
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
        public IActionResult StudentsCount(int? EnrollmentYear, int? Status)
        {
            var students = SelectStudents(EnrollmentYear, Status);
            return Ok(students.Count());
        }

        private IQueryable<Student> SelectStudents(Assignment assignment)
        {
            return SelectStudents(
                assignment.SelectEnrollmentYear,
                assignment.SelectStatus
            );
        }

        private IQueryable<Student> SelectStudents(int? EnrollmentYear = null, int? Status = null)
        {
            IQueryable<Student> students = _context.Students;
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

        private string GetStatusTypeTitle(AssignmentStatus status)
        {
            switch(status)
            {
                case AssignmentStatus.Active: return "Aktivan";
                case AssignmentStatus.Inactive: return "Neaktivan";
                case AssignmentStatus.NotStarted: return "Nije započet";
                case AssignmentStatus.Expired: return "Istekao";
            }
            return "Svi";
        }

        private IEnumerable<SelectListItem> GetAllStatusType()
        {
            var ret = new List<SelectListItem>();
            foreach (var item in System.Enum.GetValues(typeof(AssignmentStatus)).Cast<AssignmentStatus>())
            {
                ret.Add(new SelectListItem() { Value = ((int)item).ToString(), Text = GetStatusTypeTitle(item) });
            }
            return ret;
        }

        private IEnumerable<SelectListItem> GetAllCreatedByUsers()
        {
            var ret = new List<SelectListItem>();
            var users = _context.Users.AsEnumerable();
            foreach (var user in users)
            {
                ret.Add(new SelectListItem() { Value = user.Id, Text = user.UserName });
            }
            return ret;
        }
    }
}
