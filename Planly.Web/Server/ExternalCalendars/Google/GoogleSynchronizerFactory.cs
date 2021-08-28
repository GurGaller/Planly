using System.Threading;
using System.Threading.Tasks;

namespace Planly.Web.Server.ExternalCalendars.Google
{
	internal class GoogleSynchronizerFactory
	{
		private readonly GoogleCalendarServiceFactory calendarServiceFactory;
		private readonly GoogleApiCommandBuffer commandBuffer;

		public GoogleSynchronizerFactory(
			GoogleCalendarServiceFactory calendarServiceFactory,
			GoogleApiCommandBuffer commandBuffer)
		{
			this.calendarServiceFactory = calendarServiceFactory;
			this.commandBuffer = commandBuffer;
		}

		public async Task<GoogleCalendarSynchronizer> CreateAsync(string userId, CancellationToken cancellationToken)
		{
			var calendarService = await calendarServiceFactory.CreateAsync(userId, cancellationToken);

			return new GoogleCalendarSynchronizer(calendarService, commandBuffer);
		}
	}
}