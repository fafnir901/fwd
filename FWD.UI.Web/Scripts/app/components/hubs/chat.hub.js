define([
	'jquery',
	'app/helpers/configurator.static.helper',
	'app/helpers/notification.static.helper',
	'signalr.hubs'
], function($, conf, errorHelper) {
	var chatHub = (function () {
		return {
			init: function (comment) {
				var chat = $.connection.chatHub;
				$.connection.hub.stop();

				chat.off("sendMessage");
				chat.on("sendMessage", function (message) {
					var encodedName = $('<div />').text(message.name).html();
					var date = $('<div />').text(message.date).html();
					var encodedMsg = $('<div />').text(message.message).html();
					var id = $('<div />').text(message.id).html();
					var item = {
						name: encodedName,
						date: date,
						message: encodedMsg,
						userId: message.userId,
						id: id,
					};
					comment.addComment(item);
					if (!conf.isShow) {
						var buttonComment = $('.comments');
						$.when(conf.attentionOnElement(buttonComment)).then(function () {
							errorHelper.showInfo('Пришел новый комментарий');
						});
					}
				});

				chat.off('removeMessage');
				chat.on('removeMessage', function (answer) {
					if (!conf.isShow) {
						var buttonComment = $('.comments');
						comment.delete(answer.id);
						$.when(conf.attentionOnElement(buttonComment)).then(function () {
							errorHelper.showInfo(answer.message);
						});
					} else {
						var buttonComment = $('.comments');
						comment.delete(answer.id);
					}
				});

				chat.on('connectToGroup', function (result) {
					comment.render(result);
				});

				$.connection.hub.start({ transport: ['webSockets', 'longPolling'] }).done(function () {
					chat.server.joinGroup(window.location.pathname + '?source=' + conf.currentSource);
					chat.server.connectToGroup(window.location.pathname + '?source=' + conf.currentSource);
					comment.onDelete(function (id) {
						chat.server.delete(id, window.location.pathname + '?source=' + conf.currentSource, conf.currentUser.name);
					});
					comment.onSendComment(function () {
						var commentMessage = $('.comment_message');
						var obj = { message: commentMessage.val(), name: conf.currentUser.name, userId: conf.currentUser.userId };
						var errorClass = "error_element";
						commentMessage.removeClass(errorClass);
						if (obj.message.length > 150) {
							commentMessage.addClass(errorClass);
							errorHelper.showError('Длинна сообщения не может привышать 200 символов');
							var mes = commentMessage.val().substring(0, 148);
							commentMessage.val(mes);
							item.message = mes;
						} else if (obj.message.length < 1) {
							commentMessage.addClass(errorClass);
							errorHelper.showError('Сообщение не должно быть пустым');
						}
						else {
							chat.server.send(obj, window.location.pathname + '?source=' + conf.currentSource);
							$('.comment_message').val('').focus();
						}
					});
				});
			}
		};
	})();
	return chatHub;
});