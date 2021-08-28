using System;
using System.Collections.Generic;
using System.Threading;
using Planly.DomainModel.Schedules;
using Planly.DomainModel.Tasks;

namespace Planly.DomainModel.Sessions
{
	/// <inheritdoc/>
	public interface ISessionRepository : IRepository<Session>
	{
		/// <summary>
		/// Retrieves a list of <see cref="Session"/>s by the schedule they belong to,
		/// ordered by their start time. Canceled sessions are excluded.
		/// </summary>
		/// <param name="offset">The number of sessions to skip.</param>
		/// <param name="limit">The maximum number of sessions to return.</param>
		/// <param name="cancellationToken">A token used to cancel the operation.</param>
		/// <returns>The list of <see cref="Session"/>s.</returns>
		System.Threading.Tasks.Task<IReadOnlyList<Session>> GetByScheduleIdAsync(
			Identifier<Schedule> scheduleId,
			int offset,
			int limit,
			DateTimeOffset? firstDate = null,
			DateTimeOffset? lastDate = null,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieves a list of all the <see cref="Session"/>s that belong to a specified <see cref="Task"/>,
		/// ordered by their start time. Canceled sessions are excluded.
		/// </summary>
		/// <param name="taskId">The unique identifier of the <see cref="Task"/>.</param>
		/// <param name="cancellationToken">A token used to cancel the operation.</param>
		/// <returns>The list of <see cref="Session"/>s.</returns>
		System.Threading.Tasks.Task<IReadOnlyList<Session>> GetByTaskIdAsync(
			Identifier<Task> taskId,
			CancellationToken cancellationToken = default);
	}
}