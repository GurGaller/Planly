using System.Threading;
using System.Threading.Tasks;
using Planly.Application.Common.Exceptions;
using Planly.Application.Identity;
using Planly.DomainModel;
using Planly.DomainModel.Schedules;
using Planly.DomainModel.Time;

namespace Planly.Application.Schedules.Commands.ChangeActiveHours
{
	internal class Executor : ICommandExecutor<ChangeActiveHoursCommand>
	{
		private readonly IIdentityProvider identityProvider;
		private readonly IScheduleRepository scheduleRepository;

		public Executor(IScheduleRepository scheduleRepository, IIdentityProvider identityProvider)
		{
			this.scheduleRepository = scheduleRepository;
			this.identityProvider = identityProvider;
		}

		public async Task ExecuteAsync(ChangeActiveHoursCommand command, CancellationToken cancellationToken)
		{
			VerifyAuthorization(command);

			var scheduleId = new Identifier<Schedule>(command.ScheduleId);
			var schedule = await scheduleRepository.FindByIdAsync(scheduleId, cancellationToken);
			if (schedule is null)
				throw new ResourceNotFoundException();

			var startTime = new TimeOfDay(new Duration(command.Start));
			var endTime = new TimeOfDay(new Duration(command.End));
			var activeHours = new HourRange(startTime, endTime);
			schedule.ChangeActiveHours(activeHours);
		}

		private void VerifyAuthorization(ChangeActiveHoursCommand command)
		{
			var userId = identityProvider.GetCurrentUserId();
			if (userId != command.ScheduleId)
				throw new UnauthorizedRequestException();
		}
	}
}