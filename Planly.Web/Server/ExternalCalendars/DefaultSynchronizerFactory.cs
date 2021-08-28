using System;
using System.Threading;
using System.Threading.Tasks;
using Planly.Application.ExternalCalendars;
using Planly.DomainModel.ExternalCalendars;
using Planly.Web.Server.ExternalCalendars.Google;
using Microsoft.AspNetCore.Authentication.Google;

namespace Planly.Web.Server.ExternalCalendars
{
	internal class DefaultSynchronizerFactory : ICalendarSynchronizerFactory
	{
		private readonly GoogleSynchronizerFactory googleSynchronizerFactory;

		public DefaultSynchronizerFactory(GoogleSynchronizerFactory googleSynchronizerFactory)
		{
			this.googleSynchronizerFactory = googleSynchronizerFactory;
		}

		public async Task<ICalendarSynchronizer> GetSynchronizerForAsync(
			ExternalCalendar externalCalendar, CancellationToken cancellationToken = default)
		{
			var userId = externalCalendar.ScheduleId.ToGuid().ToString();

			if (externalCalendar.Provider.Name is GoogleDefaults.AuthenticationScheme)
				return await googleSynchronizerFactory.CreateAsync(userId, cancellationToken);

			throw new NotSupportedException("The calendar's provider is not supported by this factory.");
		}
	}
}