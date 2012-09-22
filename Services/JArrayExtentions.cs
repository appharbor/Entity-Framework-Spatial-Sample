using Newtonsoft.Json.Linq;
using System.Text;
using System.Linq;

namespace Services
{
	public static class JArrayExtentions
	{
		public static string ToWkt(this JArray coordinates)
		{
			var result = new StringBuilder("MULTIPOLYGON(");
			result.Append(string.Join(",", coordinates.Children().Select(polygon =>
				"(" + string.Join(",", (polygon).Select(polygonPath =>
					"(" + string.Join(",", polygonPath.Select(coordinate =>
						string.Format("{0} {1}", coordinate[0], coordinate[1])
					)) + ")"
				)) + ")"
			)));
			result.Append(")");

			return result.ToString();
		}
	}
}
