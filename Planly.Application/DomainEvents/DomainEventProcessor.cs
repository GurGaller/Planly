using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Planly.Application.Tranactions;
using Planly.DomainModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Planly.Application.DomainEvents
{
	/// <summary>
	/// Processes batches of unprocessed domain events.
	/// </summary>
	public class DomainEventProcessor
	{
		private static readonly MethodInfo HandleMethod;
		private readonly IDomainEventStore eventStore;
		private readonly ILogger<DomainEventProcessor> logger;
		private readonly IServiceProvider rootServiceProvider;

		static DomainEventProcessor()
		{
			HandleMethod = typeof(DomainEventProcessor)
				.GetMethod(nameof(HandleEventAsync), BindingFlags.Instance | BindingFlags.NonPublic)!;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DomainEventProcessor"/> class.
		/// </summary>
		/// <param name="rootServiceProvider">A service provider used for finding event handlers.</param>
		/// <param name="eventStore">An event store used for loading unprocessed events and marking them as processed.</param>
		/// <param name="logger">A logger for logging exceptions thrown by event handlers.</param>
		public DomainEventProcessor(
			IServiceProvider rootServiceProvider,
			IDomainEventStore eventStore,
			ILogger<DomainEventProcessor> logger)
		{
			this.rootServiceProvider = rootServiceProvider;
			this.eventStore = eventStore;
			this.logger = logger;
		}

		/// <summary>
		/// Processes a batch of domain events.
		/// </summary>
		/// <param name="limit">The maximum size of the batch.</param>
		/// <param name="cancellationToken">A token for canceling the operation.</param>
		/// <returns>The number of events this batch tried to handle (including events whose processing failed).</returns>
		public async Task<int> ProcessDomainEventsAsync(int limit, CancellationToken cancellationToken = default)
		{
			var newEvents = await eventStore.GetUnprocessedEventsAsync(limit, cancellationToken);

			var processingTasks = newEvents.Select(async e =>
			{
				using var scope = rootServiceProvider.CreateScope();
				await ProcessEventAsync(e, scope.ServiceProvider, cancellationToken);
			});

			await Task.WhenAll(processingTasks);

			return newEvents.Count;
		}

		private async Task HandleEventAsync<TEvent>(
			TEvent domainEvent,
			IDomainEventHandler<TEvent> handler,
			CancellationToken stoppingToken) where TEvent : DomainEvent
		{
			try
			{
				await handler.HandleAsync(domainEvent, stoppingToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error occurred while handling event {@event} by {eventHandler}", domainEvent, handler);
			}
		}

		private async Task ProcessEventAsync(
			DomainEvent domainEvent,
			IServiceProvider serviceProvider,
			CancellationToken cancellationToken)
		{
			var unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
			await using var unitOfWork = await unitOfWorkFactory.CreateAsync(cancellationToken);

			var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
			var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);
			var handlers = (IEnumerable<object>)serviceProvider.GetRequiredService(handlerCollectionType);

			var closedHandleMethod = HandleMethod.MakeGenericMethod(domainEvent.GetType());

			foreach (var handler in handlers)
				await (Task)closedHandleMethod.Invoke(this, new object[] { domainEvent, handler, cancellationToken })!;

			var eventStore = serviceProvider.GetRequiredService<IDomainEventStore>();
			eventStore.MarkAsProcessed(domainEvent);

			await unitOfWork.CompleteAsync(cancellationToken);
		}
	}
}