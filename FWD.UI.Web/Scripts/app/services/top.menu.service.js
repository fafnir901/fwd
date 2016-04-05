define([
	'jquery',
	'app/helpers/appPath.static.helper',
	'app/helpers/configurator.static.helper',
], function ($, appPath, conf) {
	var topMenuService = function () {
		return {
			toggleSource: function (source, dateTime, rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/topMenu/' + source,
					beforeSend: conf.beforeSend(rootElement),
					dataType: 'json',
					type: 'POST'
				});
			},
			saveToXMl: function (articleId, rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/saveToXml',
					beforeSend: conf.beforeSend(rootElement),
					data: JSON.stringify({ articleId: articleId }),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json',
				});
			},
			saveToDb: function (articleId, rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/saveToDb',
					beforeSend: conf.beforeSend(rootElement),
					data: JSON.stringify({ articleId: articleId }),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json',
				});
			},
			getInfo: function (rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/getInfo',
					beforeSend: conf.beforeSend(rootElement),
					type: 'GET',
					dataType: 'json',
				});
			},
			importData: function (data, rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/import',
					beforeSend: conf.beforeSend(rootElement),
					cache: false,
					data: data,
					contentType: false, //'multipart/form-data',
					processData: false,
					type: 'POST',
				});
			},
		};
	};
	return topMenuService;
});