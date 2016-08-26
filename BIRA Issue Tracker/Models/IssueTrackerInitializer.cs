using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BIRA_Issue_Tracker.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BIRA_Issue_Tracker.Models
{
	public class SchoolInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<IssueTrackerDbContext>
	{
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

			
			context.SaveChanges();
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