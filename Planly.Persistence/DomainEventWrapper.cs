using System;
using System.Text.Json;
using Planly.DomainModel;
using Planly.Persistence.JsonConverters;
using Planly.Web.Server.JsonConverters;

namespace Planly.Persistence
{
	internal class DomainEventWrapper
	{
		private static readonly JsonSerializerOptions jsonOptions = new()
		{
			Converters =
			{
				new DurationConverter(),
				new IdentifierConverterFactory(),
				new DeadlineConverter()
			}
		};

		public string Data { get; set; } = null!;
		public Guid Id { get; set; }
		public DateTimeOffset OccurrenceTime { get; set; }
		public bool SuccessfullyProcessed { get; set; }
		public string Type { get; set; } = null!;

		public static DomainEventWrapper Wrap(DomainEvent domainEvent)
		{
			return new DomainEventWrapper
			{
				Id = domainEvent.EventId,
				Type = domainEvent.GetType().FullName!,
				Data = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), jsonOptions),
				OccurrenceTime = domainEvent.OccurrenceTime
			};
		}

		public DomainEvent GetDomainEvent()
		{
			var eventType = GetEventType();

			return (DomainEvent)JsonSerializer.Deserialize(Data, eventType, jsonOptions)!;
		}

		private Type GetEventType()
		{
			var eventType = null as Type;
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly.FullName!.StartsWith("System.") || assembly.FullName!.StartsWith("Microsoft."))
					continue;

				eventType = assembly.GetType(Type);
				if (eventType is not null)
					break;
			}

			if (eventType is null || !eventType.IsAssignableTo(typeof(DomainEvent)))
				throw new Exception("This type of event for this wrapper could not be found.");

			return eventType;
		}
	}
}