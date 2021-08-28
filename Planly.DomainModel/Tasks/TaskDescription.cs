using Planly.DomainModel.Sessions;

namespace Planly.DomainModel.Tasks
{
	/// <summary>
	/// Describes a <see cref="Task"/>
	/// </summary>
	public record TaskDescription(string Title)
	{
		/// <summary>
		/// Describes a session for working on the <see cref="Task"/>.
		/// </summary>
		/// <returns>The <see cref="Session"/>'s description.</returns>
		internal SessionDescription DescribeSession()
		{
			return new SessionDescription(Title);
		}
	}
}