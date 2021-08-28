using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Planly.DomainModel;

namespace Planly.Persistence.JsonConverters
{
	internal class IdentifierConverterFactory : JsonConverterFactory
	{
		public override bool CanConvert(Type typeToConvert)
		{
			if (!typeToConvert.IsGenericType)
				return false;

			return typeToConvert.GetGenericTypeDefinition() == typeof(Identifier<>);
		}

		public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
		{
			var entityType = typeToConvert.GetGenericArguments().Single();
			var converterType = typeof(IdentifierConverter<>).MakeGenericType(entityType);
			return (JsonConverter?)Activator.CreateInstance(converterType);
		}
	}
}