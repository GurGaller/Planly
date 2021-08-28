using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Planly.DomainModel.Time;

namespace Planly.Web.Server.JsonConverters
{
	internal class DurationConverter : JsonConverter<Duration>
	{
		public override Duration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return new Duration(TimeSpan.FromTicks(reader.GetInt64()));
		}

		public override void Write(Utf8JsonWriter writer, Duration value, JsonSerializerOptions options)
		{
			writer.WriteNumberValue(value.ToTimeSpan().Ticks);
		}
	}
}