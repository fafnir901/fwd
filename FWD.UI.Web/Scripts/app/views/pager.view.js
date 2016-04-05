define([
	'jquery',
	'underscore',
	'app/helpers/configurator.static.helper'
], function ($, _, conf) {
	var pagerView = function () {

		this.viewModel = {};
		var getViewModel = function () {
			return this.viewModel;
		}.bind(this);

		var preInitContent = function (model) {
			var displayIndex = model.currentPage.index + 1;

			var $container = $('<div class="' + model.idsOfControls[0] + '"></div>');
			var $firstButton = $('<button id="' + model.idsOfControls[4] + '">First</button>');
			var $nextButton = $('<button id="' + model.idsOfControls[1] + '">Next</button>');
			var $prevButton = $('<button id="' + model.idsOfControls[2] + '">Previous</button>');
			var $lastButton = $('<button id="' + model.idsOfControls[5] + '">Last</button>');
			var allPAges = model.allPages == undefined ? displayIndex + 1 : model.allPages;

			var $currentPage = $('<span id="' + model.idsOfControls[3] + '">' + displayIndex + ' of ' + allPAges + '</span>');
			$container.append($firstButton);
			$container.append($prevButton);
			$container.append($currentPage);
			$container.append($nextButton);
			$container.append($lastButton);

			model.content = $container;
		};
		var preInit = function (entityModel) {
			var model = getViewModel();
			model.entityModel = entityModel;
			model.idsOfControls = ["pager_container", "next_button", "prev_button", "current_page", "first_page", "last_page"];
			model.pager = _.clone(conf.pager);
			model.currentPage = { index: 0 };
			model.allPages = entityModel.totalCount || undefined;
			model.content = $();
			preInitContent(model);
			return model;
		};
		return {
			init: function (entityModel) {
				var model = preInit(entityModel);
				return model;
			}
		};

	};

	return pagerView;
});