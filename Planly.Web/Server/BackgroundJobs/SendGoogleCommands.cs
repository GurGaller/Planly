using System.Threading;
using System.Threading.Tasks;
using Planly.Web.Server.ExternalCalendars.Google;
using Microsoft.Extensions.Hosting;

namespace Planly.Web.Server.BackgroundJobs
{
	internal class SendGoogleCommands : BackgroundService
	{
		private readonly GoogleApiCommandBuffer commandBuffer;

		public SendGoogleCommands(GoogleApiCommandBuffer commandBuffer)
		{
			this.commandBuffer = commandBuffer;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await commandBuffer.SendBatchAsync(stoppingToken);
				await Task.Delay(500, stoppingToken);
			}
		}
	}
}