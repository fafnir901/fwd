define([
	'jquery',
	'underscore',
	'app/helpers/configurator.static.helper',
	'app/helpers/appPath.static.helper'
], function ($, _, conf, appPath) {
	var statService = function () {
		return {
			getData: function (rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/articles/stat/fullStat',
					beforeSend: conf.beforeSend(rootElement),
					dataType: 'json',
					type: 'GET'
				});
			},
			getDataForShedule: function (rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/articles/stat/shedule',
					beforeSend: conf.beforeSend(rootElement),
					dataType: 'json',
					type: 'GET'
				});
			},

			getTagDataForRadialShedule: function (rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/tags/stat/shedule/radial',
					beforeSend: conf.beforeSend(rootElement),
					dataType: 'json',
					type: 'GET'
				});
			}
		};
	};

	return statService;
});