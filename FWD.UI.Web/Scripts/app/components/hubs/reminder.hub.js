define([
	'jquery',
	'app/helpers/notification.static.helper',
	'app/helpers/configurator.static.helper',
	'signalr.hubs'
], function ($, errorHelper, conf) {
	var reminderHub = (function () {
		return {
			init: function (reminderViewModel) {
				var reminder = $.connection.reminderHub;

				$.connection.hub.stop();

				reminder.off("notify");
				reminder.on("notify", function (message) {
					errorHelper.showReminder(message, 60);
				});


				$.connection.hub.start({ transport: ['webSockets', 'longPolling'] }).done(function () {
					reminder.server.join($.connection.hub.id, conf.currentUser.userId);

					reminderViewModel.onStore(function (userId, message) {
						return reminder.server.store(userId, message);
					});

					reminderViewModel.onGetAll(function (userId) {
						return reminder.server.getAll(userId);
					});

					reminderViewModel.onRun(function (userId, taskId, interval) {
						reminder.server.run(userId, taskId, interval);
					});

					reminderViewModel.onExit(function (taskId) {
						reminder.server.exit(taskId);
					});

					$('.user_reminder_show').removeAttr('disabled');
				});
			}
		};
	})();

	return reminderHub;
});