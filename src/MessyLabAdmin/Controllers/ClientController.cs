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

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
