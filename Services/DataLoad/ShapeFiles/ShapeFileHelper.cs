using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Collections.Generic;
using System.Linq;

namespace Services.DataLoad.ShapeFiles
{
	public class ShapeFileHelper
	{
		public IEnumerable<KeyValuePair<string, IGeometry>> Read(string filePath, string nameColumnIdentifier)
		{
			return from x in ReadShapes(filePath, nameColumnIdentifier)
				   group x by x.Key into g
				   select new KeyValuePair<string, IGeometry>(g.Key,
						(IGeometry)GeometryFactory.Default.CreateMultiPolygon(g.Select(y => (IPolygon)y.Value).ToArray()));
		}

		private IEnumerable<KeyValuePair<string, IGeometry>> ReadShapes(string filePath, string nameColumnIdentifier)
		{
			using (var reader = new ShapefileDataReader(filePath, GeometryFactory.Default))
			{
				while (reader.Read())
				{
					yield return new KeyValuePair<string, IGeometry>((string)reader[nameColumnIdentifier], reader.Geometry);
				}
			}
		}
	}
}
