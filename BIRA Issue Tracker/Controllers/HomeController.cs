using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BIRA_Issue_Tracker.Models;
using BIRA_Issue_Tracker.Models.IssueTracker;
using Microsoft.AspNet.Identity;

namespace BIRA_Issue_Tracker.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			var db = new IssueTrackerDbContext();

			var unresolvedIssues = db.Issues.Where(a => a.State != State.Closed || a.State != State.Fixed);
			ViewBag.UnresolvedIssuesCount = unresolvedIssues.Count();

			if (User.Identity.IsAuthenticated)
			{
				var currentUser = db.Users.Find(User.Identity.GetUserId());
				ViewBag.Username = currentUser.FullName.Split(' ')[0];
				ViewBag.UserAssignedUnresolvedIssuesCount = unresolvedIssues.Count(a => a.Assignee.Id == currentUser.Id);
			}

			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}