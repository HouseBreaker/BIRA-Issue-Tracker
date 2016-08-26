using System.Collections.Generic;
using BIRA_Issue_Tracker.Models;
using BIRA_Issue_Tracker.Models.Identity;
using BIRA_Issue_Tracker.Models.IssueTracker;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BIRA_Issue_Tracker.Migrations
{
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<BIRA_Issue_Tracker.Models.IssueTrackerDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
			ContextKey = "BIRA_Issue_Tracker.Models.IssueTrackerDbContext";
		}

		protected override void Seed(IssueTrackerDbContext context)
		{
			if (!context.Users.Any())
			{
				CreateUser(context, "admin@gmail.com", "123", "System Administrator");
				CreateUser(context, "pesho@gmail.com", "123", "Peter Ivanov");
				CreateUser(context, "merry@gmail.com", "123", "Maria Petrova");
				CreateUser(context, "gosho@gmail.com", "123", "George Petrov");

				CreateRole(context, "Administrators");
				AddUserToRole(context, "admin@gmail.com", "Administrators");
			}

			if (!context.Tags.Any())
			{
				context.Tags.AddRange(new[]
				{
					new Tag("user input"),
					new Tag("user interface"),
					new Tag("network"),
					new Tag("general"),
					new Tag("registration"),
					new Tag("login"),
				});
			}

			context.SaveChanges();

			if (!context.Issues.Any())
			{
				CreateIssue(context,
					"HTTP 400 error",
					"Adding an author with incorrect date format will return an HTTP 400 response code.",
					State.Open,
					"admin@gmail.com",
					"gosho@gmail.com",
					new[]
					{
						FindTagByName(context, "user input"),
						FindTagByName(context, "network"),
					}
				);

				CreateIssue(context,
					"00 birthday causes exception",
					"Adding an author with a birth date which has day 00 will add the author with a birth date of the last day of the previous month.",
					State.Open,
					"admin@gmail.com",
					"gosho@gmail.com",
					new[] {FindTagByName(context, "user input"),}
				);

				CreateIssue(context,
					"negative birthday causes exception",
					"Adding an author with a birth date the day of which is negative will add the author with a birth date which is the same but the day is non negative.",
					State.Open,
					"admin@gmail.com",
					"gosho@gmail.com",
					new[] {FindTagByName(context, "user input"),}
				);

				CreateIssue(context,
					"no last name causes exception",
					"Adding an author with a valid first name andate, but no last name throws an unhandled exception \"Last name out of range\" insteaof validating the input before sending it.",
					State.Open,
					"admin@gmail.com",
					"gosho@gmail.com",
					new[] {FindTagByName(context, "user input"),}
				);

				CreateIssue(context,
					"missing first name causes exception",
					"Adding an author with a valid last name and date, but no first name throws an unhandled exception \"First name out of range\" instead of validating the input before sending it.",
					State.Open,
					"admin@gmail.com",
					"gosho@gmail.com",
					new[] {FindTagByName(context, "user input"),}
				);

				CreateIssue(context,
					"short first name it causes exception",
					"If first name is too short, the system throws an unhandled exception \"First name out of range\" instead of validating the input before sending it.",
					State.Open,
					"admin@gmail.com",
					"gosho@gmail.com",
					new[] {FindTagByName(context, "user input"),}
				);

				CreateIssue(context,
					"short last name causes exception",
					"If last name is too short, the system throws an unhandled exception \"Last name out of range\" instead of validating the input before sending it.",
					State.Open,
					"admin@gmail.com",
					"gosho@gmail.com",
					new[] {FindTagByName(context, "user input"),}
				);

				CreateIssue(context,
					"Wrong first name upper limit",
					"First name upper limit is 239 characters instead of 240",
					State.Open,
					"admin@gmail.com",
					"gosho@gmail.com",
					new[] {FindTagByName(context, "user input"),}
				);

				CreateIssue(context,
					"Wrong last name upper limit",
					"Last name upper limit is 237 characters instead of 240",
					State.Open,
					"admin@gmail.com",
					"gosho@gmail.com",
					new[] {FindTagByName(context, "user input"),}
				);
			}

			context.SaveChanges();
		}

		private static Tag FindTagByName(IssueTrackerDbContext context, string name)
		{
			return context.Tags.Find(name);
		}

		private static void CreateUser(IssueTrackerDbContext context,
			string email, string password, string fullName)
		{
			var userManager = new UserManager<ApplicationUser>(
				new UserStore<ApplicationUser>(context))
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

		private static void CreateIssue(IssueTrackerDbContext context, string title, string description, State state,
			string author, string assignee, IEnumerable<Tag> tags)
		{
			var issue = new Issue
			{
				Title = title,
				Description = description,
				State = state,
				Author = context.Users.FirstOrDefault(u => u.UserName == author),
				Assignee = context.Users.FirstOrDefault(u => u.UserName == assignee),
				Tags = tags
			};

			context.Issues.Add(issue);
		}

		private void CreateRole(IssueTrackerDbContext context, string roleName)
		{
			var roleManager = new RoleManager<IdentityRole>(
				new RoleStore<IdentityRole>(context));
			var roleCreateResult = roleManager.Create(new IdentityRole(roleName));
			if (!roleCreateResult.Succeeded)
			{
				throw new Exception(string.Join("; ", roleCreateResult.Errors));
			}
		}

		private static void AddUserToRole(IssueTrackerDbContext context, string userName, string roleName)
		{
			var user = context.Users.First(u => u.UserName == userName);
			var userManager = new UserManager<ApplicationUser>(
				new UserStore<ApplicationUser>(context));
			var addAdminRoleResult = userManager.AddToRole(user.Id, roleName);
			if (!addAdminRoleResult.Succeeded)
			{
				throw new Exception(string.Join("; ", addAdminRoleResult.Errors));
			}
		}
	}
}