using Core.Model;
using Core.Persistence;
using GeoAPI.Geometries;
using NetTopologySuite.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using Services.DataLoad.ShapeFiles;
using System.Linq;
using Xunit;

namespace Services.UnitTest
{
	public class Scratch
	{
		private const string skipReason = "Integration test";

		[Fact(Skip=skipReason)]
		public void ReadShapeFile()
		{
			var shapes = new ShapeFileHelper().Read("..\\..\\..\\data\\KOMMUNE", "KOMNAVN", "DAGI_ID").ToList();
		}

		[Fact(Skip = skipReason)]
		public void InsertMunicipailitiesFromShapeFile()
		{
			RemoveDistricts();

			var shapes = new DagiShapeFileReader().Read("..\\..\\..\\data\\KOMMUNE", "KOMNAVN", "DAGI_ID");
			using (var context = new Context())
			{
				foreach (var shape in shapes)
				{
					context.Features.Add(new Feature { Name = shape.Key, Geography = shape.Value });
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
				foreach (var district in context.Features)
				{
					context.Features.Remove(district);
				}
				context.SaveChanges();
			}
		}
	}
}
