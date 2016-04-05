define([
	'jquery',
	'app/helpers/appPath.static.helper',
	'app/helpers/configurator.static.helper'
], function ($, appPath, conf) {
	var articleService = function () {
		var getUrl = function () {
			return appPath.getAppPath() + '/articles';
		};

		return {
			getArticleById: function (id, dateTime, rootElement) {
				var url = getUrl() + '/articleID-' + id;
				if (url != window.location) {
					var anotherUrl = appPath.getAppPath() + '/entity/type-article/id-' + id;
					window.history.pushState(null, null, anotherUrl);
				}
				return $.ajax({
					url: url,
					beforeSend: conf.beforeSend(rootElement),
					//data: 'id=' + id,
					dataType: 'json',
					type: 'GET'
				});
			},
			getStat: function (id, rootElement) {
				var url = getUrl() + '/stat/' + id;
				return $.ajax({
					url: url,
					beforeSend: conf.beforeSend(rootElement),
					//data: 'id=' + id,
					dataType: 'json',
					type: 'GET'
				});
			},
			saveArticle: function (article, dateTime, rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/saveArticle',
					beforeSend: conf.beforeSend(rootElement),
					data: JSON.stringify(article),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json',
				});
			},
			updateArticelRate: function (articelId, rate, rootElement) {
				return $.ajax({
					url: getUrl() + '/updateArticleRate',
					beforeSend: conf.beforeSend(rootElement),
					data: JSON.stringify({ articleId: articelId, rate: rate }),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json',
				});
			},
			updateArticle: function (article, dateTime, rootElement) {
				return $.ajax({
					url: getUrl() + '/updateArticle',
					beforeSend: conf.beforeSend(rootElement),
					data: JSON.stringify(article),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json',
				});
			},
			saveGroup: function (groupName, rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/save_group',
					beforeSend: conf.beforeSend(rootElement),
					data: JSON.stringify({ group: groupName }),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json',
				});
			},
			saveArticleByRef: function (link, rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/saveArticleByRef',
					beforeSend: conf.beforeSend(rootElement),
					data: JSON.stringify({ link: link }),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json',
				});
			},
			deleteArticle: function (id, dateTime, rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/deleteArticle/' + id,
					beforeSend: conf.beforeSend(rootElement),
					type: 'POST'
				});
			},
			sendEmail: function (dateTime, rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/sendEmail',
					beforeSend: conf.beforeSend(rootElement),
					type: 'POST'
				});
			},
			sendThisEmail: function (articleId, mailTo, subject, rootElement) {
				var preparedUrl = getUrl() + '/mail';
				return $.ajax({
					url: preparedUrl,
					beforeSend: conf.beforeSend(rootElement),
					type: 'POST',
					data: JSON.stringify({ id: articleId, mail: mailTo, subject: subject }),
					dataType: 'json',
					contentType: 'application/json'
				});
			}
		};
	};
	return articleService;
});