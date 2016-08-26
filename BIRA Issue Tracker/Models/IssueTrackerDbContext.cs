using System.Data.Entity;
using BIRA_Issue_Tracker.Models.Identity;
using BIRA_Issue_Tracker.Models.IssueTracker;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BIRA_Issue_Tracker.Models
{
	public class IssueTrackerDbContext : IdentityDbContext<ApplicationUser>
	{
		public IssueTrackerDbContext()
			: base("DefaultConnection", throwIfV1Schema: false)
		{
		}

		public static IssueTrackerDbContext Create()
		{
			return new IssueTrackerDbContext();
		}

		public DbSet<Issue> Issues { get; set; }

		public DbSet<Tag> Tags { get; set; }
	}
}