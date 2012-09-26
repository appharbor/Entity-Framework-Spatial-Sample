using System.Data.Spatial;

namespace Core.Model
{
	public class Feature : Entity
	{
		public string Name { get; set; }
		public DbGeography Geography { get; set; }
	}
}
