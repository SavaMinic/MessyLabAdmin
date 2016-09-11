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
using MessyLabAdmin.Services;
using System;

namespace MessyLabAdmin.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private ApplicationDbContext _context;
        private IEmailSender _email;

        public StudentsController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _email = emailSender;  
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
            ViewBag.totalPages = (int)Math.Ceiling(students.Count() / 10f);
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
        [ValidateAntiForgeryToken]
        public IActionResult DeactivateAllStudents()
        {
            var activeStudents = _context.Students.Where(s => s.IsActive);
            activeStudents.ToList().ForEach(s => s.IsActive = false);
            _context.UpdateRange(activeStudents);
            _context.SaveChanges();

            TempData.Clear();
            TempData.Add("deactivationOK", true);

            return RedirectToAction("Index");
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
            Dictionary<string, string> emailsForSending = new Dictionary<string, string>();
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

                            // prepare email for sending to students

                            if (!emailsForSending.ContainsKey(student.DefaultEmail))
                            {
                                var requestCode = Utility.CalculatePasswordRequestCode();
                                var content = string.Format(
                                       "Hello {0},<br /><br />"
                                       + "Your Messy Lab Account has been created.<br />"
                                       + "Initial password: <b>{1}</b><br /><br />"
                                       + "Click <a href=\"{2}\">here</a> to reset your initial password.<br /><br />"
                                       + "We wish you good luck with your exams!<br /><br />"
                                   , student.Username, student.InitialPassword,
                                    Url.Action("PasswordReset", "Client", new { code = requestCode }, "http", Request.PathBase));
                                emailsForSending.Add(student.DefaultEmail, content);

                                // create a password reset request
                                var reset = new PasswordReset();
                                reset.Student = student;
                                reset.CreatedTime = DateTime.Now;
                                reset.RequestCode = requestCode;
                                _context.PasswordResets.Add(reset);
                            }
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
                // send emails
                foreach (KeyValuePair<string, string> entry in emailsForSending)
                {
                    _email.SendEmailAsync(entry.Key, "Messy Lab Account created", entry.Value);
                }
            }

            TempData.Clear();
            TempData.Add("parsingOK", parsingOK);
            TempData.Add("addedStudentsCount", addedStudentsCount);

            return RedirectToAction("Index");
        }

        public IActionResult ResetPasswordAndSendInitial(int? id)
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

            // reset the password
            student.PasswordHash = Utility.CalculatePasswordHash(student.Username, student.InitialPassword);
            _context.Update(student);
            var isOK = _context.SaveChanges() == 1;

            // send the email with initial password
            var content = string.Format(
                "Hello {0},<br /><br />"
                + "Login to Messy Lab with following data:<br />"
                + "Username: <b>{1}</b><br />"
                + "Password: <b>{2}</b><br /><br />"
                + "Change your initial password after logging in.<br /><br />"
                + "If you did not request the reset, just ignore this email.<br /><br />"
            , student.FullName, student.Username, student.InitialPassword);
            _email.SendEmailAsync(student.DefaultEmail, "Messy Lab initial Password", content);

            TempData.Clear();
            TempData.Add("isOK", isOK);

            return RedirectToAction("Details", new { id = id });
        }
    }
}
