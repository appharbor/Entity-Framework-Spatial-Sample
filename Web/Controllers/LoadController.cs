using System.Web.Mvc;

namespace Web.Controllers
{
	public class LoadController : Controller
	{
		public ActionResult Index()
		{
			return Content(string.Format("District parsed"));
		}
	}
}
