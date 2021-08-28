using System;

namespace Planly.Application.Tasks.Queries.GetById
{
	/// <summary>
	/// A query that returns a task from the current user's schedule by its ID.
	/// </summary>
	public record GetTaskByIdQuery(Guid TaskId) : IQuery<TaskDto>;
}