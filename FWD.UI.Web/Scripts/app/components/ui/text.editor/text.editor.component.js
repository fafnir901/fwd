define([
	'jquery'
], function($) {
	var textEditorComponent = function() {
		var getViewModel = function() {
			if (this.viewModel == undefined) {
				var vM = {};
				vM.components = {};
				vM.components.commonContainer = $('<div class="editor_common_container"></div>');
				vM.components.menu = {};
				vM.components.menu.container = $('<div class="editor_menu"></div>');

				vM.components.menu.fontSize = {};
				vM.components.menu.fontSize.container = $('<div class="editor_menu_fontSize_container" role="button" hidefocus="hidefocus"><span>13px</span></div>');
				vM.components.menu.fontSize.dporDown = {};
				vM.components.menu.fontSize.dporDown.container = $('<div class="editor_menu_fontSize_dropDown_container"></div>');
				vM.components.menu.fontSize.dporDown.itemSelector = '.editor_menu_fontSize_dropDown_item';
				vM.components.menu.fontSize.dporDown.items = $('');
				prepareFontSizeDropDownItems(vM);

				vM.components.menu.backgroundColor = {};
				vM.components.menu.backgroundColor.container = $('<div class="editor_menu_background_container" role="button" hidefocus="hidefocus"><div class="editor_menu_background_dropDown_item_marker indicator" style="background-color:white;"></div></div>');
				vM.components.menu.backgroundColor.dporDown = {};
				vM.components.menu.backgroundColor.dporDown.container = $('<div class="editor_menu_background_dropDown_container" role="button" hidefocus="hidefocus"></div>');
				vM.components.menu.backgroundColor.dporDown.itemSelector = '.editor_menu_background_dropDown_item';
				vM.components.menu.backgroundColor.dporDown.items = $('');
				prepareBackgroundDropDownItems(vM);

				vM.components.menu.replaceHtml = {};
				vM.components.menu.replaceHtml.container = $('<div class="editor_menu_replceHtml_container" role="button"><span style="line-height: 2.5em;text-decoration: line-through;">HTML</span></div>');

				vM.components.menu.showSource = {
					showSource: true
				};
				vM.components.menu.showSource.container = $('<div class="editor_menu_showSource_container" role="button"><span>Источник</span></div>');

				vM.components.body = {};
				vM.components.body.currentSelection = null;
				//Однажды возвращает меня память в тот страшный летний день, когда идя по речке безымянной наткнулся я на труп ужасной женщины
				vM.components.body.container = $('<div class="editor_body" role="textbox" contenteditable="true"></div>');
				this.viewModel = vM;
			}
			return this.viewModel;
		}.bind(this);

		var prepareFontSizeDropDownItems = function(viewModel) {
			for (var i = 8; i < 49; i++) {
				if (i % 2 == 0) {
					var item = $('<div class="editor_menu_fontSize_dropDown_item">' + i + '</div>');
					viewModel.components.menu.fontSize.dporDown.items.push(item[0]);
				}
			}
		};

		var prepareBackgroundDropDownItems = function(viewModel) {
			var colorList = [];
			colorList.push('white');
			colorList.push('black');
			colorList.push('green');
			colorList.push('blue');
			colorList.push('yellow');
			colorList.push('red');
			colorList.push('gray');
			colorList.push('cyan');
			colorList.push('aliceblue');
			colorList.push('chocolate');
			colorList.push('gold');

			_.each(colorList, function(item) {
				var innerElement = $('<div class="editor_menu_background_dropDown_item_marker"></div>');
				var outerElement = $('<div class="editor_menu_background_dropDown_item"></div>');

				innerElement.css('background-color', item);
				outerElement.append(innerElement);

				viewModel.components.menu.backgroundColor.dporDown.items.push(outerElement[0]);
			});
		};

		var combineFontSizeContainer = function(viewModel) {

			_.each(viewModel.components.menu.fontSize.dporDown.items, function(item) {
				viewModel.components.menu.fontSize.dporDown.container.append(item);
			});
			viewModel.components.menu.fontSize.container.append(viewModel.components.menu.fontSize.dporDown.container);
		};

		var combineBackGroundContainer = function(viewModel) {
			_.each(viewModel.components.menu.backgroundColor.dporDown.items, function(item) {
				viewModel.components.menu.backgroundColor.dporDown.container.append(item);
			});
			viewModel.components.menu.backgroundColor.container.append(viewModel.components.menu.backgroundColor.dporDown.container);
		};

		var combineMenu = function(viewModel) {
			//

			//viewModel.components.menu.container.append(viewModel.components.menu.replaceHtml.container);
			viewModel.components.menu.container.append(viewModel.components.menu.showSource.container);
			viewModel.components.menu.container.append(viewModel.components.menu.backgroundColor.container);
			viewModel.components.menu.container.append(viewModel.components.menu.fontSize.container);
		};

		var combineComponents = function(viewModel) {

			combineFontSizeContainer(viewModel);
			combineBackGroundContainer(viewModel);
			combineMenu(viewModel);

			viewModel.components.commonContainer.append(viewModel.components.menu.container);
			viewModel.components.commonContainer.append(viewModel.components.body.container);
		};

		return {
			getViewModel: function() {
				return getViewModel();
			},
			render: function() {
				var model = getViewModel();
				combineComponents(model);
				return model.components.commonContainer;
			}
		};
	};

	return textEditorComponent;
});