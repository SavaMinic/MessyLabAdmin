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

            string passwordHash = CalculatePasswordHash(user, pass);
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

        private string CalculatePasswordHash(string username, string pass)
        {
            using (SHA1 sha1Hash = SHA1.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(username + pass));

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }
    }
}
