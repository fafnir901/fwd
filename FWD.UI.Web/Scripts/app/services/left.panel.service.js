define([
	'jquery',
	'app/helpers/configurator.static.helper',
	'app/helpers/appPath.static.helper'
], function ($, conf, appPath) {
	var leftPanelService = function () {
		return {
			getNames: function (dateTime, rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/panel',
					beforeSend: conf.beforeSend(rootElement),
					dataType: 'json',
					type: 'GET'
				});
			},
			getGroups: function (dateTime, rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/groups',
					beforeSend: conf.beforeSend(rootElement),
					dataType: 'json',
					type: 'GET'
				});
			}
		};

	};

	return leftPanelService;
});