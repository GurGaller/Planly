using System;
using Google.Apis.Calendar.v3.Data;

namespace Planly.Web.Server.ExternalCalendars.Google
{
	internal static class GoogleTimeExtensions
	{
		public static DateTimeOffset ToDateTimeOffset(this EventDateTime eventDateTime)
		{
			return new DateTimeOffset(eventDateTime.DateTime!.Value);
		}

		public static EventDateTime ToGoogleEventTime(this DateTimeOffset time)
		{
			return new EventDateTime
			{
				DateTime = time.UtcDateTime,
				TimeZone = "Etc/UTC"
			};
		}
	}
}