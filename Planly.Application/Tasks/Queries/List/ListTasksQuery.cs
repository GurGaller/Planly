using System.Collections.Generic;

namespace Planly.Application.Tasks.Queries.List
{
	/// <summary>
	/// A query that lists the tasks of the current user, ordered by their deadline.
	/// </summary>
	public record ListTasksQuery(int Limit, int Offset) : IQuery<IReadOnlyList<TaskDto>>;
}