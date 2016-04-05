define([
	'jquery',
	'underscore',
	'app/helpers/configurator.static.helper',
	'app/helpers/appPath.static.helper'
], function ($, _, conf, appPath) {
	var commentComponent = function () {
		var getModel = function () {
			if (this.model == undefined) {
				this.model = {};
				this.model.container = $('<div class="common_comment_container"></div>');
				this.model.senderContainer = $('<div class="comment_sender_container"></div>');
				this.model.addNewComment = $('<textarea  type="text" class="comment_message" />');
				this.model.sendNewComment = $('<button>Отправить</button>');
				this.model.deleteComment = $('<button class="comment_delete">Удалить</button>');
				this.model.hideContainer = $('<button class="hide_smile_container"/>');

				this.model.smileContainer = $('<div class="smile_container"></div>');
				this.model.simpleSmile = $('<div class="smile simple" data-tooltip="Улыбка"></div>');
				this.model.likeSmile = $('<div class="smile like" data-tooltip="Любо!"></div>');
				this.model.dislikeSmile = $('<div class="smile dislike" data-tooltip="Не любо("></div>');
				this.model.angrySmile = $('<div class="smile angry" data-tooltip="Злюсь"></div>');
				this.model.heartSmile = $('<div class="smile heart" data-tooltip="Люблю"></div>');

				this.model.simpleSmile.off('click');
				this.model.likeSmile.off('click');
				this.model.dislikeSmile.off('click');
				this.model.angrySmile.off('click');
				this.model.heartSmile.off('click');
				this.model.hideContainer.off('click');

				this.model.hideContainer.on('click', function () {
					this.model.container.stop().slideUp('slow');
					conf.isShow = false;
				}.bind(this));

				this.model.simpleSmile.on('click', function () {
					var appeningtext = ':smile(simple):';
					this.model.addNewComment.val(this.model.addNewComment.val() + ' ' + appeningtext);
				}.bind(this));

				this.model.likeSmile.on('click', function () {
					var appeningtext = ':smile(like):';
					this.model.addNewComment.val(this.model.addNewComment.val() + ' ' + appeningtext);
				}.bind(this));

				this.model.dislikeSmile.on('click', function () {
					var appeningtext = ':smile(dislike):';
					this.model.addNewComment.val(this.model.addNewComment.val() + ' ' + appeningtext);
				}.bind(this));

				this.model.heartSmile.on('click', function () {
					var appeningtext = ':smile(heart):';
					this.model.addNewComment.val(this.model.addNewComment.val() + ' ' + appeningtext);
				}.bind(this));

				this.model.angrySmile.on('click', function () {
					var appeningtext = ':smile(angry):';
					this.model.addNewComment.val(this.model.addNewComment.val() + ' ' + appeningtext);
				}.bind(this));

				this.model.commentContainer = $('<div class="comment_container"></div>');
				this.model.commentRowRender = function (senderName, date, message, id, userId) {
					var $currentContainer = $('<div item-id="' + id + '" class="comment_row"></div>');
					var displayDate = getDateString(date);
					var reg = new RegExp('\:smile\\(([a-zA-Z]*)\\)\\:', 'gmi');
					var result = reg.exec(message);
					while (result != null) {
						if (result.length > 0 && result[1] != undefined) {
							message = message.replace(result[0], '<div  class="smile ' + result[1] + '"></div>');
						}
						result = reg.exec(message);
					}

					var $row = $('<span><span><img class="comment_user_avatar" src="' + appPath.getAppPath() + '/User/avatar/' + (userId || conf.currentUser.userId) + '/size-18x18' + '"></img></span><strong>' + senderName + ' [' + displayDate + ']: </strong>' + message + '</span>');

					if (conf.currentUser.roleId == conf.admin) {
						var $delComment = $('<button class="comment_delete">X</button>');//.attr('item-id', id);
						$delComment.attr('item-id', id);
						$currentContainer.append($delComment);
						$delComment.on('click', function () {
							that.model.deleteComment($(this).attr('item-id'));
							delete1($(this).attr('item-id'));
						});
					}
					$currentContainer.append($row);
					return $currentContainer;
				}.bind(this);

				var that = this;
			}
			return this.model;
		}.bind(this);

		var delete1 = function (id) {
			var current = $('.comment_row[item-id=' + id + ']');
			current.slideUp(500, function () {
				current.remove();
			});
		};

		var getDateString = function (date) {
			var currentDate = new Date();
			var currentDateFromServer = new Date(date);
			var displayDate = '';
			var obj = {
				day: currentDateFromServer.getDate(),
				month: currentDateFromServer.getMonth() + 1,
				year: currentDateFromServer.getFullYear(),
				time: currentDateFromServer.toTimeString().split(' ')[0]
			};

			var dateSttring = [obj.day, obj.month, obj.year].join(".");
			dateSttring += " " + obj.time;
			if (currentDate.getFullYear() == obj.year && currentDate.getMonth() + 1 == obj.month && currentDate.getDate() == obj.day) {
				displayDate = 'сегодня в ';
				displayDate += obj.time;
			} else if (currentDate.getFullYear() == obj.year && currentDate.getMonth() == obj.month && currentDate.getDay() == obj.day - 1) {
				displayDate = 'вчера в ';
				displayDate += obj.time;
			} else {
				displayDate = dateSttring;
			}
			return displayDate;
		};

		return {
			delete: function (id) {
				delete1(id);
			},
			render: function (comments) {
				var model = getModel();
				if (conf.isShow) {
					model.container.show();
				} else {
					model.container.hide();
				}
				model.senderContainer.append(model.addNewComment);
				model.senderContainer.append(model.hideContainer);

				model.smileContainer.append(model.simpleSmile);
				model.smileContainer.append(model.likeSmile);
				model.smileContainer.append(model.dislikeSmile);
				model.smileContainer.append(model.angrySmile);
				model.smileContainer.append(model.heartSmile);
				model.smileContainer.append(model.sendNewComment);

				model.senderContainer.append(model.smileContainer);



				model.container.append(model.senderContainer);
				model.container.append(model.commentContainer);
				model.commentContainer.children().remove();
				var i = 0;
				var j = 0;
				var animate = function () {
					var container = $('.common_comment_container');
					_.each(comments, function (item) {
						i += 200;
						j++;
						var $row = model.commentRowRender(item.name, item.date, item.message, item.id, item.userId);
						$row.hide();
						if (model.commentContainer.children().length < 1) {
							model.commentContainer.append($row);
						} else {
							$(model.commentContainer.children()[0]).before($row);
						}
						if (j > 9) {
							i -= 200;
						}
						setTimeout(function () {
							$row.slideDown(200, function () {
								//container.scrollTop(container[0].scrollHeight, 'slow');
							});
						}, i + 100);

					});
				};
				animate();
				return model.container;
			},
			onDelete: function (func) {
				getModel().deleteComment = func;
			},
			onSendComment: function (func) {
				getModel().sendNewComment.off('click');
				getModel().sendNewComment.on('click', func);
			},
			addComment: function (item) {
				var model = getModel();

				var $row = model.commentRowRender(item.name, item.date, item.message, item.id, item.userId);
				$row.hide();

				if (model.commentContainer.children().length < 1) {
					model.commentContainer.append($row);
				} else {
					$(model.commentContainer.children()[0]).before($row);
				}
				$row.slideDown(200);
				if (model.commentContainer.children().length > 20) {
					$(model.commentContainer.children()).last().remove();
				}

			},
			show: function () {
				getModel().container.show();
			}
		};
	};
	return commentComponent;
});