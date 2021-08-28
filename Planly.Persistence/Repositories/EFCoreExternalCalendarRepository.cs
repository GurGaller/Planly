using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Planly.DomainModel;
using Planly.DomainModel.ExternalCalendars;
using Planly.DomainModel.Schedules;
using Microsoft.EntityFrameworkCore;

namespace Planly.Persistence.Repositories
{
	internal class EFCoreExternalCalendarRepository : EFCoreRepository<ExternalCalendar>, IExternalCalendarRepository
	{
		public EFCoreExternalCalendarRepository(CustomDbContext dbContext) : base(dbContext)
		{
		}

		public async Task<ExternalCalendar?> FindAsync(
			Identifier<Schedule> scheduleId,
			CalendarProvider calendarProvider,
			CancellationToken cancellationToken = default)
		{
			return await Entities
				.Where(c => c.ScheduleId == scheduleId)
				.Where(c => c.Provider.Name == calendarProvider.Name)
				.SingleOrDefaultAsync(cancellationToken);
		}

		public async Task<IReadOnlyCollection<ExternalCalendar>> GetByScheduleIdAsync(
			Identifier<Schedule> scheduleId, CancellationToken cancellationToken = default)
		{
			var calendars = await Entities
				.Where(c => c.ScheduleId == scheduleId)
				.Where(c => c.Connected)
				.ToListAsync(cancellationToken);

			return calendars.AsReadOnly();
		}
	}
}