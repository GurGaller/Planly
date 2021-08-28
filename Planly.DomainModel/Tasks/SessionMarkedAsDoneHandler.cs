using System;
using System.Threading;
using Planly.DomainModel.Sessions;

namespace Planly.DomainModel.Tasks
{
	internal class SessionMarkedAsDoneHandler : IDomainEventHandler<SessionMarkedAsDone>
	{
		private readonly ISessionRepository sessionRepository;
		private readonly ITaskRepository taskRepository;

		public SessionMarkedAsDoneHandler(ISessionRepository sessionRepository, ITaskRepository taskRepository)
		{
			this.sessionRepository = sessionRepository;
			this.taskRepository = taskRepository;
		}

		public async System.Threading.Tasks.Task HandleAsync(
			SessionMarkedAsDone domainEvent, CancellationToken cancellationToken)
		{
			var session = await sessionRepository.FindByIdAsync(domainEvent.SessionId, cancellationToken);
			if (session is null)
				throw new InvalidOperationException("Could not find the session.");

			if (!session.BelongsToTask)
				return;

			var task = await taskRepository.FindByIdAsync(session.TaskId, cancellationToken);
			if (task is null)
				throw new InvalidOperationException("Could not find the task associated with the session.");

			task.TrackProgress(session.Time.Duration);
		}
	}
}