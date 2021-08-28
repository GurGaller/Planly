using System;
using Planly.DomainModel.Tasks;

namespace Planly.Application.Tasks.Queries
{
	/// <summary>
	/// A serializable representation of a task.
	/// </summary>
	public record TaskDto(
		Guid Id,
		string Title,
		TimeSpan TotalTimeRequired,
		TimeSpan TimeCompleted,
		TimeSpan IdealSessionDuration,
		DateTimeOffset Deadline)
	{
		/// <summary>
		/// Converts a task into a serializable DTO representation.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <returns>The constructed DTO.</returns>
		public static TaskDto From(Task task)
		{
			return new TaskDto(
				task.Id.ToGuid(),
				task.Description.Title,
				task.Progress.TimeRequired.ToTimeSpan(),
				task.Progress.TimeCompleted.ToTimeSpan(),
				task.IdealSessionDuration.ToTimeSpan(),
				task.Deadline.Time);
		}
	}
}