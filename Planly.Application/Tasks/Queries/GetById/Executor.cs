using System.Threading;
using Planly.Application.Common.Exceptions;
using Planly.Application.Identity;
using Planly.DomainModel;
using Planly.DomainModel.Tasks;

namespace Planly.Application.Tasks.Queries.GetById
{
	internal class Executor : IQueryExecutor<GetTaskByIdQuery, TaskDto>
	{
		private readonly IIdentityProvider identityProvider;
		private readonly ITaskRepository taskRepository;

		public Executor(ITaskRepository taskRepository, IIdentityProvider identityProvider)
		{
			this.taskRepository = taskRepository;
			this.identityProvider = identityProvider;
		}

		public async System.Threading.Tasks.Task<TaskDto> ExecuteAsync(
			GetTaskByIdQuery query, CancellationToken cancellationToken)
		{
			var task = await GetTaskAsync(query, cancellationToken);

			CheckAuthorization(task);

			return TaskDto.From(task);
		}

		private void CheckAuthorization(Task task)
		{
			var userId = identityProvider.GetCurrentUserId();
			if (task.ScheduleId.ToGuid() != userId)
				throw new UnauthorizedRequestException();
		}

		private async System.Threading.Tasks.Task<Task> GetTaskAsync(
			GetTaskByIdQuery query, CancellationToken cancellationToken)
		{
			var taskId = new Identifier<Task>(query.TaskId);
			var task = await taskRepository.FindByIdAsync(taskId, cancellationToken);
			if (task is null)
				throw new ResourceNotFoundException();
			return task;
		}
	}
}