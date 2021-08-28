using System;
using System.Threading;
using Planly.DomainModel;
using Planly.DomainModel.ExternalCalendars;
using Planly.DomainModel.Sessions;

namespace Planly.Application.ExternalCalendars.DomainEventHandler
{
	internal class SessionScheduledHandler : IDomainEventHandler<SessionScheduled>
	{
		private readonly IExternalCalendarRepository externalCalendarRepository;
		private readonly ISessionRepository sessionRepository;
		private readonly ICalendarSynchronizerFactory synchronizerFactory;

		public SessionScheduledHandler(
			IExternalCalendarRepository externalCalendarRepository,
			ICalendarSynchronizerFactory synchronizerFactory,
			ISessionRepository sessionRepository)
		{
			this.externalCalendarRepository = externalCalendarRepository;
			this.synchronizerFactory = synchronizerFactory;
			this.sessionRepository = sessionRepository;
		}

		public async System.Threading.Tasks.Task HandleAsync(
			SessionScheduled schedulingEvent, CancellationToken cancellationToken)
		{
			var session = await GetSessionAsync(schedulingEvent, cancellationToken);

			await SyncToExternalCalendarsAsync(session, cancellationToken);
		}

		private async System.Threading.Tasks.Task<Session> GetSessionAsync(
			SessionScheduled schedulingEvent,
			CancellationToken cancellationToken)
		{
			var session = await sessionRepository.FindByIdAsync(schedulingEvent.SessionId, cancellationToken);
			if (session is null)
				throw new InvalidOperationException("The session could not be found.");
			return session;
		}

		private async System.Threading.Tasks.Task SyncSessionToAsync(
			ExternalCalendar externalCalendar,
			Session session,
			CancellationToken cancellationToken)
		{
			var synchronizer = await synchronizerFactory.GetSynchronizerForAsync(externalCalendar, cancellationToken);
			await synchronizer.CreateEntryAsync(session, cancellationToken);
		}

		private async System.Threading.Tasks.Task SyncToExternalCalendarsAsync(
			Session session,
			CancellationToken cancellationToken)
		{
			var externalCalendars = await externalCalendarRepository.GetByScheduleIdAsync(
				session.ScheduleId,
				cancellationToken);

			foreach (var externalCalendar in externalCalendars)
				await SyncSessionToAsync(externalCalendar, session, cancellationToken);
		}
	}
}