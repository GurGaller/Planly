using System;
using System.Threading;
using Planly.Application.Identity;
using Planly.DomainModel;
using Planly.DomainModel.Schedules;
using Planly.DomainModel.Tasks;
using Planly.DomainModel.Time;

namespace Planly.Application.Tasks.Commands.Capture
{
	internal class Executor : ICommandExecutor<CaptureTaskCommand, Guid>
	{
		private readonly IIdentityProvider identityProvider;
		private readonly IScheduleRepository scheduleRepository;
		private readonly ITaskRepository taskRepository;

		public Executor(
			IIdentityProvider identityProvider,
			IScheduleRepository scheduleRepository,
			ITaskRepository taskRepository)
		{
			this.identityProvider = identityProvider;
			this.scheduleRepository = scheduleRepository;
			this.taskRepository = taskRepository;
		}

		public async System.Threading.Tasks.Task<Guid> ExecuteAsync(
			CaptureTaskCommand command, CancellationToken cancellationToken)
		{
			var schedule = await GetScheduleAsync(cancellationToken);

			var task = CaptureTaskInto(schedule, command);

			return task.Id.ToGuid();
		}

		private Task CaptureTaskInto(Schedule schedule, CaptureTaskCommand command)
		{
			var taskDescription = new TaskDescription(command.Title);
			var timeRequired = new Duration(command.TotalTimeRequired);
			var deadline = Deadline.Until(command.Deadline);
			var sessionDuration = new Duration(command.IdealSessionDuration);

			var task = schedule.CaptureTask(taskDescription, deadline, sessionDuration, timeRequired);
			taskRepository.Add(task);

			return task;
		}

		private async System.Threading.Tasks.Task<Schedule> GetScheduleAsync(CancellationToken cancellationToken)
		{
			var userId = identityProvider.GetCurrentUserId();
			var scheduleId = new Identifier<Schedule>(userId);
			var schedule = await scheduleRepository.FindByIdAsync(scheduleId, cancellationToken);
			if (schedule is null)
				throw new Exception("The current user does not an associated schedule.");
			return schedule;
		}
	}
}