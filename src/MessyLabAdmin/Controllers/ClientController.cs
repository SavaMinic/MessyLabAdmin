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
using System.Security.Cryptography;
using System.Text;
using Action = MessyLabAdmin.Models.Action;
using MessyLab.PicoComputer;
using MessyLabAdmin.Services;
using MessyLabAdmin.ViewModels.Client;
using SendGrid.CSharp.HTTP.Client;
using SendGrid.Helpers.Mail;
using MessyLabAdmin.Util.Sendgrid;

namespace MessyLabAdmin.Controllers
{
    [AllowAnonymous]
    public class ClientController : Controller
    {

        private class AssignmentData
        {
            public int ID;
            public string Title;
            public string Description;
            public DateTime StartTime;
            public DateTime EndTime;
            public bool CanSendSolution;

            // optional
            public string SolutionCode;
            public DateTime SolutionCreated;
            public bool SolutionEvaluated;
        }

        private ApplicationDbContext _context;
        private IEmailSender _email;

        public ClientController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _email = emailSender;
        }

        #region Login

        // POST: /Client/Login
        [HttpPost]
        public IActionResult Login(string user, string pass)
        {
            if (user == null || user == "" || pass == null || pass == "")
                return HttpNotFound();

            string passwordHash = Utility.CalculatePasswordHash(user, pass);
            Student student = _context.Students.SingleOrDefault(s => s.Username == user && s.PasswordHash == passwordHash);

            if (student == null)
                return HttpNotFound(new { error = -1 });

            // save login action
            Action action = new Action();
            action.Type = Models.Action.ActionType.Login;
            action.CreatedTime = DateTime.Now;
            action.Student = student;
            _context.Actions.Add(action);

            // return a value for session id
            long loginTime = DateTime.Now.Ticks;
            student.LastLoginTimestamp = loginTime;
            _context.Students.Update(student);
            _context.SaveChanges();

            return Ok(new { sessionID =  loginTime + "" + student.ID });
        }

        #endregion

        #region Actions

        // POST: /Client/Action
        [HttpPost]
        public IActionResult Action(string sessionID, Action.ActionType type, string data)
        {
            if (sessionID == null || sessionID == "")
                return HttpNotFound();

            Student student = _context.Students.SingleOrDefault(s => s.SessionID == sessionID);
            if (student == null)
                return HttpNotFound(new { error = -1 });

            Action action = new Action();
            action.Type = type;
            action.Data = data;
            action.CreatedTime = DateTime.Now;
            action.Student = student;

            _context.Actions.Add(action);
            var isOk = _context.SaveChanges() == 1;

            return Ok(new { ok = isOk });
        }

        #endregion

        #region Assignments 

        // GET: /Client/Assignments
        [HttpGet]
        public IActionResult Assignments(string sessionID)
        {
            if (sessionID == null || sessionID == "")
                return HttpNotFound();

            Student student = _context.Students.SingleOrDefault(s => s.SessionID == sessionID);
            if (student == null)
                return HttpNotFound(new { error = -1 });

            var studentAssignments = _context.StudentAssignments
                .Include(sa => sa.Assignment)
                .Include(sa => sa.Solution)
                .Where(sa => sa.StudentID == student.ID
                    && sa.Assignment.IsActive 
                    && sa.Assignment.StartTime <= DateTime.Now 
                    //&& sa.Assignment.EndTime >= DateTime.Now
                );

            var ret = new List<AssignmentData>();
            foreach (var studentAssignment in studentAssignments)
            {
                var data = new AssignmentData()
                {
                    ID = studentAssignment.Assignment.ID,
                    Title = studentAssignment.Assignment.Title,
                    Description = studentAssignment.Assignment.Description,
                    StartTime = studentAssignment.Assignment.StartTime,
                    EndTime = studentAssignment.Assignment.EndTime,
                    CanSendSolution = studentAssignment.Assignment.EndTime >= DateTime.Now,
                };
                if (studentAssignment.Solution != null)
                {
                    data.SolutionCode = studentAssignment.Solution.Code;
                    data.SolutionCreated = studentAssignment.Solution.CreatedTime;
                    data.SolutionEvaluated = studentAssignment.Solution.IsEvaluated;
                }
                ret.Add(data);
            }
            return Ok(new { ok = true, assignments = ret });
        }

