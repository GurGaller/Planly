using System;
using System.Threading;
using System.Threading.Tasks;
using Planly.Application.ExternalCalendars;
using Microsoft.EntityFrameworkCore;

namespace Planly.Persistence.ExternalCalendars
{
	internal class EFCoreExternalCalendarKeyStore : IExternalCalendarKeyStore
	{
		private readonly DbSet<ExternalCalendarKey> keys;

		public EFCoreExternalCalendarKeyStore(CustomDbContext dbContext)
		{
			keys = dbContext.Set<ExternalCalendarKey>();
		}

		public async Task<ExternalCalendarKey?> FindAsync(
			Guid calendarId,
			string keyName,
			CancellationToken cancellationToken = default)
		{
			return await keys.FindAsync(new object[] { calendarId, keyName }, cancellationToken);
		}

		public void Remove(ExternalCalendarKey key) => keys.Remove(key);

		public void Store(ExternalCalendarKey key) => keys.Add(key);
	}
}