using Services;
using System.Data.Spatial;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Web.Controllers
{
	public class LoadController : Controller
	{
		public ActionResult Index()
		{
			var kmlDocument = XDocument.Load("http://geo.oiorest.dk/politikredse/12/grænse.kml");
			var geometry = kmlDocument.Descendants("{http://earth.google.com/kml/2.2}MultiGeometry").Single();
			
			geometry.SetNamespace("http://opengis.net/gml");

			var gmlDocument = XDocument.Parse(geometry.ToString());

			var geography = DbGeography.FromGml(gmlDocument.ToString());

			return Content(string.Format("District parsed"));
		}

	}
}
