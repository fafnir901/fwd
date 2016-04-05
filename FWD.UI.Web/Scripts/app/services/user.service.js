define([
	'jquery',
	'app/helpers/appPath.static.helper',
	'app/helpers/configurator.static.helper'
], function ($, appPath, configurator) {
	var userService = function () {
		var getUrl = function () {
			return appPath.getAppPath() + '/user';
		};

		return {
			getUserEditor: function (rootElement) {
				var url = getUrl() + '/editor';
				return $.ajax({
					url: url,
					beforeSend: configurator.beforeSend(rootElement),
					dataType: 'json',
					type: 'GET'
				});
			},

			saveAvatar: function (rootElement) {
				var url = getUrl() + '/saveAvatar';
				return $.ajax({
					url: url,
					beforeSend: configurator.beforeSend(rootElement),
					type: 'POST'
				});
			},
			getCurrentUserData: function () {
				var url = getUrl() + '/currentUser';
				return $.ajax({
					url: url,
					dataType: 'json',
					type: 'GET',
				});
			},
			updateUserData: function (user, rootElement) {
				var url = getUrl() + '/updateUserData';
				return $.ajax({
					url: url,
					beforeSend: configurator.beforeSend(rootElement),
					data: JSON.stringify(user),
					dataType: 'json',
					type: 'POST',
					contentType: 'application/json',
				});
			}
		};
	};
	return userService;
});