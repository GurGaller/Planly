using System.Threading;
using Planly.Application.Common.Exceptions;
using Planly.Application.Identity;
using Planly.Application.Validation;
using Planly.DomainModel;
using Planly.DomainModel.Tasks;
using Planly.DomainModel.Time;

namespace Planly.Application.Tasks.Commands.EditDetails
{
	internal class Executor : ICommandExecutor<EditTaskDetailsCommand>
	{
		private readonly IIdentityProvider identityProvider;
		private readonly ITaskRepository taskRepository;

		public Executor(ITaskRepository taskRepository, IIdentityProvider identityProvider)
		{
			this.taskRepository = taskRepository;
			this.identityProvider = identityProvider;
		}

		public async System.Threading.Tasks.Task ExecuteAsync(
			EditTaskDetailsCommand command, CancellationToken cancellationToken)
		{
			var task = await GetTaskAsync(command, cancellationToken);

			CheckAuthorization(task);

			if (task.Abandoned)
			{
				var error = new RequestValidationError(
					Code: "TaskAbandoned",
					Message: "Cannot edit the details of an abandoned task.",
					Target: nameof(command.Id));
				throw new InvalidRequestException(error);
			}

			EditDetails(command, task);
		}

		private static void EditDetails(EditTaskDetailsCommand command, Task task)
		{
			var timeRequired = new Duration(command.TotalTimeRequired);
			task.RequireTime(timeRequired);

			var deadline = Deadline.Until(command.Deadline);
			task.MoveDeadline(deadline);

			var description = task.Description with { Title = command.Title };
			task.EditDescription(description);

			var idealSessionDuration = new Duration(command.IdealSessionDuration);
			task.ChangeSessionDuration(idealSessionDuration);
		}

		private void CheckAuthorization(Task task)
		{
			var userId = identityProvider.GetCurrentUserId();
			if (task.ScheduleId.ToGuid() != userId)
				throw new UnauthorizedRequestException();
		}

		private async System.Threading.Tasks.Task<Task> GetTaskAsync(
			EditTaskDetailsCommand command, CancellationToken cancellationToken)
		{
			var taskId = new Identifier<Task>(command.Id);
			var task = await taskRepository.FindByIdAsync(taskId, cancellationToken);
			if (task is null)
				throw new ResourceNotFoundException();
			return task;
		}
	}
}