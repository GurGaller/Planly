using System;
using System.ComponentModel.DataAnnotations;

namespace Planly.Web.Client.Models
{
	public class SessionDto
	{
		[Required]
		public DateTimeOffset? EndTime { get; set; }

		public Guid Id { get; set; }

		[Required]
		public DateTimeOffset? StartTime { get; set; }

		[Required]
		[MaxLength(256)]
		public string Title { get; set; }
	}
}