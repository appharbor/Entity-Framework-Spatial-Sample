using DotSpatial.Projections;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using NetTopologySuite.CoordinateSystems.Transformation.DotSpatial.Projections;
using NetTopologySuite.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.DataLoad.ShapeFiles
{
	public class ShapeFileHelper
	{
		public IEnumerable<KeyValuePair<string, IGeometry>> Read(string filePath, string nameColumnIdentifier,
			string idColumnIdentifier)
		{
			var shapes = ReadShapes(filePath, nameColumnIdentifier, idColumnIdentifier);

			//return shapes.Select(x => new KeyValuePair<string, IGeometry>(x.Item2, x.Item1));

			var groups = from x in shapes
						 group x by x.Item3 into g
						 select g;

			var result = groups
				//.Where(x => x.Count() == 1).Take(1)
				.AsParallel().Select(x =>
			{
				var name = x.First().Item2;
				var geometry = (IGeometry)GeometryFactory.Default.CreateMultiPolygon(x.Select(y => (IPolygon)y.Item1).ToArray());
				return new KeyValuePair<string, IGeometry>(name, geometry);
			});

			return result;
		}

		private IEnumerable<Tuple<IGeometry, string, double>> ReadShapes(string filePath, string nameColumnIdentifier,
			string idColumnIdentifier)
		{
			using (var reader = new ShapefileDataReader(filePath, GeometryFactory.Default))
			{
				while (reader.Read())
				{
					yield return Tuple.Create(reader.Geometry, (string)reader[nameColumnIdentifier],
						(double)reader[idColumnIdentifier]);
				}
			}
		}
	}
}
