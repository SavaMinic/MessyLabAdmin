using System.Linq;
using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MessyLabAdmin.Models;
using MessyLabAdmin.Util;
using System.Collections.Generic;
using System.Globalization;
using Action = MessyLabAdmin.Models.Action;

namespace MessyLabAdmin.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.latestActions = _context.Actions
                .Include(a => a.Student)
                .OrderByDescending(a => a.CreatedTime)
                .Take(10);
            ViewBag.latestSolutions = _context.Solutions
                .Include(s => s.Assignment)
                .OrderByDescending(s => s.CreatedTime)
                .Take(10);
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
