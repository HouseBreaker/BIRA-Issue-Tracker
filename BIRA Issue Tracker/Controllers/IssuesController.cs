using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BIRA_Issue_Tracker.Models;
using BIRA_Issue_Tracker.Models.IssueTracker;
using Microsoft.AspNet.Identity;

namespace BIRA_Issue_Tracker.Controllers
{
	public class IssuesController : Controller
	{
		private IssueTrackerDbContext db = new IssueTrackerDbContext();

		// GET: Issues
		public ActionResult Index()
		{
			return View(db.Issues.ToList());
		}

		// GET: Issues/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var issue = db.Issues.Find(id);
			if (issue == null)
			{
				return HttpNotFound();
			}

			return View(issue);
		}

		// GET: Issues/New
		[Authorize]
		public ActionResult New()
		{
			var allUsers = new SelectList(db.Users.ToList());

			ViewBag.Users = allUsers;
			return View();
		}

		// POST: Issues/New
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public ActionResult New([Bind(Include = "Id,Title,Description,State")] Issue issue)
		{
			var assignee = Request["Assignee"];

			ModelState["Tags"].Errors.Clear();
			ModelState["Author"].Errors.Clear();

			issue.Date = DateTime.Now;
			issue.Author = db.Users.Find(User.Identity.GetUserId());
			issue.Assignee = db.Users.FirstOrDefault(a => a.UserName == assignee);

			if (issue.Tags == null)
			{
				issue.Tags = new List<Tag>();
			}

			TryValidateModel(issue);

			if (ModelState.IsValid)
			{
				db.Issues.Add(issue);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(issue);
		}

		// GET: Issues/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var issue = db.Issues.AsNoTracking().First(a => a.Id == id);

			if (issue == null)
			{
				return HttpNotFound();
			}

			if (!UserAuthorizedToEdit(issue))
			{
				return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "You're not authorized to edit others' issues");
			}

			return View(issue);
		}

		private bool UserAuthorizedToEdit(Issue issue)
		{
			// user should not edit others' issues unless they're an admin
			var isOwnIssue = User.Identity.GetUserId() == issue.Author.Id;

			var authorizedToEdit = isOwnIssue || User.IsInRole("Administrators");
			return authorizedToEdit;
		}

		// POST: Issues/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,Title,Description,State,Author,Assignee,Date,Tags")] Issue issue)
		{
			var oldIssue = db.Issues.AsNoTracking().First(a => a.Id == issue.Id);

			oldIssue.Title = issue.Title;
			oldIssue.Description = issue.Description;
			oldIssue.State = issue.State;

			issue = oldIssue;

			ModelState["Author"].Errors.Clear();
			ModelState["Tags"].Errors.Clear();

			TryValidateModel(issue);

			if (!UserAuthorizedToEdit(issue))
			{
				return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "You're not authorized to edit others' issues");
			}

			if (ModelState.IsValid)
			{
				db.Entry(issue).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(issue);
		}

		// GET: Issues/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var issue = db.Issues.Find(id);
			if (issue == null)
			{
				return HttpNotFound();
			}

			if (!UserAuthorizedToEdit(issue))
			{
				return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "You're not authorized to edit others' issues");
			}

			return View(issue);
		}

		// POST: Issues/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			var issue = db.Issues.Find(id);

			if (!UserAuthorizedToEdit(issue))
			{
				return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "You're not authorized to edit others' issues");
			}

			db.Issues.Remove(issue);
			db.SaveChanges();
			return RedirectToAction("Index");
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