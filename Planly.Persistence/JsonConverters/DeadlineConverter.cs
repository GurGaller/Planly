using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Planly.DomainModel.Tasks;

namespace Planly.Persistence.JsonConverters
{
	internal class DeadlineConverter : JsonConverter<Deadline>
	{
		public override Deadline? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var time = reader.GetDateTimeOffset();
			return Deadline.Until(time);
		}

		public override void Write(Utf8JsonWriter writer, Deadline value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.Time);
		}
	}
}