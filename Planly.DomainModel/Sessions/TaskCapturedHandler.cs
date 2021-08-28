using System;
using System.Threading;
using Planly.DomainModel.Tasks;

namespace Planly.DomainModel.Sessions
{
	internal class TaskCapturedHandler : IDomainEventHandler<TaskCaptured>
	{
		private readonly SessionScheduler sessionScheduler;
		private readonly ITaskRepository taskRepository;

		public TaskCapturedHandler(SessionScheduler sessionScheduler, ITaskRepository taskRepository)
		{
			this.sessionScheduler = sessionScheduler;
			this.taskRepository = taskRepository;
		}

		public async System.Threading.Tasks.Task HandleAsync(
			TaskCaptured creationEvent, CancellationToken cancellationToken)
		{
			var task = await taskRepository.FindByIdAsync(creationEvent.TaskId, cancellationToken);
			if (task is null)
				throw new InvalidOperationException("No such task was found in the repository.");

			await sessionScheduler.ScheduleSessionsAsync(task, cancellationToken);
		}
	}
}