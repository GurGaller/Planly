using System;
using Planly.DomainModel.Sessions;

namespace Planly.Application.Sessions.Queries
{
	/// <summary>
	/// A serializable representation of a session.
	/// </summary>
	public record SessionDto(Guid Id, string Title, DateTimeOffset StartTime, DateTimeOffset EndTime)
	{
		/// <summary>
		/// Creates a DTO that represents a given session.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <returns>The constructed DTO.</returns>
		public static SessionDto From(Session session)
		{
			return new SessionDto(
				session.Id.ToGuid(),
				session.Description.Title,
				session.Time.StartTime,
				session.Time.EndTime);
		}
	}
}