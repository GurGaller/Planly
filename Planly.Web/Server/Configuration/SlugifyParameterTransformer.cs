using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace Planly.Web.Server.Configuration
{
	internal class SlugifyParameterTransformer : IOutboundParameterTransformer
	{
		public string? TransformOutbound(object? value)
		{
			var text = value?.ToString();
			if (text is null)
				return null;

			return Slugify(text);
		}

		private static string Slugify(string value)
		{
			var slug = Regex.Replace(
				value,
				"([a-z])([A-Z])",
				"$1-$2",
				RegexOptions.None,
				matchTimeout: TimeSpan.FromMilliseconds(100));

			return slug.ToLower();
		}
	}
}