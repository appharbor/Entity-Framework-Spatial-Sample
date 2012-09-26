using Core.Model;
using Services.DataLoad.ShapeFiles;
using System;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Core.Persistence
{
	public class Configuration : DbMigrationsConfiguration<Context>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
		}

		protected override void Seed(Context context)
		{
			if (!context.Features.Any())
			{
				var location = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
				var dataPath = Path.Combine(Path.GetDirectoryName(location), "data", "KOMMUNE");
				var shapes = new DagiShapeFileReader().Read(dataPath, "KOMNAVN", "DAGI_ID");
				foreach (var shape in shapes)
				{
					context.Features.Add(new Feature { Name = shape.Key, Geography = shape.Value });
				}
				context.SaveChanges();
			}

			base.Seed(context);
		}
	}
}
