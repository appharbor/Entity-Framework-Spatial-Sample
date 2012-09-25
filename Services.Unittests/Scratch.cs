using Core.Model;
using Core.Persistence;
using GeoAPI.Geometries;
using NetTopologySuite.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using Services.DataLoad.ShapeFiles;
using System.Data.Spatial;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;

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

			var shapes = new DagiShapeFileReader().Read("..\\..\\..\\data\\KOMMUNE", "KOMNAVN", "DAGI_ID").ToList();
			using (var context = new Context())
			{
				foreach (var shape in shapes)
				{
					context.Districts.Add(new District { Name = shape.Key, Geography = shape.Value });
					context.SaveChanges();
				}
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

		[Fact]
		public void TestRead()
		{
			using (var context = new Context())
			{
				var foo = context.Districts.First();
			}
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
	}
}
