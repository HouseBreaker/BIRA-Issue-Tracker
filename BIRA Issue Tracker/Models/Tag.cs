using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BIRA_Issue_Tracker.Models
{
	public class Tag
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(6)]
		public string Name { get; set; }

		[Required]
		public List<Tag> Tags { get; set; }
	}
}