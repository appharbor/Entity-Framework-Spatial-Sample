using Core.Model;
using Core.Persistence;
using Newtonsoft.Json.Linq;
using System.Data.Spatial;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Services.DataLoad.OioRest
{
	public class PoliceDistricts
	{
		public void Load()
		{
			using (var context = new Context())
			{
				foreach (var district in context.Districts)
				{
					context.Districts.Remove(district);
				}
				context.SaveChanges();
			}

			var districts = JArray.Parse(GetResource("http://geo.oiorest.dk/politikredse.json"))
				.Select(x => new
				{
					url = x["grænse"].ToString(),
					name = x["navn"].ToString(),
				});

			var parsedDistricts = districts.AsParallel().Select(x =>
			{
				var response = GetResource(x.url);
				var coordinates = JObject.Parse(response)["coordinates"];
				var wkt = (coordinates as JArray).ToWkt();
				return new District
				{
					Name = x.name,
					Geography = DbGeography.FromText(wkt, 4326),
				};
			});

			using (var context = new Context())
			{
				parsedDistricts.ForAll(x => context.Districts.Add(x));
				context.SaveChanges();
			}
		}

		private string GetResource(string url)
		{
			string responseContent;
			using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }))
			{
				var response = client.GetAsync(url).Result;
				responseContent = response.Content.ReadAsStringAsync().Result;
			}

			return responseContent;
		}
	}
}
