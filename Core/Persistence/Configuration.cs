using Core.Model;
using Services.DataLoad.ShapeFiles;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Spatial;
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
				var seedDataDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

				IEnumerable<KeyValuePair<string, DbGeography>> shapes;
				try
				{
					shapes = new DagiShapeFileReader().Read(Path.Combine(seedDataDirectory, "data", "KOMMUNE"), "KOMNAVN", "DAGI_ID");
				}
				catch (FileNotFoundException)
				{
					seedDataDirectory = Directory.GetParent(seedDataDirectory).FullName;
					shapes = new DagiShapeFileReader().Read(Path.Combine(seedDataDirectory, "KOMMUNE"), "KOMNAVN", "DAGI_ID");
				}

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
