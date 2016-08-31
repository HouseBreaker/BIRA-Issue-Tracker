using System.Collections.Generic;
using BIRA_Issue_Tracker.Models;
using BIRA_Issue_Tracker.Models.Identity;
using BIRA_Issue_Tracker.Models.IssueTracker;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BIRA_Issue_Tracker.Migrations
{
	using System;
	using System.Data.Entity.Migrations;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<IssueTrackerDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
			ContextKey = "BIRA_Issue_Tracker.Models.IssueTrackerDbContext";
		}

		protected override void Seed(IssueTrackerDbContext db)
		{
			if (!db.Users.Any())
			{
				CreateUser(db, "admin@gmail.com", "123", "Vladi Admina");
				CreateUser(db, "gosho@gmail.com", "123", "George Petrov");
				CreateUser(db, "pesho@gmail.com", "123", "Peter Ivanov");
				CreateUser(db, "merry@gmail.com", "123", "Maria Petrova");

				CreateRole(db, "Administrators");
				AddUserToRole(db, "admin@gmail.com", "Administrators");
			}

			db.SaveChanges();

			if (!db.Issues.Any())
			{
				CreateIssue(db,
					"HTTP 400 error",
					"Adding an author with incorrect date format will return an HTTP 400 response code.",
					State.Open,
					"admin@gmail.com",
					"gosho@gmail.com",
					new SortedSet<Tag>
					{
						FindTagByName(db, "user input"),
						FindTagByName(db, "network"),
						FindTagByName(db, "date"),
						FindTagByName(db, "HTTP 400"),
						FindTagByName(db, "author"),
					}
				);

				CreateIssue(db,
					"\'00\' birthday causes exception",
					"Adding an author with a birth date which has day 00 will add the author with a birth date of the last day of the previous month",
					State.Open,
					"pesho@gmail.com",
					"merry@gmail.com",
					new SortedSet<Tag>
					{
						FindTagByName(db, "birth date"),
						FindTagByName(db, "form"),
						FindTagByName(db, "error"),
						FindTagByName(db, "input validation"),
					}
				);

				CreateIssue(db,
					"negative birthday causes exception",
					"Adding an author with a birth date the day of which is negative will add the author with a birth date which is the same but the day is non negative",
					State.Open,
					"merry@gmail.com",
					"gosho@gmail.com",
					new SortedSet<Tag>
					{
						FindTagByName(db, "birthday"),
						FindTagByName(db, "error"),
						FindTagByName(db, "input validation"),
					}
				);

				CreateIssue(db,
					"no last name causes exception",
					"Adding an author with a valid first name and date, but no last name throws an unhandled exception \"Last name out of range\" insteaof validating the input before sending it.",
					State.Open,
					"gosho@gmail.com",
					"admin@gmail.com",
					new SortedSet<Tag>
					{
						FindTagByName(db, "form"),
						FindTagByName(db, "last name"),
						FindTagByName(db, "date"),
					}
				);

				CreateIssue(db,
					"missing first name causes exception",
					"Adding an author with a valid last name and date, but no first name throws an unhandled exception \"First name out of range\" instead of validating the input before sending it.",
					State.Open,
					"pesho@gmail.com",
					"gosho@gmail.com",
					new SortedSet<Tag>
					{
						FindTagByName(db, "form"),
						FindTagByName(db, "last name"),
						FindTagByName(db, "date"),
					}
				);

				CreateIssue(db,
					"short first name causes exception",
					"If first name is too short, the system throws an unhandled exception \"First name out of range\" instead of validating the input before sending it.",
					State.Open,
					"admin@gmail.com",
					"admin@gmail.com",
					new SortedSet<Tag>
					{
						FindTagByName(db, "first name"),
						FindTagByName(db, "error"),
						FindTagByName(db, "form"),
					}
				);

				CreateIssue(db,
					"short last name causes exception",
					"If last name is too short, the system throws an unhandled exception \"Last name out of range\" instead of validating the input before sending it.",
					State.Open,
					"merry@gmail.com",
					"gosho@gmail.com",
					new SortedSet<Tag>
					{
						FindTagByName(db, "last name"),
						FindTagByName(db, "error"),
						FindTagByName(db, "form"),
					}
				);

				CreateIssue(db,
					"Wrong first name upper limit",
					"First name upper limit is 239 characters instead of 240",
					State.Open,
					"merry@gmail.com",
					"admin@gmail.com",
					new SortedSet<Tag>
					{
						FindTagByName(db, "back end"),
						FindTagByName(db, "form"),
						FindTagByName(db, "error"),
					}
				);

				CreateIssue(db,
					"Wrong last name upper limit",
					"Last name upper limit is 237 characters instead of 240",
					State.Open,
					"gosho@gmail.com",
					"gosho@gmail.com",
					new SortedSet<Tag>
					{
						FindTagByName(db, "back end"),
						FindTagByName(db, "form"),
						FindTagByName(db, "error"),
					}
				);

				CreateIssue(db,
					"Tags don't work",
					"Last name upper limit is 237 characters instead of 240",
					State.Open,
					"gosho@gmail.com",
					"gosho@gmail.com",
					new SortedSet<Tag>
					{
						FindTagByName(db, "back end"),
						FindTagByName(db, "form"),
						FindTagByName(db, "error"),
					}
				);
			}

			db.SaveChanges();
		}

		private static Tag FindTagByName(IssueTrackerDbContext db, string name)
		{
			var foundTag = db.Tags.FirstOrDefault(t => t.Name == name);

			if (foundTag == null)
			{
				var newTag = new Tag(name);
				db.Tags.Add(newTag);

				db.SaveChanges();
				return db.Tags.FirstOrDefault(t => t.Name == newTag.Name);
			}

			return foundTag;
		}

		private static void CreateUser(IssueTrackerDbContext db,
			string email, string password, string fullName)
		{
			var userManager = new UserManager<ApplicationUser>(
				new UserStore<ApplicationUser>(db))
			{
				PasswordValidator = new PasswordValidator
				{
					RequiredLength = 1,
					RequireNonLetterOrDigit = false,
					RequireDigit = false,
					RequireLowercase = false,
					RequireUppercase = false,
				}
			};

			var user = new ApplicationUser
			{
				UserName = email,
				Email = email,
				FullName = fullName
			};

			var userCreateResult = userManager.Create(user, password);
			if (!userCreateResult.Succeeded)
			{
				throw new Exception(string.Join("; ", userCreateResult.Errors));
			}
		}

		private static void CreateIssue(IssueTrackerDbContext db, string title, string description, State state,
			string author, string assignee, ISet<Tag> tags)
		{
			var authorAsUser = db.Users.FirstOrDefault(u => u.UserName == author);
			var assigneeAsUser = db.Users.FirstOrDefault(u => u.UserName == assignee);

			var issue = new Issue
			{
				Title = title,
				Description = description,
				State = state,
				Author = authorAsUser,
				Assignee = assigneeAsUser,
				Date = DateTime.Now,
				Tags = tags
			};

			db.Issues.Add(issue);
			//var issueInDatabase = db.Issues.Find(issue.Id);

			//foreach (var tag in tags)
			//{
			//	issueInDatabase.Tags.Add(tag);
			//}
		}

		private void CreateRole(IssueTrackerDbContext db, string roleName)
		{
			var roleManager = new RoleManager<IdentityRole>(
				new RoleStore<IdentityRole>(db));
			var roleCreateResult = roleManager.Create(new IdentityRole(roleName));
			if (!roleCreateResult.Succeeded)
			{
				throw new Exception(string.Join("; ", roleCreateResult.Errors));
			}
		}

		private static void AddUserToRole(IssueTrackerDbContext db, string userName, string roleName)
		{
			var user = db.Users.First(u => u.UserName == userName);
			var userManager = new UserManager<ApplicationUser>(
				new UserStore<ApplicationUser>(db));
			var addAdminRoleResult = userManager.AddToRole(user.Id, roleName);
			if (!addAdminRoleResult.Succeeded)
			{
				throw new Exception(string.Join("; ", addAdminRoleResult.Errors));
			}
		}
	}
}