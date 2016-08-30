using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BIRA_Issue_Tracker.Extensions;
using BIRA_Issue_Tracker.Models;
using BIRA_Issue_Tracker.Models.IssueTracker;
using Microsoft.Ajax.Utilities;
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

		// GET: Tagged
		public ActionResult Tagged(string id)
		{
			var slug = id;
			if (slug.IsNullOrWhiteSpace())
			{
				this.AddNotification("Couldn't find that tag.", NotificationType.Error);
				return HttpNotFound();
			}

			var issuesWithTag = db.Issues.Where(i => i.Tags.Any(b => b.Slug == slug)).ToArray();

			if (!issuesWithTag.Any())
			{
				this.AddNotification("Couldn't find any issues with that tag.", NotificationType.Warning);
				return RedirectToAction("Index", "Issues");
			}

			ViewBag.TagName = slug;
			return View(issuesWithTag);
		}

		// GET: Issues/AssignedToMe
		[Authorize]
		public ActionResult AssignedToMe()
		{
			var currentUserId = User.Identity.GetUserId();
			var currentUserIssues = db.Issues.Where(i => i.Assignee.Id == currentUserId).ToList();
			return View(currentUserIssues);
		}

		// GET: Issues/Mine
		[Authorize]
		public ActionResult Mine()
		{
			var currentUserId = User.Identity.GetUserId();
			var currentUserIssues = db.Issues.Where(i => i.Author.Id == currentUserId).ToList();
			return View(currentUserIssues);
		}

		// GET: Issues/Details/5
		public ActionResult Details(int? id, string returnTo = "Index")
		{
			if (id == null)
			{
				this.AddNotification("Invalid ID", NotificationType.Error);
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var issue = db.Issues.Find(id);
			if (issue == null)
			{
				this.AddNotification("Couldn't find that issue", NotificationType.Error);
				return HttpNotFound();
			}

			ViewBag.ReturnTo = returnTo;
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
		public ActionResult New([Bind(Include = "Id,Title,Description,State")] Issue issue, string returnTo = "Index")
		{
			var assignee = Request["Assignee"];
			var tagsRequest = Request["Tags"];

			var tags = tagsRequest.Split(',').Select(a => a.Trim()).ToArray();
			
			ModelState["Tags"].Errors.Clear();
			ModelState["Author"].Errors.Clear();

			issue.Date = DateTime.Now;
			issue.Author = db.Users.Find(User.Identity.GetUserId());
			issue.Assignee = db.Users.FirstOrDefault(a => a.UserName == assignee);

			issue.Tags = new HashSet<Tag>();
			foreach (var tagName in tags)
			{
				Tag tag;
				if (db.Tags.Any(a => a.Name == tagName))
				{
					tag = db.Tags.FirstOrDefault(a => a.Name == tagName);
					issue.Tags.Add(tag);
				}
				else
				{
					tag = new Tag(tagName);
				}

				issue.Tags.Add(tag);
			}

			TryValidateModel(issue);

			if (ModelState.IsValid)
			{
				db.Issues.Add(issue);
				db.SaveChanges();

				this.AddNotification("Issue created.", NotificationType.Success);
				return RedirectToAction(returnTo);
			}

			return View(issue);
		}

		// GET: Issues/Edit/5
		public ActionResult Edit(int? id, string returnTo = "Index")
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
				this.AddNotification("You're not authorized to edit this issue! Please log in.", NotificationType.Error);
				return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "You're not authorized to edit others' issues");
			}

			ViewBag.IsOwnIssue = UserCreatedIssue(issue);
			ViewBag.ReturnTo = returnTo;
			return View(issue);
		}

		// POST: Issues/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,Title,Description,State,Author,Assignee,Date,Tags")] Issue issue, string returnTo = "Index")
		{
			var oldIssue = db.Issues.AsNoTracking().First(a => a.Id == issue.Id);

			if (UserCreatedIssue(oldIssue))
			{
				oldIssue.Title = issue.Title;
				oldIssue.Description = issue.Description;
			}
			else
			{
				issue.Title = oldIssue.Title;
				issue.Description = oldIssue.Description;

				ModelState["Title"].Errors.Clear();
			}
			
			oldIssue.State = issue.State;
			
			issue = oldIssue;

			ViewBag.IsOwnIssue = UserCreatedIssue(issue);
			
			ModelState["Author"].Errors.Clear();
			ModelState["Tags"].Errors.Clear();

			TryValidateModel(issue);

			if (!UserAuthorizedToEdit(issue))
			{
				this.AddNotification("You're not authorized to edit this issue! Please log in.", NotificationType.Error);
				return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "You're not authorized to edit others' issues");
			}

			if (ModelState.IsValid)
			{
				db.Entry(issue).State = EntityState.Modified;
				db.SaveChanges();
				this.AddNotification("Edited issue!", NotificationType.Success);
				return RedirectToAction(returnTo);
			}
			return View(issue);
		}
		 
		// GET: Issues/Delete/5
		public ActionResult Delete(int? id, string returnTo = "Index")
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
				this.AddNotification("You're not authorized to edit this issue! Please log in.", NotificationType.Error);
				return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "You're not authorized to edit others' issues");
			}

			ViewBag.ReturnTo = returnTo;
			return View(issue);
		}

		// POST: Issues/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id, string returnTo = "Index")
		{
			var issue = db.Issues.Find(id);

			if (!UserAuthorizedToEdit(issue))
			{
				this.AddNotification("You're not authorized to edit this issue! Please log in.", NotificationType.Error);
				return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "You're not authorized to edit others' issues");
			}
			
			foreach (var tag in issue.Tags.ToList())
			{
				if (tag.Issues.Count == 1)
				{
					db.Tags.Remove(tag);
				}
			}

			db.Issues.Remove(issue);
			db.SaveChanges();

			this.AddNotification($"Issue \"{issue.Title}\" was deleted.", NotificationType.Success);
			return RedirectToAction(returnTo);
		}

		private bool UserAuthorizedToEdit(Issue issue)
		{
			// user should not edit others' issues unless they're an admin
			var isOwnIssue = UserCreatedIssue(issue);
			var isAdmin = User.IsInRole("Administrators");
			var isAssignedIssue = UserIsAssignedIssue(issue);
		
			var authorizedToEdit = isOwnIssue || isAssignedIssue || isAdmin;
			return authorizedToEdit;
		}

		private bool UserCreatedIssue(Issue issue)
		{
			return User.Identity.GetUserId() == issue.Author.Id;
		}

		private bool UserIsAssignedIssue(Issue issue)
		{
			return User.Identity.GetUserId() == issue.Assignee.Id;
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