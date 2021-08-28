using Planly.DomainModel.Schedules;

namespace Planly.Persistence.Repositories
{
	internal class EFCoreScheduleRepository : EFCoreRepository<Schedule>, IScheduleRepository
	{
		public EFCoreScheduleRepository(CustomDbContext dbContext) : base(dbContext)
		{
		}
	}
}