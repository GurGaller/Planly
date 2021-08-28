using System.Threading;
using Planly.DomainModel.Tasks;

namespace Planly.DomainModel.Sessions
{
	internal class TaskDescriptionEditedHandler : IDomainEventHandler<TaskDescriptionEdited>
	{
		private readonly ISessionRepository sessionRepository;

		public TaskDescriptionEditedHandler(ISessionRepository sessionRepository)
		{
			this.sessionRepository = sessionRepository;
		}

		public async System.Threading.Tasks.Task HandleAsync(
			TaskDescriptionEdited editingEvent, CancellationToken cancellationToken)
		{
			var taskSessions = await sessionRepository.GetByTaskIdAsync(editingEvent.TaskId, cancellationToken);

			foreach (var session in taskSessions)
			{
				var newSessionDescription = editingEvent.NewDescription.DescribeSession();
				session.EditDescription(newSessionDescription);
			}
		}
	}
}