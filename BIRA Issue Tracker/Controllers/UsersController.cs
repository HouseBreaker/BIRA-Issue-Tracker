using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BIRA_Issue_Tracker.Models;
using BIRA_Issue_Tracker.Models.Identity;
using BIRA_Issue_Tracker.Models.IssueTracker;

namespace BIRA_Issue_Tracker.Controllers
{
    public class UsersController : Controller
    {
        private IssueTrackerDbContext db = new IssueTrackerDbContext();

        // GET: Users
        public ActionResult Index()
        {
	        var issues = db.Issues.ToList();
	        ViewBag.Issues = issues;

			return View(db.Users.ToList());
        }

        // GET: Users/Profile/test@gmail.com
        public ActionResult Profile(string id)
        {
	        var email = id;
            if (email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return HttpNotFound();
            }

	        var createdIssues = db.Issues.Where(a => a.Author.Id == user.Id).ToList();
	        var assignedIssues = db.Issues.Where(a => a.Assignee.Id == user.Id).ToList();

	        ViewBag.CreatedIssues = createdIssues;
	        ViewBag.AssignedIssues = assignedIssues;

	        int solvedIssuesRatio = 0;
	        if (!assignedIssues.Any())
	        {
		        solvedIssuesRatio = 100;
	        }
	        else
	        {
		        solvedIssuesRatio = (int)(assignedIssues.Count(a => a.State == State.Fixed || a.State == State.Closed) / (double)assignedIssues.Count * 100);
			}
			ViewBag.SolvedIssuesRatio = solvedIssuesRatio;

			return View(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
