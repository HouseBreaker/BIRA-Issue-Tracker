using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using BIRA_Issue_Tracker.Models.Identity;

namespace BIRA_Issue_Tracker.Models.IssueTracker
{
	public class Issue
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(200)]
		public string Title { get; set; }

		[DataType(DataType.MultilineText)]
		public string Description { get; set; }

		[Required]
		public State State { get; set; }

		[Required]
		public virtual ICollection<Tag> Tags { get; set; }

		[Required]
		public virtual ApplicationUser Author { get; set; }
		
		public virtual ApplicationUser Assignee { get; set; }

		[Required]
		public DateTime Date { get; set; }
	}
}