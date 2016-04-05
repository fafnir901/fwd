define([
	'jquery',
	'app/helpers/configurator.static.helper',
	'app/helpers/notification.static.helper',
	'signalr.hubs'
], function($, conf, errorHelper) {
	var articleHub = (function () {
		return {
			init: function (article) {
				var articles = $.connection.articlesHub;
				$.connection.hub.stop();

				articles.off("sendMessage");
				articles.on("sendMessage", function (message) {
					var encodedName = $('<div />').text(message.articleName).html();
					var date = $('<div />').text(message.date).html();
					var id = $('<div />').text(message.id).html();
					var item = {
						isUpdate: message.isUpdate,
						articleName: encodedName,
						date: date,
						id: id,
						middleMessage: message.isUpdate ? 'обновилась' : 'добавилась',
						message: 'Статья "' + message.articleName + '" ' + (message.isUpdate ? 'обновилась' : 'добавилась')
					};
					item.message = message.isDeleted ? 'Статья "' + message.articleName + '" удалилась' : item.message;
					if (!conf.isShow) {
						var counter = $('.buttons_container button:first');
						conf.attentionOnElement(counter).then(function () {
							errorHelper.showInfo(item.message);
						});
					}
				});

				$.connection.hub.start({ transport: ['webSockets', 'longPolling'] }).done(function () {
					var obj = { isUpdate: article.dto.isUpdate, articleName: article.dto.ArticleName, isDeleted: article.dto.isDeleted };
					articles.server.notifyThatArticleWasAddedOrUpdated(obj);
				});
			}
		};
	})();
	return articleHub;
});