using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Planly.DomainModel;

namespace Planly.Persistence.JsonConverters
{
	internal class IdentifierConverter<TEntity> : JsonConverter<Identifier<TEntity>>
		where TEntity : Entity<TEntity>
	{
		public override Identifier<TEntity>? Read(
			ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var input = reader.GetString();
			if (input is null)
				return null;
			var internalId = Guid.Parse(input);
			return new Identifier<TEntity>(internalId);
		}

		public override void Write(Utf8JsonWriter writer, Identifier<TEntity> value, JsonSerializerOptions options)
		{
			var field = value.GetType().GetField("internalId", BindingFlags.Instance | BindingFlags.NonPublic)!;
			var internalId = (Guid)field.GetValue(value)!;
			writer.WriteStringValue(internalId.ToString());
		}
	}
}