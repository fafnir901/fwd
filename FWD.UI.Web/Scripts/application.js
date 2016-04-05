define("application", [
	"jquery",
	"underscore",
	"app/helpers/configurator.static.helper",
	'app/helpers/notification.static.helper',
	'app/helpers/appPath.static.helper',
	'app/models/article.model',
	'app/models/left.panel.model',
	'app/models/top.menu.model',
	'app/components/ui/user.container.component'
], function ($, _, configurator, errorHelper, appPath, ArticleModel, LeftPanelModel, TopMenuModel, UserContainerComponent) {
	var application = (function () {
		var self;
		var model = function () {
			self = this;
		};
		var initFunc = function () {
			var rootElement = configurator.globalRootElement;
			configurator.components.userContainer = new UserContainerComponent(configurator);

			var articleModel = new ArticleModel();
			var leftPanel = new LeftPanelModel();
			var topMenuModel = new TopMenuModel();

			leftPanel.init(articleModel, $('.main_content'), false);
			configurator.components.userContainer.init().then(function (md) {
				topMenuModel.init(leftPanel, articleModel, $('.left_panel_container'), rootElement, md);
			});

			configurator.models.articleModel = articleModel;
			configurator.models.leftPanelModel = leftPanel;
			configurator.models.topMenuModel = topMenuModel;

			$(document).ajaxError(function (e, request, errorThrown, exception) {
				if (request.status == "308") {
					window.location = request.getResponseHeader('location');
				}
			});

			if (window.showArticle != undefined) {
				var id = window.showArticle.articleId;
				configurator.models.articleModel.showArticle(id, rootElement).then(function () {
					configurator.selectItem(id, rootElement);
				});
				window.showArticle = undefined;
			};

			if (window.showError != undefined) {
				errorHelper.showError(window.showError.ex);
				window.showError = undefined;
			}
		};

		model.prototype = {
			run: function () {
				$.ajax({
					url: appPath.getAppPath() + '/features/featureList',
					beforeSend: configurator.beforeSend(configurator.globalRootElement),
					dataType: 'json',
					type: 'GET'
				}).success(function (list) {
					configurator.featureList = list;
					initFunc();
				})
				.fail(function (ex) {
					errorHelper.showError(ex.responseText);
					initFunc();
				});
			}
		};
		return model;
	})();
	return application;
});