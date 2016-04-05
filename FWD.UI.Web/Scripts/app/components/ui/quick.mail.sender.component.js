define([
	'jquery',
	'app/services/article.service',
	'app/helpers/notification.static.helper'
], function($,ArticleService, errorHelper) {
	var quickMailSender = function(articleId, rootElement) {


		var viewModel = {
			component: {
				container: {},
				mailToText: {},
				process: {}
			}
		};

		var validate = function(mail) {
			var str = '';

			if (mail.length == 0) {
				str += 'Отсутствует адрес получателя. ';
			} else if (mail.length < 5) {
				str += 'Адрес получателя должен превышать 5 символов. ';
			}

			var re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
			if (!re.test(mail)) {
				str += 'Что-то это не похоже на адрес O_o.';
			}
			return str;
		};

		var render = function(element) {
			var left = element.offset().left;

			viewModel.component.container = $('<div class="quick_mail_sender" style="display:none"></div>');
			viewModel.component.mailToText = $('<input type="text" placeholder="Адрес" class="quick_mail_sender_mail_to"/>');
			viewModel.component.subject = $('<input type="text" placeholder="Тема" class="quick_mail_sender_subject"/>');
			viewModel.component.process = $('<button id="quick_mail_submit">Послать</button>');

			viewModel.component.container.css('left', (left - 133 / 2 - 10) + 'px');


			viewModel.component.container.append(viewModel.component.mailToText);
			viewModel.component.container.append(viewModel.component.subject);
			viewModel.component.container.append(viewModel.component.process);

			viewModel.component.process.off('click');
			viewModel.component.process.on('click', function() {
				var str = validate(viewModel.component.mailToText.val());
				if (str.length == 0) {
					viewModel.component.container.remove();
					new ArticleService().sendThisEmail(articleId, viewModel.component.mailToText.val(), viewModel.component.subject.val(), rootElement)
						.fail(function(data) {
							errorHelper.showError(data.responseText);
						})
						.success(function(data) {
							errorHelper.showInfo("Email успешно отправлен по адресу " + data);
						});
				} else {
					errorHelper.showError(str);
				}
			});

			var timeout = {};

			viewModel.component.container.off('mouseleave');
			viewModel.component.container.on('mouseleave', function() {
				timeout = setTimeout(function() {
					viewModel.component.container.slideUp(function() {
						viewModel.component.container.remove();
					});
				}, 3000);
			});

			viewModel.component.container.off('click');
			viewModel.component.container.on('click', function(event) {
				if ($(event.target).hasClass('quick_mail_sender'))
					viewModel.component.container.slideUp(function() {
						viewModel.component.container.remove();
					});
			});

			viewModel.component.container.off('mouseenter');
			viewModel.component.container.on('mouseenter', function() {
				clearTimeout(timeout);
			});

			return viewModel.component.container;
		};
		return {
			render: function(element) {
				return render(element);
			}
		};
	};
	return quickMailSender;
});