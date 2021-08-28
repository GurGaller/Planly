using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Planly.DomainModel.ExternalCalendars;
using Planly.Persistence;
using Planly.Web.Server.ExternalCalendars.Google;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Planly.Web.Server.BackgroundJobs
{
	internal class ImportChangesFromGoogle : BackgroundService
	{
		private const int ConcurrentCalendarsLimit = 1;
		private readonly ILogger<ImportChangesFromGoogle> logger;
		private readonly IServiceProvider serviceProvider;

		public ImportChangesFromGoogle(IServiceProvider serviceProvider, ILogger<ImportChangesFromGoogle> logger)
		{
			this.serviceProvider = serviceProvider;
			this.logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			using var scope = serviceProvider.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<CustomDbContext>();
			var offset = 0;
			while (!stoppingToken.IsCancellationRequested)
			{
				var calendars = await dbContext.Set<ExternalCalendar>()
					.Where(c => c.Connected)
					.Where(c => c.Provider.Name == GoogleDefaults.AuthenticationScheme)
					.OrderBy(c => c.Id)
					.Skip(offset)
					.Take(ConcurrentCalendarsLimit)
					.ToListAsync(stoppingToken);

				if (calendars.Count < ConcurrentCalendarsLimit)
					offset = 0;
				else
					offset += ConcurrentCalendarsLimit;

				var importTasks = calendars.Select(async calendar =>
				{
					using var importScope = scope.ServiceProvider.CreateScope();
					var changeImporter = importScope.ServiceProvider.GetRequiredService<GoogleCalendarChangeImporter>();
					try
					{
						await changeImporter.ImportChangesAsync(calendar, stoppingToken);
					}
					catch (Exception ex)
					{
						logger.LogCritical(ex, "Failed to import changes from calendar {CalendarId}", calendar.Id.ToGuid());
					}
					var scopedDbContext = importScope.ServiceProvider.GetRequiredService<CustomDbContext>();
					await scopedDbContext.SaveChangesAsync(stoppingToken);
				});
				await Task.WhenAll(importTasks);

				await Task.Delay(500, stoppingToken);
			}
		}
	}
}