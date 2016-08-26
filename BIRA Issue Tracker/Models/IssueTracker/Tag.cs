﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BIRA_Issue_Tracker.Models.IssueTracker
{
	public class Tag
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(30)]
		public string Name { get; set; }

		[Required]
		public List<Tag> Tags { get; set; }
	}
}