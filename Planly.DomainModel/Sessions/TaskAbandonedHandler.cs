using System;
using System.Threading;
using Planly.DomainModel.Tasks;

namespace Planly.DomainModel.Sessions
{
	internal class TaskAbandonedHandler : IDomainEventHandler<TaskAbandoned>
	{
		private readonly ISessionRepository sessionRepository;

		public TaskAbandonedHandler(ISessionRepository sessionRepository)
		{
			this.sessionRepository = sessionRepository;
		}

		public async System.Threading.Tasks.Task HandleAsync(
			TaskAbandoned domainEvent, CancellationToken cancellationToken)
		{
			var associatedSessions = await sessionRepository.GetByTaskIdAsync(domainEvent.TaskId, cancellationToken);

			foreach (var session in associatedSessions)
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
	}
}