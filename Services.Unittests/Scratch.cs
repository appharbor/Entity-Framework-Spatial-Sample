﻿using System.Linq;
using System.Data.Spatial;
using System.Xml.Linq;
using Xunit;

namespace Services.UnitTest
{
	public class Scratch
	{
		[Fact]
		public void ParsePoint()
		{
			DbGeography.FromGml(@"<Point xmlns=""http://www.opengis.net/gml""><pos>10 30</pos></Point>");
		}

		[Fact]
		public void ParseMultiPolygonFromText()
		{
			DbGeography.FromText(@"MULTIPOLYGON (((40 40, 20 45, 45 30, 40 40)),
((20 35, 45 20, 30 5, 10 10, 10 30, 20 35),
(30 20, 20 25, 20 15, 30 20)))");
		}

		[Fact]
		public void ParseLinearRingFromText()
		{
			DbGeography.FromText(@"LinearRing (((40 40, 20 45, 45 30, 40 40)))");
		}

		[Fact]
		public void ParsePolygon()
		{
			DbGeography.FromGml(@"<MultiGeometry xmlns=""http://opengis.net/gml"">
	<Polygon>
		<outerBoundaryIs>
			<LinearRing>
				<coordinates>14.7803252828559,55.0511739782992 14.7803739493835,55.0511700395841 14.7808806908795,55.0511285246034 14.7808713746528,55.0510835974668 14.7803948697363,55.0511260126015 14.7803394479516,55.0509324053318 14.7804121244788,55.0508819518665 14.7808706415113,55.050837692909 14.7809324369853,55.050860585117 14.7809961044559,55.0511144740194 14.7812013691271,55.051147094822 14.7812133480148,55.0511019633119 14.7812013381467,55.0510687773391 14.7811998267416,55.0510572365374 14.7811946948014,55.0510455981672 14.7811819327951,55.0510332430618 14.7811545723499,55.0510196930222 14.781122228601,55.0510063803987 14.7811017713036,55.0509939418067 14.7810935015792,55.0509779518381 14.7810892627602,55.0509486266433 14.7810873424025,55.0509266627978 14.7810847636154,55.0508990589495 14.7810778903573,55.0508699492895 14.7810644215799,55.0508461950802 14.7810509392449,55.0508234317554 14.7810340166826,55.0508029028326 14.7810163264644,55.050785741287 14.7809920278092,55.0507738457798 14.7809649893728,55.0507646914291 14.780919339172,55.0507602046886 14.7808810579149,55.0507589677342 14.7808318879692,55.0507594198012 14.7807821835904,55.050763768245 14.7807167086019,55.0507739992251 14.7806471581408,55.0507853245406 14.7805891878557,55.0507888964053 14.7805546337861,55.050783070721 14.7805008999922,55.0507768984158 14.7804330453074,55.0507739193861 14.7803697292395,55.0507763054592 14.7803172352618,55.05077979636 14.7802730567847,55.0507876622688 14.7802388557981,55.0507983836757 14.7802048350152,55.0508114370535 14.7801774833378,55.0508261532119 14.7801612924036,55.0508399776289 14.7801532761911,55.0508550330281 14.7801489811596,55.0508752224488 14.7801514225708,55.0508932005196 14.7801610526728,55.0509120964579 14.7801755253221,55.0509276109572 14.7801886617039,55.050942558957 14.7801957008493,55.0509543765763 14.7801986502613,55.0509693597305 14.7802015575427,55.0509894761306 14.7802050701064,55.0510170355017 14.7802093178434,55.0510518516129 14.7802130656985,55.0510766991236 14.7802221219546,55.0511003035248 14.7802334413465,55.0511156981624 14.7802445947863,55.0511266896393 14.7802597998893,55.051133167066 14.7803164780644,55.0511423499897 14.7803252828559,55.0511739782992 </coordinates>
			</LinearRing>
		</outerBoundaryIs>
	</Polygon>
</MultiGeometry>");
		}

		[Fact]
		public void ParseDistrict()
		{
			var kmlDocument = XDocument.Load("http://geo.oiorest.dk/politikredse/12/grænse.kml");
			var geometry = kmlDocument.Descendants("{http://earth.google.com/kml/2.2}MultiGeometry").Single();
			
			geometry.SetNamespace("http://opengis.net/gml");

			var gmlDocument = XDocument.Parse(geometry.ToString());

			var geography = DbGeography.FromGml(gmlDocument.ToString());
		}
	}
}
