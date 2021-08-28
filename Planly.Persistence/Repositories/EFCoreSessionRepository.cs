using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Planly.DomainModel;
using Planly.DomainModel.Schedules;
using Planly.DomainModel.Sessions;
using Microsoft.EntityFrameworkCore;

namespace Planly.Persistence.Repositories
{
	internal class EFCoreSessionRepository : EFCoreRepository<Session>, ISessionRepository
	{
		public EFCoreSessionRepository(CustomDbContext dbContext) : base(dbContext)
		{
		}

		public async Task<IReadOnlyList<Session>> GetByScheduleIdAsync(
			Identifier<Schedule> scheduleId,
			int offset,
			int limit,
			DateTimeOffset? firstDate = null,
			DateTimeOffset? lastDate = null,
			CancellationToken cancellationToken = default)
		{
			firstDate ??= DateTimeOffset.MinValue;
			lastDate ??= DateTimeOffset.MaxValue;
			var sessions = await Entities
				.Where(s => s.ScheduleId == scheduleId)
				.Where(s => !s.Canceled)
				.Where(s => s.Time.EndTime >= firstDate)
				.Where(s => s.Time.StartTime <= lastDate)
				.OrderBy(s => s.Time.StartTime)
				.Skip(offset)
				.Take(limit)
				.ToListAsync(cancellationToken);

			return sessions.AsReadOnly();
		}

		public async Task<IReadOnlyList<Session>> GetByTaskIdAsync(
			Identifier<DomainModel.Tasks.Task> taskId, CancellationToken cancellationToken = default)
		{
			var sessions = await Entities
				.Where(s => s.TaskId == taskId)
				.Where(s => !s.Canceled)
				.OrderBy(s => s.Time.StartTime)
				.ToListAsync(cancellationToken);

			return sessions.AsReadOnly();
		}
	}
}