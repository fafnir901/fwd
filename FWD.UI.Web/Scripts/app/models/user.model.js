define([
	'jquery',
	'underscore',
	'app/services/user.service',
	'app/helpers/configurator.static.helper',
	'app/helpers/notification.static.helper',
	'app/helpers/appPath.static.helper',
	'app/components/ui/file.uploader.component'
], function ($, _, UserService, conf, errorHelper, appPath, FileUploader) {
	var userModel = function () {
		var service = new UserService;
		var model;

		var getUserData = function (model) {
			var user = { FirstName: {}, LastName: {}, Email: {} };
			user.FirstName = model.viewModel.content.userName.find('input').val();
			user.LastName = model.viewModel.content.lastName.find('input').val();
			user.Email = model.viewModel.content.email.find('input').val();
			return user;
		};

		var equalUsers = function (oldUser, currentUser) {
			return oldUser.FirstName == currentUser.FirstName
				&& oldUser.LastName == currentUser.LastName
				&& oldUser.Email == currentUser.Email;
		};

		var updateUserData = function (user) {
			return service.updateUserData(user, model.viewModel.content.userDataContainer);
		};

		var initViewModel = function () {
			model = {};
			model.prevUser = {};
			model.viewModel = {};
			model.viewModel.content = {};
			model.viewModel.content.container = $('<div class="user_editor"></div>');
			model.viewModel.content.userDataContainer = $('<div class="user_editor_user_data"></div>');
			model.viewModel.content.userCredentialsContainer = $('<div class="user_editor_credentials"></div>');
			model.viewModel.content.userAvatarContainer = $('<div class="user_editor_avatar"></div>');
			model._viewData = {}; //service.getUserEditor($('.navigate_panel'));

			model.viewModel.content.userName = $('<div><div><span class="field_name">Имя:</span></div><input type="text" id="userName" class="user_editor_input" disabled/></div>');
			model.viewModel.content.lastName = $('<div><div><span class="field_name">Фамилия:</span></div><input type="text" id="lastName" class="user_editor_input" disabled/></div>');
			model.viewModel.content.email = $('<div><div><span class="field_name">Email:</span></div><input type="text" id="email" class="user_editor_input" disabled/></div>');
			model.viewModel.content.editUserContainer = $('<div></div>');
			model.viewModel.content.editUserContainer.edit = $('<button id="edit_user_data" style="margin-bottom:10px">Редактировать</button>');
			model.viewModel.content.editUserContainer.save = $('<button id="save_user_data" class="hidden" style="margin-bottom:10px">Сохранить</button>');

			model.viewModel.content.editUserContainer.save.on('click', function (evt) {
				model.viewModel.content.userDataContainer.removeClass('error_element');
				var childs = model.viewModel.content.userDataContainer.find('input');
				var user = getUserData(model);
				if (equalUsers(model.prevUser, user)) {
					errorHelper.showError("Вы не изменили данные пользователя");
					model.viewModel.content.userDataContainer.addClass('error_element');
				} else {
					updateUserData(user)
						.success(function (res) {
							_.each(childs, function (item) {
								$(item).attr('disabled', '');
							});
							model.viewModel.content.editUserContainer.save.addClass('hidden');
							model.viewModel.content.editUserContainer.edit.removeClass('hidden');
							errorHelper.showInfo("Данные о пользователе успешно обновлены");
							$('#userEditor').text(user.FirstName);
							conf.components.userContainer.changeData();
						})
						.error(function (res) {
							errorHelper.showError(res.responseText);
						});
				}
			});

			model.viewModel.content.editUserContainer.edit.on('click', function (evt) {
				var res = model.viewModel.content.userDataContainer.find('[disabled]');
				if (res.length != 0) {

					model.prevUser = getUserData(model);
					_.each(res, function (item) {
						$(item).removeAttr('disabled');
					});
					model.viewModel.content.editUserContainer.save.removeClass('hidden');
					model.viewModel.content.editUserContainer.edit.addClass('hidden');
				}
			});
			model.viewModel.content.editUserContainer.append(model.viewModel.content.editUserContainer.edit);
			model.viewModel.content.editUserContainer.append(model.viewModel.content.editUserContainer.save);

			model.viewModel.content.avatar = $('<div><div><span class="field_name">Аватар:</span></div><div class="user_editor_avatar_container"><img id="avatar"></div>');
			model.viewModel.content.saveAvatar = $('<div><button id="save_avatar" style="margin-bottom:10px">Сохранить</Button></div>');
			model.viewModel.content.saveAvatar.on('click', function () {
				service.saveAvatar($(model.viewModel.content.container)).success(function () {
					errorHelper.showInfo('Аватар заменен');
					conf.components.userContainer.changeData();
				}).fail(function (ex) {
					errorHelper.showError(ex.responseText);
				});
			});


			model.viewModel.content.login = $('<div><div><span class="field_name">Логин:</span></div><input type="text" id="login" class="user_editor_input" disabled/></div>');
			model.viewModel.content.password = $('<div><div><span class="field_name">Пароль:</span></div><input disabled type="password" id="password" class="user_editor_input"/></div>');

			//model.viewModel.content.userName.text(model._viewData.User.FirstName);


			model.viewModel.content.userDataContainer.append(model.viewModel.content.userName);
			model.viewModel.content.userDataContainer.append(model.viewModel.content.lastName);
			model.viewModel.content.userDataContainer.append(model.viewModel.content.email);
			model.viewModel.content.userDataContainer.append(model.viewModel.content.editUserContainer);

			model.viewModel.content.userCredentialsContainer.append(model.viewModel.content.login);
			model.viewModel.content.userCredentialsContainer.append(model.viewModel.content.password);

			model.viewModel.content.avatar.find('#avatar').attr('src', appPath.getAppPath() + '/user/avatar/size-300x400');

			model.viewModel.content.userAvatarContainer.append(model.viewModel.content.avatar);


			var fileUploader = new FileUploader();
			model.viewModel.content.userAvatarContainer.append(fileUploader.getUploader($(model.viewModel.content.avatar.find('.user_editor_avatar_container')), true));
			model.viewModel.content.userAvatarContainer.append(model.viewModel.content.saveAvatar);

			model.viewModel.content.container.append(model.viewModel.content.userDataContainer);
			model.viewModel.content.container.append(model.viewModel.content.userCredentialsContainer);
			model.viewModel.content.container.append(model.viewModel.content.userAvatarContainer);
			return model;
		};

		return {
			render: function () {
				var currentModel = initViewModel();
				return service.getUserEditor($('.navigate_panel')).then(function (result) {
					model._viewData = result;
					model.viewModel.content.userName.find('#userName').val(model._viewData.User.FirstName);
					model.viewModel.content.lastName.find('#lastName').val(model._viewData.User.LastName);
					model.viewModel.content.email.find('#email').val(model._viewData.User.Email);
					model.viewModel.content.login.find('#login').val(model._viewData.User.Login);
					model.viewModel.content.password.find('#password').val(model._viewData.User.Password);
					return model.viewModel.content.container;
				});
			}
		};
	};

	return userModel;
});