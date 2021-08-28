using System;
using System.Threading;
using System.Threading.Tasks;
using Planly.DomainModel;
using Planly.DomainModel.ExternalCalendars;
using Planly.DomainModel.Schedules;

namespace Planly.Application.ExternalCalendars.Commands.Connect
{
	internal class Executor : ICommandExecutor<ConnectExternalCalendarCommand>
	{
		private readonly CalendarConnector calendarConnector;
		private readonly IScheduleRepository scheduleRepository;

		public Executor(CalendarConnector calendarConnector, IScheduleRepository scheduleRepository)
		{
			this.calendarConnector = calendarConnector;
			this.scheduleRepository = scheduleRepository;
		}

		public async Task ExecuteAsync(ConnectExternalCalendarCommand command, CancellationToken cancellationToken)
		{
			var scheduleId = new Identifier<Schedule>(command.UserId);
			var schedule = await scheduleRepository.FindByIdAsync(scheduleId, cancellationToken);
			if (schedule is null)
				throw new Exception("The current user does not have an associated schedule.");

			await calendarConnector.ConnectAsync(new CalendarProvider(command.Provider), schedule, cancellationToken);
		}
	}
}