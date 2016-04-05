define([
	'jquery',
	'app/helpers/configurator.static.helper'
], function ($, conf) {
	var toolTip = function (options) {
		var $element = options.element;
		var offSet = options.offset == undefined ? 10 : options.offset;
		var text = options.text;
		var container = options.container;
		var takeHtmlFromElelement = options.takeHtmlFromElelement || false;
		var takeHtmlAsIs = options.takeHtmlAsIs || false;
		var additionalContainer = options.additionalContainer || null;
		var toolTipElement;

		var onMouseEnter = function (e) {
			var that = $(this);
			if (takeHtmlAsIs && additionalContainer != null) {
				that = additionalContainer(that);
			}
			show(e.pageX, e.pageY, that);

			if (conf.matchMedia) {
				setTimeout(function () {
					hide();
				}, 1000);
			}
		};

		var onMouseLeave = function (e) {
			hide();
		};

		var onMouseMove = function (e) {
			var that = $(this);
			if (takeHtmlAsIs && additionalContainer != null) {
				that = additionalContainer(that);
			}
			show(e.pageX, e.pageY, that);
		};


		$element.off('mouseenter');
		$element.off('mouseleave');
		$element.off('mousemove');

		$element.on({
			'mouseenter': onMouseEnter,
			'mouseleave': onMouseLeave,
			'mousemove': onMouseMove
		});


		var getTextInContainer = function (txt) {
			var currentText;
			if (container != undefined) {
				var currentContainer = $(container);
				currentContainer.text(txt);
				currentText = currentContainer[0].outerHTML;
			} else {
				currentText = txt;
			}
			return currentText;
		};

		var getHtmlFromContainer = function ($el) {
			var html;
			if (container != undefined) {
				var currentContainer = $(container);
				currentContainer.append($el);
				html = currentContainer[0].outerHTML;
			} else {
				html = $el[0].outerHTML;
			}
			return html;
		};

		var getTooltipElem = function ($el) {
			if (!toolTipElement) {
				toolTipElement = $('<div/>', {
					'class': 'tooltip',
					html: getTextInContainer(text)
				});
			}
			if ($el != undefined) {
				if (!takeHtmlAsIs) {
					toolTipElement.html(takeHtmlFromElelement
						? getTextInContainer($el.text() || $el.val())
						: getTextInContainer(text));
				} else {
					toolTipElement.html(getHtmlFromContainer($el));
				}
			}
			return toolTipElement;
		};

		var hide = function () {
			getTooltipElem().remove();
		};

		var show = function (pageX, pageY, $el) {
			var tooltipElem = getTooltipElem($el);

			if (!tooltipElem.is(':visible')) {
				// первым делом - отобразить подсказку, чтобы можно было получить её размеры
				tooltipElem.appendTo('body');
			}

			var scrollY = $(window).scrollTop();
			var winBottom = scrollY + $(window).height();

			var scrollX = $(window).scrollLeft();
			var winRight = scrollX + $(window).width();

			var newLeft = pageX + offSet;
			var newTop = pageY + offSet;

			if (newLeft + tooltipElem.outerWidth() > winRight) { // если за правой границей окна
				newLeft -= tooltipElem.outerWidth();
				newLeft -= offSet + 2; // немного левее, чтобы курсор был не над подсказкой
			}

			if (newTop + tooltipElem.outerHeight() > winBottom) { // если за нижней границей окна
				newTop -= tooltipElem.outerHeight();
				newTop -= offSet + 2; // немного выше
			}

			tooltipElem.css({
				left: newLeft,
				top: newTop
			});
		};
	};

	return toolTip;
});