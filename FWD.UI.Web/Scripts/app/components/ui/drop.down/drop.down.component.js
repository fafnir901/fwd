define([
	'jquery',
	'underscore'
], function($,_) {
	var dropDownComponent = function() {

		var getViewModel = function(items) {
			if (this.viewModel == undefined) {
				var vm = {};
				vm.components = {};
				vm.components.commonContainer = $('<div class="drop_down_common_container"></div>');
				vm.components.dropDownInput = $('<input class="drop_down_input" />');
				vm.components.dropDownButton = $('<div class="drop_down_button state_begin" role="button"></div>');
				vm.components.dropDownContainer = $('<div class="drop_down_container"></div>');
				vm.components.dropDownItemTemplate = '<div class="drop_down_item"></div>';
				vm.components.renderedDropDownItems = $('');

				this.viewModel = vm;
				if (items != undefined) {
					_.each(items, function(item) {
						//var dropDownItem = $(this.viewModel.components.dropDownItemTemplate);
						//dropDownItem.text(item.text);
						//dropDownItem.attr('title', item.text);
						//this.viewModel.components.renderedDropDownItems.push(dropDownItem[0]);
						addItem(item, this.viewModel);
					}.bind(this));
				}
			}
			return this.viewModel;
		}.bind(this);

		var addItem = function(item, viewModel) {
			var dropDownItem = $(viewModel.components.dropDownItemTemplate);
			dropDownItem.text(item.text || item);
			dropDownItem.attr('title', item.text || item);
			viewModel.components.renderedDropDownItems.push(dropDownItem[0]);
			return dropDownItem;
		};

		var combineComponents = function(viewModel) {
			viewModel.components.commonContainer.append(viewModel.components.dropDownInput);
			viewModel.components.commonContainer.append(viewModel.components.dropDownButton);

			_.each(viewModel.components.renderedDropDownItems, function(item) {
				viewModel.components.dropDownContainer.append(item);
			});
			viewModel.components.commonContainer.append(viewModel.components.dropDownContainer);

		};

		return {
			getViewModel: function(items) {
				return getViewModel(items);
			},
			addItem: function(item) {
				var model = getViewModel();
				var rendered = addItem(item, model);
				$('.drop_down_container').append(rendered);
			},
			render: function(items) {
				var model = getViewModel(items);
				combineComponents(model);
				return model.components.commonContainer;
			}
		};
	};

	return dropDownComponent;
});