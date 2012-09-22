using Core.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Core.Persistence
{
	public class Context : DbContext
	{
		public DbSet<District> Districts { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<Context, Configuration>());
		}
	}
}
