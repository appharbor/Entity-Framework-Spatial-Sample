using System.Data.Objects.SqlClient;
using Core.Persistence;
using System.Data.Spatial;
using System.Linq;
using System.Web.Mvc;

namespace Web.Controllers
{
	public class FeatureController : Controller
	{
		Context _context = new Context();

		public ActionResult Show(double latitude, double longitude)
		{
			var point = DbGeography.FromText(string.Format("POINT ({0} {1})", longitude, latitude), 4326);
			var district = _context.Districts
				.Where(x => x.Geography.Intersects(point))
				.Select(x => new { Name = x.Name, Geography = SqlSpatialFunctions.Reduce(x.Geography, 100).AsText() })
				.SingleOrDefault();

			object result = new { };
			if (district != null)
			{
				result = new
				{
					name = district.Name,
					geography = district.Geography,
				};
			}
			
			return Json(result, JsonRequestBehavior.AllowGet);
		}
	}
}
