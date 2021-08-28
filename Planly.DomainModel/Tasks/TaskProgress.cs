using Planly.DomainModel.Time;

namespace Planly.DomainModel.Tasks
{
	/// <summary>
	/// Tracks the progress of a <see cref="Task"/>
	/// </summary>
	public record TaskProgress
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TaskProgress"/> class.
		/// </summary>
		/// <param name="timeRequired">The work time required for the <see cref="Task"/>.</param>
		/// <param name="timeCompleted">The amount of time already completed working on the <see cref="Task"/>.</param>
		public TaskProgress(Duration timeRequired, Duration timeCompleted)
		{
			if (timeCompleted.IsLongerThan(timeRequired))
				timeCompleted = timeRequired;

			TimeRequired = timeRequired;
			TimeCompleted = timeCompleted;
		}

		/// <summary>
		/// Gets the work time required for the <see cref="Task"/>.
		/// </summary>
		public Duration TimeRequired { get; }

		/// <summary>
		/// Gets the amount of time already completed working on the <see cref="Task"/>.
		/// </summary>
		public Duration TimeCompleted { get; }

		/// <summary>
		/// Gets the work time left for completing the <see cref="Task"/>.
		/// </summary>
		public Duration TimeLeft => new(TimeRequired.ToTimeSpan() - TimeCompleted.ToTimeSpan());

		internal TaskProgress AddTime(Duration additionalTimeSpent)
		{
			return new TaskProgress(TimeRequired, TimeCompleted + additionalTimeSpent);
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="TaskProgress"/> is completed.
		/// </summary>
		public bool Completed => TimeLeft == Duration.Zero;

		internal TaskProgress WithTimeRequirement(Duration timeRequired)
		{
			return new TaskProgress(timeRequired, TimeCompleted);
		}
	}
}