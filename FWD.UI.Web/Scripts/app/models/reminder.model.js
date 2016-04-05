define([
	'jquery',
	'underscore',
	'app/helpers/notification.static.helper',
	'app/helpers/configurator.static.helper'
], function ($, _, errorHelper, conf) {
	var reminder = function () {

		var model = {
			list: [],
			run: {},
			exit: {},
			getAll: {},
			store: {},
			view: {
				container: $('<div class="reminder_container"></div>'),
				addContainer: $('<div class="reminder_add_container"></div>'),
				reminderMessageContainer: $('<div class="reminder_message_container"></div>'),
				reminderTimeContainer: $('<div class="reminder_time_container"></div>'),
				reminderMessage: $('<input type="text" class="reminder_message" placeholder="Введите напоминание..."/>'),
				reminderAddMessage: $('<button class="reminder_add">Добавить</button>'),
				reminderTimeTypeSelect: $('<select class="remider_time_type"/>'),
				reminderTimeSelect: $('<select class="remider_time"/>'),
				reminderRowContainer: $('<div class="reminder_row_container"></div>')
			}
		};

		var add = function (userId, taskId, message, isRunning) {
			model.list.push({ userId: userId, taskId: taskId, message: message, isRunning: isRunning });
		};

		var validate = function (text) {
			model.view.reminderMessage.removeClass(conf.errorClass);
			if (text == undefined || text.length == 0) {
				errorHelper.showError('Напоминание не может быть пустым');
				model.view.reminderMessage.addClass(conf.errorClass);
				return false;
			}
			return true;
		};

		var renderRow = function (item) {
			var $row = $('<div class="reminder_row"></div>');
			var $run = $('<button class="reminder_run" />');
			var $stop = $('<button class="reminder_stop" />');
			var text = $('<div class="reminder_message_text not_runned"></div>');
			$row.attr('user_id', item.userId);
			$row.attr('task_id', item.taskId);
			$row.attr('interval', item.interval || 10);
			text.text(item.message);
			$row.append(text);
			if (!item.isRunning) {
				$row.append($run);
			} else {
				$row.append($stop);
				text.removeClass('not_runned');
				text.addClass('runned');
			}


			$run.off('click');
			$run.on('click', function () {
				model.run(item.userId, item.taskId, item.interval || 10);
				$run.remove();
				$row.append($stop);
				text.removeClass('not_runned');
				text.addClass('runned');
			});

			$stop.off('click');
			$stop.on('click', function () {
				model.exit(item.taskId);
				$row.slideUp(function () {
					$row.remove();
				});

			});
			return $row;
		};

		var initTimeType = function () {
			model.view.reminderTimeTypeSelect.children().remove();
			model.view.reminderTimeTypeSelect.append($('<option>s</option>'));
			model.view.reminderTimeTypeSelect.append($('<option>m</option>'));
			model.view.reminderTimeTypeSelect.append($('<option>h</option>'));
		};

		var initTime = function (timeType) {
			model.view.reminderTimeSelect.children().remove();
			switch (timeType) {
				case "s":
					model.view.reminderTimeSelect.append($('<option>1</option>'));
					for (var i = 2; i < 60; i++) {
						model.view.reminderTimeSelect.append($('<option>' + i + '</option>'));
					}
					break;
				case "m":
					model.view.reminderTimeSelect.append($('<option>1</option>'));
					for (var i = 2; i < 60; i++) {
						model.view.reminderTimeSelect.append($('<option>' + i + '</option>'));
					}
					break;
				case "h":
					model.view.reminderTimeSelect.append($('<option>1</option>'));
					for (var i = 2; i < 25; i++) {
						model.view.reminderTimeSelect.append($('<option>' + i + '</option>'));
					}
					break;
				default:
			}
		};

		var getSeconds = function (time, type) {
			switch (type) {
				case "s":
					return time;
				case "m":
					return time * 60;
				case "h":
					return time * 60 * 60;
				default:
					return time;
			}
		};

		var renderContainer = function () {
			model.view.container.children().remove();
			model.view.reminderRowContainer.children().remove();
			model.list.splice(0, model.list.length);

			initTimeType();
			initTime('s');
			model.view.reminderTimeTypeSelect.off('change');
			model.view.reminderTimeTypeSelect.on('change', function () {
				initTime($(this).val());
			});

			model.view.reminderMessageContainer.append(model.view.reminderMessage);
			model.view.reminderMessageContainer.append(model.view.reminderAddMessage);

			model.view.reminderTimeContainer.append(model.view.reminderTimeSelect);
			model.view.reminderTimeContainer.append(model.view.reminderTimeTypeSelect);

			model.view.addContainer.append(model.view.reminderMessageContainer);
			model.view.addContainer.append(model.view.reminderTimeContainer);

			model.view.reminderAddMessage.off('click');
			model.view.reminderAddMessage.on('click', function () {
				var message = model.view.reminderMessage.val();
				var isValid = validate(message);
				if (isValid) {
					var time = model.view.reminderTimeSelect.val();
					var timeType = model.view.reminderTimeTypeSelect.val();


					model.view.reminderMessage.val('');
					model.store(conf.currentUser.userId, message).then(function (taskId) {
						add(conf.currentUser.userId, taskId, message, false);
						var row = renderRow({ userId: conf.currentUser.userId, taskId: taskId, message: message, isRunning: false, interval: getSeconds(time, timeType) });

						row.hide();

						if (model.view.reminderRowContainer.children().length < 1) {
							model.view.reminderRowContainer.append(row);
						} else {
							$(model.view.reminderRowContainer.children()[0]).before(row);
						}
						row.slideDown();
						if (model.view.reminderRowContainer.children().length > 10) {
							$(model.view.reminderRowContainer.children()).last().remove();
						}
					});
				}
			});

			return model.getAll(conf.currentUser.userId).then(function (data) {
				_.each(data, function (item) {
					add(item.UserId, item.TaskId, item.Message, item.IsRunning);
				});
				_.each(model.list, function (item) {
					var row = renderRow(item);
					model.view.reminderRowContainer.append(row);
				});
				model.view.container.append(model.view.addContainer);
				model.view.container.append(model.view.reminderRowContainer);
				return model.view.container;
			});

		};

		return {
			init: function () {

			},
			onStore: function (func) {
				model.store = func;
			},
			onRun: function (func) {
				model.run = func;
			},
			onExit: function (func) {
				model.exit = func;
			},
			onGetAll: function (func) {
				model.getAll = func;
			},
			render: function () {
				return renderContainer();
			},
		};


	};
	return reminder;
});