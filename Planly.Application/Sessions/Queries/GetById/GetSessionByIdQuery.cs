using System;

namespace Planly.Application.Sessions.Queries.GetById
{
	/// <summary>
	/// A query that returns a specific session.
	/// </summary>
	public record GetSessionByIdQuery(Guid SessionId) : IQuery<SessionDto>;
}