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

namespace MessyLabAdmin.Controllers
{
    [AllowAnonymous]
    public class ClientController : Controller
    {

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
    }
}
