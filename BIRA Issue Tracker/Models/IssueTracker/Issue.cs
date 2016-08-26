using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BIRA_Issue_Tracker.Models.Identity;

namespace BIRA_Issue_Tracker.Models.IssueTracker
{
	public class Issue
	{
		public Issue()
		{
			this.Date = DateTime.Now;
		}

		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(200)]
		public string Title { get; set; }

		public string Description { get; set; }

		[Required]
		public State State { get; set; }

		[Required]
		public IEnumerable<Tag> Tags { get; set; }

		[Required]
		public ApplicationUser Author { get; set; }

		public ApplicationUser Assignee { get; set; }

		[Required]
		public DateTime Date { get; set; }
	}
}