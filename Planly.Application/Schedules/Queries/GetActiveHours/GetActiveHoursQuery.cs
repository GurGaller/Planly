using System;

namespace Planly.Application.Schedules.Queries.GetActiveHours
{
	/// <summary>
	/// A query that returns the range of hours in which sessions can be scheduled automatically.
	/// </summary>
	public record GetActiveHoursQuery(Guid ScheduleId) : IQuery<ActiveHoursDto>;
}