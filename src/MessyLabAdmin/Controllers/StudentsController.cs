using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MessyLabAdmin.Models;
using MessyLabAdmin.Util;
using System.Collections.Generic;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.Net.Http.Headers;
using System.IO;

namespace MessyLabAdmin.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Students
        public IActionResult Index(int? page, string firstName, string lastName, int? enrollmentYear, int? enrollmentNumber, int? isActive, int? solutionCount)
        {
            IQueryable<Student> students = _context.Students
                .Include(u => u.StudentAssignments)
                .Include(u => u.Actions);

            if (firstName != null && firstName != "")
            {
                students = students.Where(s => s.FirstName.Contains(firstName));
                ViewBag.firstName = firstName;
            }
            if (lastName != null && lastName != "")
            {
                students = students.Where(s => s.LastName.Contains(lastName));
                ViewBag.lastName = lastName;
            }
            if (enrollmentYear != null)
            {
                students = students.Where(s => s.EnrollmentYear == enrollmentYear);
                ViewBag.enrollmentYear = enrollmentYear;
            }
            if (enrollmentNumber != null)
            {
                students = students.Where(s => s.EnrollmentNumber == enrollmentNumber);
                ViewBag.enrollmentNumber = enrollmentNumber;
            }
            if (isActive != null)
            {
                students = students.Where(s => s.IsActive == (isActive == 1));
                ViewBag.isActive = isActive;
            }
            if (solutionCount != null)
            {
                students = students.Where(s => 
                    s.StudentAssignments.Where(sa => sa.Solution != null).Count() == solutionCount
                );
                ViewBag.solutionCount = solutionCount;
            }

            ViewBag.currentPage = page ?? 1;
            ViewBag.totalPages = students.Count() / 10 + 1;
            ViewBag.statusData = new List<SelectListItem>()
            {
                new SelectListItem() { Value = "0", Text = "Neaktivan" },
                new SelectListItem() { Value = "1", Text = "Aktivan" },
            };

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

            ViewBag.PasswordChanged = student.PasswordHash != Utility.CalculatePasswordHash(student.Username, student.InitialPassword);

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
                student.InitialPassword = student.PasswordHash;
                student.PasswordHash = Utility.CalculatePasswordHash(student.Username, student.PasswordHash);
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

            ViewBag.PasswordChanged = student.PasswordHash != Utility.CalculatePasswordHash(student.Username, student.InitialPassword);

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

        [HttpPost]
        public IActionResult ResetPassword(int id)
        {
            Student student = _context.Students.Single(m => m.ID == id);
            if (student == null)
            {
                return HttpNotFound();
            }

            student.PasswordHash = Utility.CalculatePasswordHash(student.Username, student.InitialPassword);
            _context.Students.Update(student);
            _context.SaveChanges();

            return Ok(new { ok = true });
        }

        [HttpPost]
        public IActionResult ImportStudents(IFormFile studentsCSV)
        {
            if (studentsCSV == null)
            {
                return HttpNotFound();
            }

            // parse CSV file and insert students
            bool parsingOK = true;
            int addedStudentsCount = 0;
            using (var stream = studentsCSV.OpenReadStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    while(!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line == null) { parsingOK = false; break; }
                        if (line == "") continue;

                        var data = line.Split(',');
                        if(data.Length < 5) { parsingOK = false;  break; }

                        try
                        {
                            var student = new Student()
                            {
                                EnrollmentYear = int.Parse(data[0]),
                                EnrollmentNumber = int.Parse(data[1]),
                                FirstName = data[2],
                                LastName = data[3],
                                IsActive = bool.Parse(data[4]),
                            };
                            student.Username = data.Length > 5 ? data[5] : student.DefaultUsername;
                            student.InitialPassword = data.Length > 6 ? data[6] : Utility.CalculatePasswordRequestCode(8);
                            student.PasswordHash = Utility.CalculatePasswordHash(student.Username, student.PasswordHash);
                            _context.Students.Add(student);
                            addedStudentsCount++;
                        }
                        catch (System.FormatException)
                        {
                            parsingOK = false;
                            break;
                        }
                    }
                }
            }

            if (parsingOK)
            {
                _context.SaveChanges();
            }

            return RedirectToAction("Index", new { ok = parsingOK, c = addedStudentsCount });
        }
    }
}
