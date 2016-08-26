using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BIRA_Issue_Tracker.Models.IssueTracker
{
	public class Tag
	{
		public Tag(string name)
		{
			this.Name = name;
		}

		public Tag()
		{
		}

		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(30)]
		public string Name { get; set; }

		public override string ToString()
		{
			return this.Name;
		}
	}
}