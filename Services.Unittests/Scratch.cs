using Core.Model;
using Core.Persistence;
using Newtonsoft.Json.Linq;
using Services.DataLoad.ShapeFiles.ShapeFileReader;
using System.Data.Spatial;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;
using NetTopologySuite.IO;
using NetTopologySuite.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.Converters.WellKnownText;
using GeoAPI.CoordinateSystems;
using ProjNet.CoordinateSystems;
using GeoAPI.Geometries;
using DotSpatial.Projections;
using NetTopologySuite.CoordinateSystems.Transformation.DotSpatial.Projections;

namespace Services.UnitTest
{
	public class Scratch
	{
		[Fact]
		public void ParseDistrict()
		{
			string responseContent;
			using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }))
			{
				var response = client.GetAsync("http://geo.oiorest.dk/politikredse/12/grænse.json").Result;
				responseContent = response.Content.ReadAsStringAsync().Result;
			}

			var json = JObject.Parse(responseContent);
			var coordinates = json["coordinates"] as JArray;
			var wkt = coordinates.ToWkt();

			var geography = DbGeography.FromText(wkt);
		}

		[Fact]
		public void InsertDistrict()
		{
			RemoveDistricts();

			using (var context = new Context())
			{
				context.Districts.Add(new District { Name = "Foo" });
				context.Districts.Add(new District { Name = "Bar" });
				context.SaveChanges();
			}
			
			using (var context = new Context())
			{
				Assert.Equal(2, context.Districts.Count());
			}
		}

		[Fact]
		public void ReadShapeFile()
		{
			var shapes = new ShapeFileHelper().Read("..\\..\\..\\data\\KOMMUNE", "KOMNAVN", "DAGI_ID").ToList();
		}


		[Fact]
		public void InsertMunicipailitiesFromShapeFile()
		{
			RemoveDistricts();

			var shapes = new ShapeFileHelper().Read("..\\..\\..\\data\\KOMMUNE", "KOMNAVN", "DAGI_ID").ToList();
			//var geometry = shapes.First().Value;

			//var etrs89wkt = "GEOGCS[\"ETRS89\",DATUM[\"European_Terrestrial_Reference_System_1989\",SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],AUTHORITY[\"EPSG\",\"6258\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4258\"]],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",9],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],AUTHORITY[\"EPSG\",\"25832\"],AXIS[\"Easting\",EAST],AXIS[\"Northing\",NORTH]";
			//var wgs84wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]";

			//var etrs89projection = ProjectionInfo.FromEsriString(etrs89wkt);
			//var wgs84projection = ProjectionInfo.FromEsriString(wgs84wkt);
			//var dotSpatialTransform = new DotSpatialMathTransform(etrs89projection, wgs84projection);

			//var etrs89 = CoordinateSystemWktReader.Parse(etrs89wkt) as ICoordinateSystem;
			//var projNetTransform = new CoordinateTransformationFactory().CreateFromCoordinateSystems(etrs89, GeographicCoordinateSystem.WGS84);

			//var transformedGeometry = GeometryTransform.TransformGeometry(
			//	GeometryFactory.Default, geometry, dotSpatialTransform);

			//var writer = new WKTWriter();
			using (var context = new Context())
			{
				foreach (var shape in shapes)
				{
					context.Districts.Add(new District { Name = shape.Key, Geography = DbGeography.FromText(shape.Value.AsText()) });
				}
				context.SaveChanges();
			}
		}

		[Fact]
		public void TestConversion()
		{
			var point = GeometryFactory.Default.CreatePoint(new Coordinate(492155.73, 6303867.82));
			var transform = new CoordinateTransformationFactory().CreateFromCoordinateSystems(
				ProjectedCoordinateSystem.WGS84_UTM(32, true), GeographicCoordinateSystem.WGS84);
			var transformedPoint = GeometryTransform.TransformGeometry(GeometryFactory.Default, point, transform.MathTransform);
		}

		private void RemoveDistricts()
		{
			using (var context = new Context())
			{
				foreach (var district in context.Districts)
				{
					context.Districts.Remove(district);
				}
				context.SaveChanges();
			}
		}

		//[Fact]
		//public void ReadAndTransform()
		//{
		//	var transform = new CoordinateTransformation
		//	// epsg25832
		//	var shapes = new ShapeFileHelper().Read("..\\..\\..\\data\\KOMMUNE", "KOMNAVN", "DAGI_ID");
		//	shapes.Select(x => 
		//}
	}
}
