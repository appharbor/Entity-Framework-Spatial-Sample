using Core.Persistence;
using System.Data.Spatial;
using System.Linq;
using System.Web.Mvc;

namespace Web.Controllers
{
	public class DistrictController : Controller
	{
		Context _context = new Context();

		public ActionResult Show(double latitude, double longitude)
		{
			var point = DbGeography.FromText(string.Format("POINT ({0} {1})", longitude, latitude), 4326);
			var district = _context.Districts.SingleOrDefault(x => x.Geography.Intersects(point));

			object result = new { };
			if (district != null)
			{
				result = new
				{
					name = district.Name,
					geography = district.Geography.AsText(),
				};
			}
			
			return Json(result, JsonRequestBehavior.AllowGet);
		}
	}
}
