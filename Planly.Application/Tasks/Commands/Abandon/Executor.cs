using System.Threading;
using Planly.Application.Common.Exceptions;
using Planly.Application.Identity;
using Planly.Application.Validation;
using Planly.DomainModel;
using Planly.DomainModel.Tasks;

namespace Planly.Application.Tasks.Commands.Abandon
{
	internal class Executor : ICommandExecutor<AbandonTaskCommand>
	{
		private readonly IIdentityProvider identityProvider;
		private readonly ITaskRepository taskRepository;

		public Executor(ITaskRepository taskRepository, IIdentityProvider identityProvider)
		{
			this.taskRepository = taskRepository;
			this.identityProvider = identityProvider;
		}

		public async System.Threading.Tasks.Task ExecuteAsync(
			AbandonTaskCommand command, CancellationToken cancellationToken)
		{
			var task = await GetTaskAsync(command, cancellationToken);

			CheckAuthorization(task);

			if (task.Progress.Completed)
			{
				var error = new RequestValidationError(
					Code: "CannotAbandonCompletedTasks",
					Message: "This task is completed so it cannot be abandoned.",
					Target: nameof(command.TaskId));
				throw new InvalidRequestException(error);
			}

			task.Abandon();
		}

		private void CheckAuthorization(Task task)
		{
			var userId = identityProvider.GetCurrentUserId();
			if (task.ScheduleId.ToGuid() != userId)
				throw new UnauthorizedRequestException();
		}

		private async System.Threading.Tasks.Task<Task> GetTaskAsync(AbandonTaskCommand command, CancellationToken cancellationToken)
		{
			var taskId = new Identifier<Task>(command.TaskId);
			var task = await taskRepository.FindByIdAsync(taskId, cancellationToken);
			if (task is null)
				throw new ResourceNotFoundException();
			return task;
		}
	}
}