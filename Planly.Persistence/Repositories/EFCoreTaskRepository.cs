using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Planly.DomainModel;
using Planly.DomainModel.Schedules;
using Planly.DomainModel.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Planly.Persistence.Repositories
{
	internal class EFCoreTaskRepository : EFCoreRepository<Task>, ITaskRepository
	{
		public EFCoreTaskRepository(CustomDbContext dbContext) : base(dbContext)
		{
		}

		public async System.Threading.Tasks.Task<IReadOnlyList<Task>> GetByScheduleIdAsync(
			Identifier<Schedule> scheduleId,
			int offset,
			int limit,
			CancellationToken cancellationToken = default)
		{
			var tasks = await Entities
				.Where(t => t.ScheduleId == scheduleId)
				.Where(t => !t.Abandoned)
				.OrderBy(t => t.Deadline)
				.Skip(offset)
				.Take(limit)
				.ToListAsync(cancellationToken);

			return tasks.AsReadOnly();
		}
	}
}