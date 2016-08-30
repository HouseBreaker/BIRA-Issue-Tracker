using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BIRA_Issue_Tracker.Models;
using BIRA_Issue_Tracker.Models.IssueTracker;

namespace BIRA_Issue_Tracker.Controllers
{
	public class TagsController : Controller
	{
		private IssueTrackerDbContext db = new IssueTrackerDbContext();

		// GET: Tags
		public ActionResult Index()
		{
			return View(db.Tags.ToList());
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