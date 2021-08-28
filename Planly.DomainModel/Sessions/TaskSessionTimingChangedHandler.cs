using System;
using System.Linq;
using System.Threading;
using Planly.DomainModel.Tasks;

namespace Planly.DomainModel.Sessions
{
	internal class TaskSessionTimingChangedHandler
		: IDomainEventHandler<TaskSessionDurationChanged>,
		IDomainEventHandler<TaskTimeRequirementChanged>,
		IDomainEventHandler<TaskDeadlineMoved>
	{
		private readonly ISessionRepository sessionRepository;
		private readonly SessionScheduler sessionScheduler;
		private readonly ITaskRepository taskRepository;

		public TaskSessionTimingChangedHandler(
			ISessionRepository sessionRepository,
			ITaskRepository taskRepository,
			SessionScheduler sessionScheduler)
		{
			this.sessionRepository = sessionRepository;
			this.taskRepository = taskRepository;
			this.sessionScheduler = sessionScheduler;
		}

		public System.Threading.Tasks.Task HandleAsync(
			TaskSessionDurationChanged changeEvent, CancellationToken cancellationToken)
		{
			return RescheduleSessionsAsync(changeEvent.TaskId, cancellationToken);
		}

		public System.Threading.Tasks.Task HandleAsync(
			TaskTimeRequirementChanged changeEvent, CancellationToken cancellationToken)
		{
			return RescheduleSessionsAsync(changeEvent.TaskId, cancellationToken);
		}

		public System.Threading.Tasks.Task HandleAsync(
			TaskDeadlineMoved moveEvent, CancellationToken cancellationToken)
		{
			return RescheduleSessionsAsync(moveEvent.TaskId, cancellationToken);
		}

		private async System.Threading.Tasks.Task RemoveNotDoneSessionsAsync(
			Identifier<Task> taskId, CancellationToken cancellationToken)
		{
			var taskSessions = await sessionRepository.GetByTaskIdAsync(taskId, cancellationToken);
			var sessionsNotDone = taskSessions.Where(s => !s.Done);

			foreach (var session in sessionsNotDone)
			{
				try
				{
					session.Cancel();
				}
				catch (InvalidOperationException)
				{
					continue;
				}
			}
		}

		private async System.Threading.Tasks.Task RescheduleSessionsAsync(
			Identifier<Task> taskId, CancellationToken cancellationToken)
		{
			await RemoveNotDoneSessionsAsync(taskId, cancellationToken);

			await ScheduleSessionsAsync(taskId, cancellationToken);
		}

		private async System.Threading.Tasks.Task ScheduleSessionsAsync(
			Identifier<Task> taskId, CancellationToken cancellationToken)
		{
			var task = await taskRepository.FindByIdAsync(taskId, cancellationToken);
			if (task is null)
				throw new InvalidOperationException("No such task was found in the repository.");

			await sessionScheduler.ScheduleSessionsAsync(task, cancellationToken);
		}
	}
}