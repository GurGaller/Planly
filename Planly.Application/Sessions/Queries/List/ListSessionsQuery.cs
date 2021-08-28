using System;
using System.Collections.Generic;

namespace Planly.Application.Sessions.Queries.List
{
	/// <summary>
	/// Lists sessions of the current user.
	/// </summary>
	public record ListSessionsQuery(int Limit, int Offset, DateTimeOffset? RangeStartTime = null)
		: IQuery<IReadOnlyList<SessionDto>>;
}