using DotSpatial.Projections;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using NetTopologySuite.CoordinateSystems.Transformation.DotSpatial.Projections;
using NetTopologySuite.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using ProjNet.Converters.WellKnownText;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.SqlServer.Types;

namespace Services.DataLoad.ShapeFiles.ShapeFileReader
{
	public class ShapeFileHelper
	{
		public IEnumerable<KeyValuePair<string, IGeometry>> Read(string filePath, string nameColumnIdentifier,
			string idColumnIdentifier)
		{
			var shapes = ReadShapes(filePath, nameColumnIdentifier, idColumnIdentifier);

			var groupedShapes = from x in shapes
								group x by x.Item3 into g
								select new
								{
									geometry = (IGeometry)GeometryFactory.Default.CreateMultiPolygon(
										g.Select(y => (IPolygon)y.Item1).ToArray()),
									name = g.First().Item2,
								};

			return groupedShapes.ToDictionary(x => x.name, x => x.geometry);
		}

		private IEnumerable<Tuple<IGeometry, string, double>> ReadShapes(string filePath, string nameColumnIdentifier,
			string idColumnIdentifier)
		{
			//var etrs89wkt = "GEOGCS[\"ETRS89\",DATUM[\"European_Terrestrial_Reference_System_1989\",SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],AUTHORITY[\"EPSG\",\"6258\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4258\"]],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",9],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],AUTHORITY[\"EPSG\",\"25832\"],AXIS[\"Easting\",EAST],AXIS[\"Northing\",NORTH]";
			var wgs84wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]";
			var etrs89utm32wkt = "PROJCS[\"ETRS89 / UTM zone 32N\",GEOGCS[\"ETRS89\",DATUM[\"D_ETRS_1989\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",9],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],UNIT[\"Meter\",1]]";

			//var etrs89projection = ProjectionInfo.FromEsriString(etrs89wkt);
			var etrs89utmprojection = ProjectionInfo.FromEsriString(etrs89utm32wkt);
			var wgs84projection = ProjectionInfo.FromEsriString(wgs84wkt);
			var dotSpatialTransform = new DotSpatialMathTransform(etrs89utmprojection, wgs84projection);

			//var etrs89 = CoordinateSystemWktReader.Parse(etrs89wkt) as ICoordinateSystem;
			//var etrs89utm = CoordinateSystemWktReader.Parse(etrs89utmwkt) as ICoordinateSystem;
			//var projNetTransform = new CoordinateTransformationFactory().CreateFromCoordinateSystems(etrs89utm, GeographicCoordinateSystem.WGS84).MathTransform;

			//var transform = new CoordinateTransformationFactory().CreateFromCoordinateSystems(
			//	ProjectedCoordinateSystem.WGS84_UTM(32, true), GeographicCoordinateSystem.WGS84);

			using (var reader = new ShapefileDataReader(filePath, GeometryFactory.Default))
			{
				while (reader.Read())
				{
					var transformedGeometry = GeometryTransform.TransformGeometry(GeometryFactory.Default, reader.Geometry,
						dotSpatialTransform);
					yield return Tuple.Create(transformedGeometry, (string)reader[nameColumnIdentifier],
						(double)reader[idColumnIdentifier]);
				}
			}
		}
	}
}
