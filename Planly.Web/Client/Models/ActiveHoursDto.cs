using System;

namespace Planly.Web.Client.Models
{
	public class ActiveHoursDto
	{
		public TimeSpan End { get; set; }
		public string ScheduleId { get; init; }
		public TimeSpan Start { get; set; }
	}
}