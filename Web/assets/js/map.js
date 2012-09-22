$().ready(function () {
	var map = new OpenLayers.Map({
		div: "map",
		layers: [
			new OpenLayers.Layer.Stamen("watercolor")
		]
	});

	var bounds = new OpenLayers.Bounds([8.10791, 54.547217, 12.799072, 57.787404]);
	map.zoomToExtent(bounds.transform(new OpenLayers.Projection("EPSG:4326"), map.getProjectionObject()));

	map.events.register("click", map, function (event) {
		var lonLat = map.getLonLatFromPixel(event.xy)
			.transform(map.getProjectionObject(), new OpenLayers.Projection("EPSG:4326"));
		$.ajax({
			url: '/district/show',
			data: {
				latitude: lonLat.lat,
				longitude: lonLat.lon
			}
		}).done(function (result) {
			alert('Name: ' + result.name);
		});
	});
});
