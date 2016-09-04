using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MessyLabAdmin.Models;
using System;
using Microsoft.AspNet.Authorization;

namespace MessyLabAdmin.Controllers
{
    [Authorize]
    public class SolutionsController : Controller
    {
        private ApplicationDbContext _context;

        public SolutionsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Solutions/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Solution solution = _context.Solutions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .Include(s => s.AssignmentTestResults)
                .ThenInclude(atr => atr.AssignmentTest)
                .Single(m => m.ID == id);
            if (solution == null)
            {
                return HttpNotFound();
            }

            var studentAssignment = _context.StudentAssignments
                .Include(sa => sa.SolutionHistory)
                .ThenInclude(s => s.AssignmentTestResults)
                .Include(sa => sa.Solution)
                .ThenInclude(s => s.AssignmentTestResults)
                .Single(
                    sa => sa.AssignmentID == solution.AssignmentID && sa.StudentID == solution.StudentID
                );
            if (studentAssignment != null)
            {
                var variant = _context.AssignmentVariants.Single(
                       av => av.AssignmentID == solution.AssignmentID && av.Index == studentAssignment.AssignmentVariantIndex
                );
                ViewBag.variant = variant;
                ViewBag.testsCount = _context.AssignmentTests.Count(at => at.AssignmentVariantID == variant.ID);
                ViewBag.studentAssignment = studentAssignment;
            }
            return View(solution);
        }

        public IActionResult ToggleEvaluated(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Solution solution = _context.Solutions.Single(m => m.ID == id);
            if (solution == null)
            {
                return HttpNotFound();
            }

            solution.IsEvaluated = !solution.IsEvaluated;
            _context.Update(solution);
            _context.SaveChanges();

            TempData.Clear();
            TempData.Add("isEvaluated", solution.IsEvaluated);

            return RedirectToAction("Details", new { id = id });
        }
    }
}
