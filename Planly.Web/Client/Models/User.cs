using System;

namespace Planly.Web.Client.Models
{
	public class User
	{
		public bool Admin { get; set; }
		public string EmailAddress { get; init; }
		public Guid Id { get; init; }
		public bool LockedOut { get; set; }
	}
}