        // POST: /Client/Assignments
        [HttpPost]
        public IActionResult Assignments(string sessionID, int assignmentID, string code)
        {
            if (sessionID == null || sessionID == "")
                return HttpNotFound();

            Student student = _context.Students.SingleOrDefault(s => s.SessionID == sessionID);
            if (student == null)
                return HttpNotFound(new { error = -1 });

            var studentAssignment = _context.StudentAssignments
                .Include(sa => sa.Assignment)
                .Include(sa => sa.Solution)
                .ThenInclude(s => s.AssignmentTestResults)
                .SingleOrDefault(sa => sa.StudentID == student.ID
                    && sa.Assignment.IsActive
                    && sa.Assignment.StartTime <= DateTime.Now
                    && sa.Assignment.EndTime >= DateTime.Now
                    && sa.AssignmentID == assignmentID
                );
            if (studentAssignment == null)
                return HttpNotFound(new { error = -2 });

            // this is just in case, it's encoded
            code = code.ConvertUrlEncodedToNewLine();

            if (studentAssignment.Solution == null)
            {
                var solution = new Solution()
                {
                    Code = code,
                    CreatedTime = DateTime.Now,
                    Student = student,
                    Assignment = studentAssignment.Assignment,
                };
                _context.Solutions.Add(solution);
                studentAssignment.Solution = solution;
                _context.StudentAssignments.Update(studentAssignment);
            }
            else
            {
                studentAssignment.Solution.Code = code;
                studentAssignment.Solution.CreatedTime = DateTime.Now;
                _context.Solutions.Update(studentAssignment.Solution);
            }
            _context.SaveChanges();

            // compile user code and test it out
            var errors = CompileAndRunUserCode(studentAssignment, code, false);

            return errors == null ? Ok(new { ok = true }) : Ok(new { errors = errors });
        }

        #endregion

        #region Passwords

        // POST: /Client/RequestPasswordReset
        [HttpPost]
        public IActionResult RequestPasswordReset(string username)
        {
            if (username == null || username == "")
                return HttpNotFound(new { error = -1 });

            var student = _context.Students.SingleOrDefault(s => s.Username == username);
            if (student == null)
                return HttpNotFound(new { error = -2 });

            // if already non-used request exists in last 24h
            var existingRequest = _context.PasswordResets.Where(
                r => r.StudentID == student.ID 
                && !r.IsUsed 
                && r.CreatedTime.AddHours(24) >= DateTime.Now
            ).OrderByDescending(r => r.CreatedTime).FirstOrDefault();
            if (existingRequest != null)
                return HttpNotFound(new { error = -3 });

            var requestCode = Utility.CalculatePasswordRequestCode();

            // send email
            var content = string.Format(
                "Hello {0},<br /><br />"
                + "Click <a href=\"{1}\">here</a> to reset your password.<br /><br />"
                + "If you did not request the reset, just ignore this email.<br /><br />"
            , username, Url.Action("PasswordReset", "Client", new { code = requestCode }, "http", Request.PathBase));
            _email.SendEmailAsync(student.DefaultEmail, "Messy Lab Password request", content);

            // create a password reset request
            var reset = new PasswordReset();
            reset.Student = student;
            reset.CreatedTime = DateTime.Now;
            reset.RequestCode = requestCode;
            _context.PasswordResets.Add(reset);

            _context.SaveChanges();

            return Ok(new { ok = true });
        }

        // GET: /Client/PasswordReset
        [HttpGet]
        public IActionResult PasswordReset(string code = null)
        {
            if (code == null || code == "")
                return HttpNotFound();

            var resetRequest = _context.PasswordResets.SingleOrDefault(
                r => r.RequestCode == code
            );
            if (resetRequest == null)
                ViewBag.Error = "Request is not valid!";
            else if (resetRequest.IsUsed)
                ViewBag.Error = "Request already used!";
            else if (resetRequest.CreatedTime.AddHours(24) < DateTime.Now)
                ViewBag.Error = "Request expires after 24h!";

            return View();
        }

