using System;
using System.ComponentModel.DataAnnotations;

namespace Planly.Web.Client.Models
{
	public class TaskDto
	{
		[Required]
		public DateTimeOffset? Deadline { get; set; }

		public Guid Id { get; set; }

		[Required]
		public TimeSpan? IdealSessionDuration { get; set; }

		public TimeSpan TimeCompleted { get; set; }

		[Required]
		[MaxLength(256)]
		public string Title { get; set; }

		[Required]
		public TimeSpan? TotalTimeRequired { get; set; }
	}
}