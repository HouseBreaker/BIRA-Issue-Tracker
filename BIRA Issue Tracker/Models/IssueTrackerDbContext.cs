using BIRA_Issue_Tracker.Models.Identity;
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

		public System.Data.Entity.DbSet<Issue> Issues { get; set; }
	}
}