using System.Collections.Generic;
using System.Threading;
using Planly.DomainModel.Schedules;

namespace Planly.DomainModel.Tasks
{
	/// <inheritdoc/>
	public interface ITaskRepository : IRepository<Task>
	{
		/// <summary>
		/// Retrieves a list of <see cref="Task"/>s by the <see cref="Schedule"/> they belong to,
		/// ordered by their <see cref="Deadline"/>. Abandoned tasks are excluded.
		/// </summary>
		/// <param name="scheduleId">The <see cref="Schedule"/>'s ID.</param>
		/// <param name="offset">The number of tasks to skip.</param>
		/// <param name="limit">The maximum number of tasks to return.</param>
		/// <param name="cancellationToken">A token used to cancel the operation.</param>
		/// <returns>The list of <see cref="Task"/>s.</returns>
		System.Threading.Tasks.Task<IReadOnlyList<Task>> GetByScheduleIdAsync(
			Identifier<Schedule> scheduleId,
			int offset,
			int limit,
			CancellationToken cancellationToken = default);
	}
}