using System.Threading;
using System.Threading.Tasks;
using Planly.DomainModel;
using Planly.DomainModel.Schedules;

namespace Planly.Application.Schedules.Commands.Create
{
	internal class Executor : ICommandExecutor<CreateScheduleCommand>
	{
		private readonly IScheduleRepository scheduleRepository;

		public Executor(IScheduleRepository scheduleRepository)
		{
			this.scheduleRepository = scheduleRepository;
		}

		public Task ExecuteAsync(CreateScheduleCommand command, CancellationToken cancellationToken)
		{
			var scheduleId = new Identifier<Schedule>(command.UserId);
			var schedule = new Schedule(scheduleId);
			scheduleRepository.Add(schedule);

			return Task.CompletedTask;
		}
	}
}