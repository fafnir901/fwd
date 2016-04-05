define([
	'jquery',
	'app/helpers/appPath.static.helper',
	'app/helpers/configurator.static.helper'
], function ($, appPath, configurator) {
	var searchService = function () {
		var getUrl = function () {
			return appPath.getAppPath() + '/search';
		};

		return {
			search: function (searchString, rootElement) {
				var url = getUrl() + '/getResult';
				return $.ajax({
					url: url,
					beforeSend: configurator.beforeSend(rootElement),
					data: 'searchString=' + searchString,
					type: 'POST'
				});
			},
			searchByTagIds: function (ids, rootElement) {
				var url = getUrl() + '/byTags/getResult';
				return $.ajax({
					traditional: true,
					url: url,
					beforeSend: configurator.beforeSend(rootElement),
					data: { ids: ids },
					type: 'POST',
					//dataType: 'json',
					//contentType: 'application/json',
				});
			}
		};
	};
	return searchService;
});