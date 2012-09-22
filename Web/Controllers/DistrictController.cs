using Core.Persistence;
using System.Web.Mvc;
using System.Linq;
using System.Data.Spatial;

namespace Web.Controllers
{
	public class DistrictController : Controller
	{
		Context _context = new Context();

		public ActionResult Show(double latitude, double longitude)
		{
			var point = DbGeography.FromText(string.Format("POINT ({0} {1})", longitude, latitude), 4326);
			var district = _context.Districts.SingleOrDefault(x => x.Geography.Intersects(point));

			if (district != null)
			{
				return Json(new
				{
					name = district.Name,
				});
			}
			
			return Json(new { });
		}
	}
}
