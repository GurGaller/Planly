using System;
using System.Threading;
using System.Threading.Tasks;
using Planly.DomainModel;
using Planly.DomainModel.ExternalCalendars;
using Planly.DomainModel.Sessions;

namespace Planly.Application.ExternalCalendars.DomainEventHandler
{
	internal class SyncLocalSessionsToExternalCalendarUponConnection : IDomainEventHandler<ExternalCalendarConnected>
	{
		private readonly IExternalCalendarRepository externalCalendarRepository;
		private readonly ISessionRepository sessionRepository;
		private readonly ICalendarSynchronizerFactory synchronizerFactory;

		public SyncLocalSessionsToExternalCalendarUponConnection(
			ISessionRepository sessionRepository,
			IExternalCalendarRepository externalCalendarRepository,
			ICalendarSynchronizerFactory synchronizerFactory)
		{
			this.sessionRepository = sessionRepository;
			this.externalCalendarRepository = externalCalendarRepository;
			this.synchronizerFactory = synchronizerFactory;
		}

		public async Task HandleAsync(ExternalCalendarConnected connectionEvent, CancellationToken cancellationToken)
		{
			var localSessions = await sessionRepository.GetByScheduleIdAsync(
				connectionEvent.ScheduleId,
				offset: 0,
				limit: int.MaxValue,
				firstDate: DateTimeOffset.UtcNow,
				cancellationToken: cancellationToken);

			var synchronizer = await GetCalendarSychronizerAsync(connectionEvent.CalendarId, cancellationToken);

			await synchronizer.CreateEntriesAsync(localSessions, cancellationToken);
		}

		private async Task<ICalendarSynchronizer> GetCalendarSychronizerAsync(
			Identifier<ExternalCalendar> calendarId,
			CancellationToken cancellationToken)
		{
			var externalCalendar = await externalCalendarRepository.FindByIdAsync(calendarId, cancellationToken);
			if (externalCalendar is null)
				throw new Exception("There is no external calendar with the given ID.");

			return await synchronizerFactory.GetSynchronizerForAsync(externalCalendar, cancellationToken);
		}
	}
}