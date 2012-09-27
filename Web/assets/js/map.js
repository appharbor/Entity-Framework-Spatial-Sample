$().ready(function () {
	var map = new OpenLayers.Map({
		div: "map",
		displayProjection: "EPSG:4326",
		layers: [
			new OpenLayers.Layer.Stamen("watercolor")
		]
	});

	var bounds = new OpenLayers.Bounds([7.81, 54.52, 12.82, 57.87]);
	map.zoomToExtent(bounds.transform(map.displayProjection, map.getProjectionObject()));

	var vector = new OpenLayers.Layer.Vector("Vector Layer", {
		projection: map.displayProjection,
		styleMap: new OpenLayers.StyleMap({'default':{
			fillColor: '#000000',
			fillOpacity: 0.5,
			label: '${name}'
		}})
	});

	map.addLayer(vector);

	map.events.register("click", map, function (event) {
		var lonLat = map.getLonLatFromPixel(event.xy)
			.transform(map.getProjectionObject(), map.displayProjection);
		$.ajax({
			url: '/feature/show',
			data: {
				latitude: lonLat.lat,
				longitude: lonLat.lon
			}
		}).done(function (result) {
			var reader = new OpenLayers.Format.WKT({
				'internalProjection': map.getProjectionObject(),
				'externalProjection': map.displayProjection
			});

			var feature = reader.read(result.geography);
			feature.attributes = {
				name: result.name
			};

			vector.removeAllFeatures();
			vector.addFeatures(feature);

			map.zoomToExtent(vector.getDataExtent());
		});
	});
});
