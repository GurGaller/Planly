using System;

namespace Planly.Web.Server.Models
{
	/// <summary>
	/// A serializable representation of a user.
	/// </summary>
	public record UserDto
	{
		public Guid Id { get; init; }
		public bool LockedOut { get; init; }
		public bool Admin { get; init; }
		public string EmailAddress { get; init; }
	}
}