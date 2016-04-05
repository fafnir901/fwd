define([
	'jquery',
	'underscore',
	'app/components/ui/drop.down/drop.down.component'
], function ($, _, DropDownComponent) {
	var dropDownComponentModel = function () {

		var getModel = function (items) {
			if (this.model == undefined) {
				var m = {};
				m.dropDownUi = new DropDownComponent();
				if (items != undefined) {
					m.items = items;
				}
				setHandlerOnItem(m);
				setHandlerOnButton(m);

				m.selectedItem = {};
				m.selectedText = '';

				this.model = m;
			}
			return this.model;
		}.bind(this);

		var setHandlerOnButton = function (model) {
			var viewModel = model.dropDownUi.getViewModel(model.items);
			viewModel.components.dropDownButton.off('click');
			viewModel.components.dropDownButton.on('click', function () {
				if (viewModel.components.dropDownContainer.css('display') == 'none') {
					viewModel.components.dropDownContainer.css('display', 'block');
					viewModel.components.dropDownContainer.animate({
						'height': '200px'
					});
					$(this).removeClass('state_begin');
					$(this).addClass('state_down');
				} else {

					viewModel.components.dropDownContainer.animate({
						'height': '0px'
					}, function () {
						viewModel.components.dropDownContainer.css('display', 'none');
					});

					$(this).addClass('state_begin');
					$(this).removeClass('state_down');
				}
			});
		};

		var setHandlerOnItem = function (model) {
			var viewModel = model.dropDownUi.getViewModel(model.items);
			viewModel.components.renderedDropDownItems.off('click');
			viewModel.components.renderedDropDownItems.on('click', function () {
				var that = $(this);
				model.selectedText = that.text();
				model.selectedItem = that;
				viewModel.components.dropDownInput.val(model.selectedText);

				viewModel.components.dropDownContainer.animate({
					'height': '0px'
				}, function () {
					viewModel.components.dropDownContainer.css('display', 'none');

					viewModel.components.dropDownButton.addClass('state_begin');
					viewModel.components.dropDownButton.removeClass('state_down');
				});
			});
		};

		return {
			render: function () {
				return getModel().dropDownUi.render();
			},
			addAndRenderItem: function (text) {
				var model = getModel();
				model.dropDownUi.addItem(text);
				model.items.push({
					text: text,
					default: false
				});
				setHandlerOnItem(model);
				setHandlerOnButton(model);
			},
			getModel: function () {
				return getModel();
			},
			getCurrentValue: function () {
				var model = getModel().dropDownUi.getViewModel(getModel().items);
				var res = model.components.dropDownInput.val();
				return res;
			},
			setCurrentValue: function (itemName) {
				var model = getModel().dropDownUi.getViewModel(getModel().items);
				var match = _.find(model.components.renderedDropDownItems, function (item) {
					return $(item).text() == itemName;
				});
				if (match != undefined) {
					model.components.dropDownInput.val($(match).text());
				}
			},
			addItems: function (items) {

				var current = _.find(items, function (item) {
					return item.default;
				});
				getModel(items);

				if (current != null) {
					getModel().selectedText = current.text;
					getModel().dropDownUi.getViewModel().components.dropDownInput.val(current.text);
				}

			}
		};
	};

	return dropDownComponentModel;
});