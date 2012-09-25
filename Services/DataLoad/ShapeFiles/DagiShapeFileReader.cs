using DotSpatial.Projections;
using GeoAPI.Geometries;
using Microsoft.SqlServer.Types;
using NetTopologySuite.CoordinateSystems.Transformation.DotSpatial.Projections;
using NetTopologySuite.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Data.Spatial;
using System.Data.SqlTypes;
using System.Linq;

namespace Services.DataLoad.ShapeFiles
{
	public class DagiShapeFileReader
	{
		private const string _wgs84wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]";
		private const string _etrs89utm32wkt = "PROJCS[\"ETRS89 / UTM zone 32N\",GEOGCS[\"ETRS89\",DATUM[\"D_ETRS_1989\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",9],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],UNIT[\"Meter\",1]]";

		private readonly DotSpatialMathTransform _transform;

		public DagiShapeFileReader()
		{
			var etrs89utmprojection = ProjectionInfo.FromEsriString(_etrs89utm32wkt);
			var wgs84projection = ProjectionInfo.FromEsriString(_wgs84wkt);
			_transform = new DotSpatialMathTransform(etrs89utmprojection, wgs84projection);
		}

		public IEnumerable<KeyValuePair<string, DbGeography>> Read(string filePath, string nameColumnIdentifier,
			string idColumnIdentifier)
		{
			var shapeFileHelper = new ShapeFileHelper();
			var shapes = shapeFileHelper.Read(filePath, nameColumnIdentifier, idColumnIdentifier);
			
			return shapes.AsParallel().Select(x =>
			{
				var transformedGeography = GeometryTransform.TransformGeometry(
						GeometryFactory.Default, x.Value, _transform);

				var sqlGeography = SqlGeography.STGeomFromText(new SqlChars(transformedGeography.AsText()), 4326)
						.MakeValid();

				var invertedSqlGeography = sqlGeography.ReorientObject();
				if (sqlGeography.STArea() > invertedSqlGeography.STArea())
				{
					sqlGeography = invertedSqlGeography;
				}

				var dbGeography = DbSpatialServices.Default.GeographyFromProviderValue(sqlGeography);

				return new KeyValuePair<string, DbGeography>(x.Key, dbGeography);
			});
		}
	}
}
