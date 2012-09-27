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
			var district = _context.Features
				.Where(x => x.Geography.Intersects(point))
				.Select(x => new { Name = x.Name, Geography = SqlSpatialFunctions.Reduce(x.Geography, 100).AsText() })
				.SingleOrDefault();

			if (district != null)
			{
				return Json(new
				{
					name = district.Name,
					geography = district.Geography,
				}, JsonRequestBehavior.AllowGet);
			}

			Response.StatusCode = 404;
			Response.TrySkipIisCustomErrors = true;
			return Json(new { }, JsonRequestBehavior.AllowGet);
		}
	}
}