        // POST: /Client/PasswordReset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PasswordReset(StudentResetPasswordViewModel reset)
        {
            if (ModelState.IsValid)
            {
                var resetRequest = _context.PasswordResets.SingleOrDefault(
                    r => r.RequestCode == reset.Code
                    && !r.IsUsed
                    && r.CreatedTime.AddHours(24) >= DateTime.Now
                );
                if (resetRequest != null)
                {
                    var student = _context.Students.SingleOrDefault(s => s.ID == resetRequest.StudentID);
                    if (student != null)
                    {
                        student.PasswordHash = Utility.CalculatePasswordHash(student.Username, reset.Password);
                        _context.Update(student);

                        resetRequest.IsUsed = true;
                        _context.Update(resetRequest);

                        _context.SaveChanges();
                        ViewBag.Success = true;

                        // send success email
                        var content = string.Format(
                            "Hello {0},<br /><br />"
                            + "Password for your account {1} has been changed successfully."
                            + "If you didn't do this change, contact administrator because <b>your account might be compromised</b>.<br /><br />"
                        , student.FullName, student.Username);
                        _email.SendEmailAsync(student.DefaultEmail, "Messy Lab password changed!", content);
                    }
                }
            }
            return View();
        }

        #endregion

        #region Solutions

        [HttpGet]
        public IActionResult ManualTestOfSolution(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Solution solution = _context.Solutions.Single(m => m.ID == id);
            if (solution == null)
            {
                return HttpNotFound(new { error = -1 });
            }

            var studentAssignment = _context.StudentAssignments
                .Include(sa => sa.Solution)
                .ThenInclude(s => s.AssignmentTestResults)
                .Single(
                    sa => sa.AssignmentID == solution.AssignmentID && sa.SolutionID == solution.ID
                );
            if (studentAssignment == null)
            {
                return HttpNotFound(new { error = -2 });
            }

            var errors = CompileAndRunUserCode(studentAssignment, solution.Code);

            TempData.Clear();
            TempData.Add("isOK", errors == null);

            return RedirectToAction("Details", "Solutions", new { id = id });
        }

        /// <summary>
        /// Compiles and runs the code on Pico VM
        /// </summary>
        /// <param name="studentAssignment">StudentAssignment, requires its Solution and its TestResults to be loaded</param>
        /// <param name="code">User source code</param>
        /// <returns></returns>
        private List<Error> CompileAndRunUserCode(StudentAssignment studentAssignment, string code, bool returnExpectedOutputError = true)
        {
            Assembler a = new Assembler();
            a.LoadFromString(code);
            if (!a.Process())
            {
                return a.Errors;
            }

            var hex = a.ExportToHex();

            // get tests
            var variant = _context.AssignmentVariants.Single(av => av.AssignmentID == studentAssignment.AssignmentID && av.Index == studentAssignment.AssignmentVariantIndex);
            var tests = _context.AssignmentTests.Where(at => at.AssignmentVariantID == variant.ID);

            // mark as testing, clear all past tests
            studentAssignment.Solution.LastTestedTime = DateTime.Now;
            if (studentAssignment.Solution.AssignmentTestResults != null)
            {
                studentAssignment.Solution.AssignmentTestResults.Clear();
            }
            _context.Update(studentAssignment.Solution);
            _context.SaveChanges();

            var errors = new List<Error>();
            foreach(var test in tests)
            {
                ushort[] givenInput = test.GivenInput != null
                    ? test.GivenInput.Trim().Split(' ').Select(ushort.Parse).ToArray()
                    : new ushort[0];
                ushort[] expectedOutput = test.ExpectedOutput != null
                    ? test.ExpectedOutput.Trim().Split(' ').Select(ushort.Parse).ToArray()
                    : new ushort[0];

                VirtualMachine vm = new VirtualMachine(givenInput, expectedOutput);
                vm.LoadFromLines(hex);
                RuntimeException e = vm.Run();

                // save test result
                var testResult = new AssignmentTestResult()
                {
                    AssignmentTestID = test.ID,
                    SolutionID = studentAssignment.Solution.ID,
                    CreatedTime = DateTime.Now,
                    CalculatedOutput = (vm.Processor.IODevice as TestingIODevice).CalculatedOutput,             
                };
                if (e == null || e is NormalTerminationRuntimeException)
                {
                    testResult.IsSuccess = true;
                }
                else
                {
                    if (returnExpectedOutputError)
                    {
                        errors.Add(new Error() { ID = 666, Description = e.ToString() });
                    }
                }
                _context.AssignmentTestResults.Add(testResult);
            }
            _context.SaveChanges();
            return errors.Count > 0 ? errors : null;
        }

        #endregion
    }
}
