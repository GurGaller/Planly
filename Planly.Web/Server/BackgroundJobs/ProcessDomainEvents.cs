using System;
using System.Threading;
using System.Threading.Tasks;
using Planly.Application.DomainEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Planly.Web.Server.BackgroundJobs
{
	/// <summary>
	/// A background service that processes domain events emitted by commands processed by the system.
	/// </summary>
	internal class ProcessDomainEvents : BackgroundService
	{
		private const int MaxConcurrentTasks = 50;
		private readonly IServiceProvider serviceProvider;

		/// <summary>
		/// Creates a new instance of the processor.
		/// </summary>
		/// <param name="serviceProvider">
		/// A root service provider used for creating DI scopes for event processing.
		/// </param>
		/// <remarks>
		/// Should only be constructed once per application lifecycle.
		/// </remarks>
		public ProcessDomainEvents(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		/// <summary>
		/// Runs the background service.
		/// </summary>
		/// <param name="stoppingToken">A token for shutting down the service.</param>
		/// <remarks>
		/// Call once per application lifecycle.
		/// </remarks>
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				using var scope = serviceProvider.CreateScope();
				var processor = scope.ServiceProvider.GetRequiredService<DomainEventProcessor>();

				var eventsProcessed = await processor.ProcessDomainEventsAsync(MaxConcurrentTasks, stoppingToken);

				if (eventsProcessed < MaxConcurrentTasks)
					await Task.Delay(1000, stoppingToken);
			}
		}
	}
}