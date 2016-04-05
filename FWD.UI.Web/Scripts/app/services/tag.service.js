define([
	'jquery',
	'app/helpers/appPath.static.helper',
	'app/helpers/configurator.static.helper'
],function ($, appPath, conf) {
	var tagService = function () {
		var that = this;
		that.list = {};
		return {
			getAll: function (rootElement) {
				var fake = [];
				fake.push({
					Id: 1,
					Name: 'tutorial',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				fake.push({
					Id: 2,
					Name: 'годно',
					TagType: 0,
					TagColor: '#FFFF00',
					Priority: 0
				});

				that.list = fake;

				return this;
			},

			success: function (callback) {
				callback(that.list);

				return function () { };
			},

			getCommonInfo: function (rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/tags/info',
					beforeSend: conf.beforeSend(rootElement),
					dataType: 'json',
					type: 'GET'
				});
			},
			save: function (rootElement, tag) {
				return $.ajax({
					url: appPath.getAppPath() + '/tags/save',
					beforeSend: conf.beforeSend(rootElement),
					data: JSON.stringify(tag),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json',
				});
			},
			delete: function (rootElement, id) {
				return $.ajax({
					url: appPath.getAppPath() + '/tags/delete/' + id,
					beforeSend: conf.beforeSend(rootElement),
					type: 'POST'
				});
			},
			getTag: function (rootElement, id) {
				return $.ajax({
					url: appPath.getAppPath() + '/tags/get/' + id,
					beforeSend: conf.beforeSend(rootElement),
					dataType: 'json',
					type: 'GET'
				});
			},
			update: function (rootElement, tag) {
				return $.ajax({
					url: appPath.getAppPath() + '/tags/update',
					beforeSend: conf.beforeSend(rootElement),
					data: JSON.stringify(tag),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json',
				});
			}
		};

	};

	return tagService;
});