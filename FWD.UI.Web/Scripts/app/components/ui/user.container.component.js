define([
	'jquery',
	'underscore',
	'app/helpers/appPath.static.helper',
	'app/helpers/window.static.helper',
	'app/services/user.service',
	'app/models/user.model',
	'app/helpers/configurator.static.helper',
	'app/models/reminder.model',
	'app/components/hubs/reminder.hub'
], function ($, _, appPath, windowHelper, UserService, UserModel, conf, ReminderModel, reminderHub) {
	var userContainerComponent = function (configurator) {

		this.model = {
			userId: -1,
			userName: '',
			userRole: '',
		};

		this.view = {
			container: $('<div class="user_info"></div>'),
			actionContainer: $('<div class="user_action"></div>'),
			reminderContainer: $('<div class="user_reminder"></div>'),
			reminderButton: $('<button class="user_reminder_show" disabled/>'),
			userNameContainer: $('<span id="userEditor"></span>'),
			exitContainer: $('<a class="ui_link" href="' + appPath.getAppPath() + '/User/Logout' + '">Выход</a>'),
			avatarContainer: $('<div class="user_avatar"></div>'),
			avatarImg: $('<img src="' + appPath.getAppPath() + '/User/avatar/size-42x42' + '"/>')
		};
		var that = this;

		var getData = function () {
			return new UserService().getCurrentUserData().then(function (data) {
				that.model.userId = data.UserId;
				that.model.userName = data.UserName;
				that.model.userRole = data.UserRole;

				configurator.currentUser.name = data.UserName;
				configurator.currentUser.roleId = data.UserRole;
				configurator.currentUser.userId = data.UserId;
			});
		};

		var changeData = function () {
			that.view.userNameContainer.children().remove();
			that.view.userNameContainer.text(that.model.userName);
			that.view.actionContainer.append(that.view.userNameContainer);
			that.view.actionContainer.append(that.view.exitContainer);

			that.view.reminderContainer.append(that.view.reminderButton);

			that.view.avatarImg.attr('src', appPath.getAppPath() + '/User/avatar/size-42x42?' + Math.floor((Math.random() * 10) + 1));

			that.view.avatarContainer.append(that.view.avatarImg);

			that.view.container.append(that.view.reminderContainer);
			that.view.container.append(that.view.actionContainer);
			that.view.container.append(that.view.avatarContainer);
		};

		return {
			view: that.view,
			model: that.model,
			init: function () {
				var self = this;
				return getData().then(function () {
					changeData();

					var initialMarginLeft = -60;
					var initialWidth = 140;

					if (self.model.userName.length > 8) {
						initialMarginLeft -= (self.model.userName.length) + 10;
						initialWidth += (self.model.userName.length) + 10;
					}

					self.view.container.css('width', initialWidth + 'px');

					self.view.userNameContainer.off('click');
					self.view.reminderButton.off('click');
					self.view.userNameContainer.off('mouseenter');
					self.view.userNameContainer.off('mouseleave');

					self.view.userNameContainer.on('click', function () {
						new UserModel().render().then(function (res) {
							windowHelper.showPopUp(res, false);
						});
					});


					self.view.userNameContainer.on('mouseenter', function () {
						$(this).text('Редактировать');
						$(this).after('<div style="visibility:collapse;font-size:22px">Text</div>');

						self.view.reminderContainer.css('margin-left', (initialMarginLeft - 30) + 'px');
						self.view.container.css('padding-left', ((-initialMarginLeft) + 30) + 'px');

					});

					self.view.userNameContainer.on('mouseleave', function () {
						$(this).text(self.model.userName);
						$($(this).parent().find('div')).remove();

						self.view.reminderContainer.css('margin-left', '');
						self.view.container.css('padding-left', '');
					});

					self.view.avatarImg.off('click');

					self.view.avatarImg.on('click', function () {
						var img = $('<img src="' + appPath.getAppPath() + '/User/avatar/size-real' + '"/>');
						img.css('opacity', '0');
						$(document.body).append(img);
						setTimeout(function () {
							img.css('opacity', '1');
							img.remove();
							windowHelper.showImage(img, [img]);
						}, 100);

					});

					if (configurator.featureList.Reminder) {
						var reminder = new ReminderModel();
						var hub = reminderHub;
						hub.init(reminder);

						self.view.reminderButton.on('click', function () {
							var existing = $('.common').find('.reminder_container');
							if (existing.length == 0) {
								reminder.render().then(function (res) {
									$('.common').append(res);
								});

							} else {
								var current = $(existing);
								if (current.css('display') == 'none') {
									current.slideDown();
								} else {
									current.slideUp();
								}
							}

						});

					} else {
						self.view.reminderButton.attr('disabled', '');
					}
					return self;
				});
			},
			changeData: function () {
				return getData().then(function () {
					changeData();
				});
			}
		};
	};

	return userContainerComponent;
});