using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Planly.Application.Identity;
using Planly.DomainModel;
using Planly.DomainModel.Schedules;
using Planly.DomainModel.Tasks;

namespace Planly.Application.Tasks.Queries.List
{
	internal class Executor : IQueryExecutor<ListTasksQuery, IReadOnlyList<TaskDto>>
	{
		private readonly IIdentityProvider identityProvider;
		private readonly ITaskRepository taskRepository;

		public Executor(ITaskRepository taskRepository, IIdentityProvider identityProvider)
		{
			this.taskRepository = taskRepository;
			this.identityProvider = identityProvider;
		}

		public async System.Threading.Tasks.Task<IReadOnlyList<TaskDto>> ExecuteAsync(
			ListTasksQuery query, CancellationToken cancellationToken)
		{
			var userId = identityProvider.GetCurrentUserId();
			var scheduleId = new Identifier<Schedule>(userId);

			var tasks = await taskRepository.GetByScheduleIdAsync(
				scheduleId,
				query.Offset,
				query.Limit,
				cancellationToken);

			return tasks
				.Select(TaskDto.From)
				.ToList()
				.AsReadOnly();
		}
	}
}