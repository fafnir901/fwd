define([
	'jquery',
	'underscore'
], function ($, _) {
	var sorterComponent = function () {

		var model = {
			isExpanded: false,
			sortOrder: 'asc',
			articleViewMode: 'default',
			propertyList: {
				"Название": "Name",
				"Рейтинг": "Rate",
				"Дата создания": "CreationDate",
				"Количество картинок": "CountOfImages",
			},
			view: {
				parent: $('.left_panel_container'),
				container: $('.article_sorter_container'),
				expand: $('.article_sorter_expander'),
				order: $('<button class="article_sorter_order asc">&#8593</button>'),
				apply: $('<button class="article_sorter_apply">OK</button>'),
				propList: $('<select class="article_sorter_prop_list"></select>'),
				behaviourContainer: $('<div class="article_sorter_behaviour_container"></div>')
			},
			itemList: [],
			rerender: {}
		};

		var initPropList = function () {
			_.each(model.propertyList, function (item, key) {
				var option = $('<option></option>');
				option.text(key);
				option.attr('sortBy', item);
				model.view.propList.append(option);
			});
		};

		var handleOrderClick = function () {
			model.view.order.off('click');
			model.view.order.on('click', function () {

				if (model.sortOrder == 'asc') {
					model.view.order.css({ 'transform': 'rotate(180deg)' });
				} else {
					model.view.order.css({ 'transform': 'rotate(0deg)' });
				}


				model.sortOrder = model.sortOrder == "asc" ? "desc" : "asc";
			});
		};
		var handleApplyClick = function () {
			model.view.apply.off('click');
			model.view.apply.on('click', function () {
				sort();
			});
		};

		var getSelectedProperty = function() {
			var selectedIndex = model.view.propList[0].selectedIndex;
			var option = $(model.view.propList[0][selectedIndex]);
			var prop = option.attr('sortBy');
			return prop;
		};

		var sortDefault = function() {
			var prop = getSelectedProperty();

			var sorted = _.sortBy(model.itemList, prop);
			sorted = model.sortOrder == 'asc' ? sorted : sorted.reverse();
			model.rerender(sorted);
		};
		var sortTree = function() {
			var prop = getSelectedProperty();
			_.each(model.itemList, function(item) {
				var sorted = _.sortBy(item.Articles, prop);
				sorted = model.sortOrder == 'asc' ? sorted : sorted.reverse();
				item.Articles = sorted;
			});
			model.rerender(model.itemList);
		};

		var sort = function () {
			if (model.articleViewMode == "default") {
				sortDefault();
			} else {
				sortTree();
			}
		};

		var combineBehaviourContainer = function () {
			model.view.behaviourContainer.append(model.view.propList);
			model.view.behaviourContainer.append(model.view.order);
			model.view.behaviourContainer.append(model.view.apply);
			handleOrderClick();
			handleApplyClick();
			model.view.container.append(model.view.behaviourContainer);
		};

		var expand = function () {
			model.view.parent.css({ 'height': 'calc(100% - 90px)' });
			model.view.container.stop().animate({ 'margin-top': '0px' });
			model.isExpanded = true;
			combineBehaviourContainer();
		};

		var collapse = function () {
			model.view.parent.css({ 'height': 'calc(100% - 51px)' });
			model.view.container.stop().animate({ 'margin-top': '-34px' });
			model.isExpanded = false;
			model.view.behaviourContainer.children().remove();
			model.view.behaviourContainer.remove();
		};

		var combineComponent = function () {
			model.view.expand.off('click');
			model.view.expand.on('click', function () {
				if (!model.isExpanded) {
					expand();
				} else {
					collapse();
				}
			});
		};

		return {
			init: function () {
				initPropList();
				combineComponent();
				
			},
			setBehavior: function (items, renderFunc) {
				model.itemList = items;
				model.rerender = renderFunc;
			},
			container: model.view.container,
			model: model,
			setViewMode: function (mode) {
				model.articleViewMode = mode;
			},
		};

	};
	return sorterComponent;
});