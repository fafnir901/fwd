define([
	"jquery",
	"underscore",
	"app/helpers/common.static.helper",
	"app/helpers/notification.static.helper",
	"app/helpers/window.static.helper",
	"app/helpers/utils/pager.model",
	"app/helpers/utils/drag.model",
	"app/helpers/appPath.static.helper"
], function ($, _, commonHelper,
	notificationHelper, windowHelper, pager, dragModel, appPath) {
	var configurator = (function () {
		var $class = 'article_in_load';
		var $loader = $('<div class="article_loader"><span style="display:inline-block; vertical-align:middle">Загрузка</span></div>');
		var timeoutSet = 0;
		var beforeSend = function (currentRootElement, additionalRules) {
			var $doc = $(document);
			$doc.off('ajaxStart');
			$doc.off('ajaxStop');
			$doc.ajaxStart(function () {

				timeoutSet = setTimeout(function () {
					configurator.loader.hide();
					var current = $(currentRootElement);
					if (additionalRules != undefined) {

						var offset = commonHelper.getOffset(current[0]);
						configurator.loader.width(current.width() + additionalRules.additionalWidth);
						configurator.loader.height(current.height() + additionalRules.additionalHeight);
						configurator.loader.css('left', offset.left - additionalRules.additionalMinusLeft);
						configurator.loader.css('top', offset.top);
					}
					current.append(configurator.loader);
					configurator.loader.show(100);
				}, 500);

			});

			$doc.ajaxStop(function () {
				clearTimeout(timeoutSet);

				configurator.loader.css('width', '100%');
				configurator.loader.css('height', '100%');
				configurator.loader.css('left', '');
				configurator.loader.css('top', '');

				configurator.loader.hide(100);
				configurator.loader.remove();
			});
		};
		var attentionOnElement = function ($el, property, value) {
			var def = $.Deferred();
			var obj = {};
			obj[property] = value;
			$el.animate(obj, 100, def.resolve);
			return def.promise();
		};
		var attentionOnElementChain = function ($el, property, value) {
			var def = new $.Deferred();
			var inittial = $el.css(property);
			$el.addClass('attention');
			var transition = $el.css('transition');
			$el.css('transition', 'none');
			$el.addClass('animated rubberBand');
			$el.on('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
				$el.removeClass('animated rubberBand');
				$el.removeClass('attention');
				$el.css('transition', transition);
				def.resolve();
			});
			return def.promise();
		};

		var selectItem = function (articleId) {
			var $element = $('.left_panel-item[item-id=' + articleId + ']');
			var parent = $element.parent();
			var lftModel = configurator.models.leftPanelModel;
			if (lftModel.getModel().currentView == "default") {
				var count = 0;
				var childrens = parent.children();
				for (var i = 0; i < childrens.length; i++) {
					if ($(childrens[i]).attr('item-id') != $element.attr('item-id')) {
						count++;
					} else {
						break;
					}
				}
				$(parent).scrollTop(count * $element.height());
				$element.addClass('left_panel-item-selection');
				$element.attr("selected", "selected");
			} else {
				$element.addClass('left_panel-item-selection');
				$element.attr("selected", "selected");
				lftModel.animateForGroup(parent);
			}
			return $element;
		};

		return {
			featureList: {},
			appPath: appPath,
			uploadParams: {
				url: appPath.getAppPath() + '/uploadImage',
				onSuccess: function () {
					var def = $.Deferred();
					def.resolve(notificationHelper.showInfo('Изображение успешно загружено'));
					return def.promise();
				},
				onError: function (text) {
					notificationHelper.showError(text);
				},
				onProgress: function (loaded, total, container) {
					var progressIndicator = $('<div></div>');
				}
			},
			commonHelper: commonHelper,
			beforeSend: beforeSend,
			errorClass: 'error_element',
			classLoader: $class,
			admin: "Admin",
			loader: $loader,
			pager: pager,
			currentSource: 'db',
			pagerScope: {
				skip: 15,
				take: 15
			},
			isShow: false,
			globalRootElement: $('.main_content'),
			selectItem: selectItem,
			currentUser: {
				name: '',
				roleId: '',
				avatar: '',
				userId: ''
			},
			initialArticleViewSettings: {
				fontSize: 13,
				splitterPosition: '51%',
				artilceWidth: '50%',
				imageContainerWidth: '38%',
				imageContainerLeft: '52%',
			},
			attentionOnElement: function ($element) {
				switch ($element.css("position")) {
					default:
					case "relative":
						return attentionOnElementChain($element, "margin-left", "20");
					case "absolute":
						return attentionOnElementChain($element, "left", "20");
				}
			},
			matchMedia: window.matchMedia('handheld').matches,
			models: {
				//userModel: {},
				leftPanelModel: {},
				planModel: {},
				tranModel: {},
				topMenuModel: {},
				searchModel: {},
				statModel: {}
			},
			components: {
				userContainer: {},
			},
		};
	})();
	return configurator;
});