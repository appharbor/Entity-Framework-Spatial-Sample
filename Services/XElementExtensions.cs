using System.Linq;
using System.Xml.Linq;

namespace Services
{
	public static class XElementExtensions
	{
		public static void SetNamespace(this XElement xelem, XNamespace xmlns)
		{
			xelem.Name = xmlns + xelem.Name.LocalName;
			
			foreach (var element in xelem.Elements())
				element.SetNamespace(xmlns);
		}
	}
}
