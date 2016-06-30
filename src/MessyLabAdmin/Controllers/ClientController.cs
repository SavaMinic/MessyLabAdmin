﻿using System.Linq;
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
                .SingleOrDefault(sa => sa.StudentID == student.ID
                    && sa.Assignment.IsActive
                    && sa.Assignment.StartTime <= DateTime.Now
                    && sa.Assignment.EndTime >= DateTime.Now
                    && sa.AssignmentID == assignmentID
                );
            if (studentAssignment == null)
                return HttpNotFound(new { error = -2 });

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
            return Ok(new { ok = true });
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
                "<h1>Messy Lab</h1>"
                + "Hello {0},<br />\n"
                + "Click <a href='{1}'>HERE</a> to reset your password.<br />\n"
            , username, Url.Action("PasswordReset", "Client", new { code = requestCode }, "http", Request.PathBase));
            _email.SendEmailAsync("minic.sava@gmail.com", "Messy Lab Password request", content);

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

                        // TODO: send success email
                    }
                }
            }
            return View();
        }

        #endregion

        #region Solutions

        // just testing
        public IActionResult GetCheckResult(ushort test)
        {
            Assembler a = new Assembler();
            //a.LoadFromFile("filename");
            a.LoadFromString("z = 0\n" +
                  "a = 1\n" +
                  "b = 2\n" +
                  "c = 3\n" +
                  "d = 4\n" +
                  "e = 5\n" +
                  "f = 6\n" +
                  "g = 7\n" +
                  "ORG 8\n" +
                  "MOV a, 666\n" +
                  "OUT a\n" +
                  "STOP");
            if (!a.Process())
            {
                return Ok(new { errors = a.Errors });
            }

            var hex = a.ExportToHex();

            ushort[] givenInput = new ushort[] { };
            ushort[] expectedOutput = new ushort[] { test };

            VirtualMachine vm = new VirtualMachine(givenInput, expectedOutput);
            vm.LoadFromLines(hex);
            RuntimeException e = vm.Run();

            if (e== null || e is NormalTerminationRuntimeException)
            {
                return Ok(new { ok = true });
            }
            return Ok(new { error = e.ToString() });
        }

        #endregion
    }
}
