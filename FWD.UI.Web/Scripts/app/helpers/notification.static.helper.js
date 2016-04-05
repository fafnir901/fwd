define([
	"jquery",
	"underscore"
],
function ($, _) {
	var notificationHelper = (function () {
		var preInit = function () {
			this.current = {};
			this.current.rootElement = $('.common');
			this.current.errorBoxClass = 'error_box';
			this.current.infromationBoxClass = 'info_box';
			this.current.reminderBoxClass = 'reminder_box';
			this.current.element = $('<div></div>');
			this.current.element.addClass(this.current.errorBoxClass);
			this.current.element.on('click', function () {
				$(this).slideUp();
			});
			this.current.rootElement.append(this.current.element);
			return this.current;
		};

		var getPreInitValue = function () {
			if (this.current == undefined) {
				this.current = preInit();
			}
			return this.current;
		};

		var show = function (message, isHtml, interval) {
			isHtml = isHtml || false;
			interval = interval || 30000;
			var model = getPreInitValue();
			if (isHtml) {
				model.element.html(message);
			} else {
				model.element.text(message);
			}
			model.element.slideDown(function () {
				model.element.addClass('animated shake');
				model.element.one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
					model.element.removeClass('animated shake');
				});
			});
			setTimeout(function () {
				model.element.slideUp();
			}, interval);
		};

		var hide = function () {
			getPreInitValue().element.slideUp();
		};

		return {
			init: function (rootElement) {
				preInit();
				rootElement.append(getPreInitValue().element);
			},
			showError: function (text) {
				var inputText = '';
				try {
					var renderedText = $($(text).children()[1]).text();
					inputText = renderedText;
				} catch (err) {
					inputText = text;
				} finally {
					if (inputText == "") {
						inputText = text;
					}
					var model = getPreInitValue();
					model.element.removeClass(model.infromationBoxClass);
					model.element.removeClass(model.reminderBoxClass);
					model.element.addClass(model.errorBoxClass);
					show(inputText);
				}
			},
			showInfo: function (text, isHtml) {
				var model = getPreInitValue();
				model.element.removeClass(model.errorBoxClass);
				model.element.removeClass(model.reminderBoxClass);
				model.element.addClass(model.infromationBoxClass);
				show(text, isHtml);
			},
			showReminder: function (text, intervalInSeconds) {
				var model = getPreInitValue();
				intervalInSeconds = intervalInSeconds * 1000;
				model.element.removeClass(model.errorBoxClass);
				model.element.removeClass(model.infromationBoxClass);
				model.element.addClass(model.reminderBoxClass);
				var newElem = $('<div class="reminder_view"></div>');
				var img = $('<div class="reminder_img"></div>');
				var txt = $('<div class="reminder_view_text"></div>');
				newElem.append(img);
				newElem.append(txt);
				txt.text(text);

				show(newElem, true, intervalInSeconds);
			},
			hide: function () {
				hide();
			},
		};
	})();

	return notificationHelper;
});