using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BIRA_Issue_Tracker.Models.IssueTracker
{
	public class Tag
	{
		public Tag(string name)
		{
			this.Name = name;
			this.Slug = GenerateSlug(name);
		}

		public Tag()
		{
		}

		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(30)]
		public string Name { get; set; }

		[Required]
		[StringLength(30)]
		public string Slug { get; set; }

		public virtual ICollection<Issue> Issues { get; set; }

		private static string GenerateSlug(string name)
		{
			return Regex.Replace(name, @"[^a-zA-Zа-яА-Я\d]+", "-").ToLowerInvariant();
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}