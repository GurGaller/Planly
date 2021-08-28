using System;
using System.Threading;
using Planly.DomainModel;
using Planly.DomainModel.ExternalCalendars;
using Planly.DomainModel.Sessions;

namespace Planly.Application.ExternalCalendars.DomainEventHandler
{
	internal class SessionCanceledHandler : IDomainEventHandler<SessionCanceled>
	{
		private readonly IExternalCalendarRepository externalCalendarRepository;
		private readonly ISessionRepository sessionRepository;
		private readonly ICalendarSynchronizerFactory synchronizerFactory;

		public SessionCanceledHandler(
			IExternalCalendarRepository externalCalendarRepository,
			ICalendarSynchronizerFactory synchronizerFactory,
			ISessionRepository sessionRepository)
		{
			this.externalCalendarRepository = externalCalendarRepository;
			this.synchronizerFactory = synchronizerFactory;
			this.sessionRepository = sessionRepository;
		}

		public async System.Threading.Tasks.Task HandleAsync(
			SessionCanceled domainEvent, CancellationToken cancellationToken)
		{
			var session = await sessionRepository.FindByIdAsync(domainEvent.SessionId, cancellationToken);
			if (session is null)
				throw new InvalidOperationException("Could not find the session.");

			var externalCalendars = await externalCalendarRepository.GetByScheduleIdAsync(
				session.ScheduleId,
				cancellationToken);

			foreach (var externalCalendar in externalCalendars)
			{
				var synchronizer = await synchronizerFactory.GetSynchronizerForAsync(
					externalCalendar, cancellationToken);
				await synchronizer.RemoveEntryAssociatedWithSessionAsync(session.Id, cancellationToken);
			}
		}
	}
}