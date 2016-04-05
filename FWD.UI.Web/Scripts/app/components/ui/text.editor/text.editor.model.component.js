define([
	'jquery',
	'underscore',
	'app/components/ui/text.editor/text.editor.component'
], function($,_, TextEditorComponent) {

	var editorModelComponent = function() {

		var getModel = function() {
			if (this.model == undefined) {
				var m = {};
				m.uiComponent = new TextEditorComponent();
				m.font = {};
				m.font.fontSize = 25;
				m.font.color = 'black';
				m.font.backgroundColor = 'white';

				m.changes = {};
				m.changes.fontSizeChanges = [];
				m.changes.backGroundColorChanged = [];
				fontSizeChangeSetHandler(m);
				backgroundColorChangeSetHandler(m);
				//replaceAllHtmlSetHandler(m);
				showSource(m);
				this.model = m;
			}
			return this.model;
		}.bind(this);

		var createSelection = function(model, viewModel) {
			var range = window.getSelection().getRangeAt(0);
			if (range != undefined) {

				var parent = range.commonAncestorContainer.className == 'editor_body'
					? $(range.commonAncestorContainer)
					: $(range.commonAncestorContainer.parentElement);

				while (!parent.hasClass(viewModel.components.body.container.attr('class')) && parent.length > 0) {
					parent = $(parent.parent());
				}
				if (parent.length > 0) {
					var selectedText = {};
					selectedText.selectedText = '';
					selectedText.startOffset = 0;
					selectedText.endOffset = 0;
					if (range.collapsed) {
						selectedText.selectedText = range.startContainer.data.substring(range.startOffset) + range.endContainer.data.substring(0, range.endOffset);
						selectedText.startSelectedText = range.startContainer.data.substring(range.startOffset);
						selectedText.endSelectedText = range.endContainer.data.substring(0, range.endOffset);
						selectedText.startOffset = range.startOffset;
						selectedText.endOffset = range.endOffset;
						selectedText.parent = range.commonAncestorContainer.parentElement;
					} else {
						selectedText.selectedText = range.commonAncestorContainer.textContent.substring(range.startOffset, range.endOffset);
						selectedText.startOffset = range.startOffset;
						selectedText.endOffset = range.endOffset;
						selectedText.parent = range.commonAncestorContainer.parentElement;
					}

					return selectedText;
				}
				return null;
			}
			return null;
		};

		var restoreSelection = function(viewModel) {
			if (viewModel.components.body.currentSelection != null) {
				var current = window.getSelection();
				var newRange = document.createRange();
				newRange.setStart(viewModel.components.body.currentSelection.startContainer, viewModel.components.body.currentSelection.startOffset);
				newRange.setEnd(viewModel.components.body.currentSelection.endContainer, viewModel.components.body.currentSelection.endOffset);
				current.removeAllRanges();
				current.addRange(newRange);
			}
		};

		var changeFontSizeOnSelection = function(model, viewModel, selectedText) {
			var newSelectedText = '<span style="font-size:' + model.font.fontSize + 'px">' + selectedText.selectedText + '</span>';
			var res = _.find(model.changes.fontSizeChanges, function(item) {
				return item.selectedText == selectedText.selectedText;
			});
			if (res != undefined) {

				model.changes.fontSizeChanges = _.reject(model.changes.fontSizeChanges, function(item) {
					return item.selectedText == selectedText.selectedText;
				});
				model.changes.fontSizeChanges.push({
					selectedText: selectedText.selectedText,
					newSelectedText: newSelectedText,
					startSelectedText: selectedText.startSelectedText,
					endSelectedText: selectedText.endSelectedText,
					startOffset: selectedText.startOffset,
					endOffset: selectedText.endOffset,
					parent: $(selectedText.parent)
				});
			} else {
				model.changes.fontSizeChanges.push({
					selectedText: selectedText.selectedText,
					newSelectedText: newSelectedText,
					startSelectedText: selectedText.startSelectedText,
					endSelectedText: selectedText.endSelectedText,
					startOffset: selectedText.startOffset,
					endOffset: selectedText.endOffset,
					parent: $(selectedText.parent)
				});
			}

			commitAllChanges(model, viewModel);
		};

		var changeBackGroundColorOnSelection = function(model, viewModel, selectedText) {
			var newSelectedText = '<span style="background-color:' + model.font.backgroundColor + '">' + selectedText.selectedText + '</span>';
			var res = _.find(model.changes.backGroundColorChanged, function(item) {
				return item.selectedText == selectedText.selectedText;
			});
			if (res != undefined) {

				model.changes.backGroundColorChanged = _.reject(model.changes.backGroundColorChanged, function(item) {
					return item.selectedText == selectedText.selectedText;
				});
				model.changes.backGroundColorChanged.push({
					selectedText: selectedText.selectedText,
					newSelectedText: newSelectedText,
					startSelectedText: selectedText.startSelectedText,
					endSelectedText: selectedText.endSelectedText,
					startOffset: selectedText.startOffset,
					endOffset: selectedText.endOffset,
					parent: $(selectedText.parent)
				});
			} else {
				model.changes.backGroundColorChanged.push({
					selectedText: selectedText.selectedText,
					newSelectedText: newSelectedText,
					startSelectedText: selectedText.startSelectedText,
					endSelectedText: selectedText.endSelectedText,
					startOffset: selectedText.startOffset,
					endOffset: selectedText.endOffset,
					parent: $(selectedText.parent)
				});
			}
			commitAllChanges(model, viewModel);

		};

		var fontSizeChangeSetHandler = function(model) {
			var viewModel = model.uiComponent.getViewModel();
			viewModel.components.menu.fontSize.container.off('click');
			viewModel.components.menu.fontSize.container.on('click', function(e) {
				restoreSelection(viewModel);
				if (viewModel.components.menu.fontSize.dporDown.container.css('display') == 'none') {
					viewModel.components.menu.fontSize.dporDown.container.slideDown();

					viewModel.components.menu.backgroundColor.dporDown.container.slideUp();
				} else {
					viewModel.components.menu.fontSize.dporDown.container.slideUp();
				}
			});

			viewModel.components.body.container.on('blur', function(e) {
				if (window.getSelection().baseOffset != 0) {
					viewModel.components.body.currentSelection = window.getSelection().getRangeAt(0);
				}
			});

			viewModel.components.menu.fontSize.dporDown.items.off('click');
			viewModel.components.menu.fontSize.dporDown.items.on('click', function() {
				restoreSelection(viewModel);
				var that = $(this);
				model.font.fontSize = parseInt(that.text());
				viewModel.components.menu.fontSize.container.find('span').text(that.text() + 'px');
				var selection = createSelection(model, viewModel);
				if (selection != null) {
					changeFontSizeOnSelection(model, viewModel, selection);
				} else {
					viewModel.components.body.container.css('font-size', model.font.fontSize);
				}
			});
		};

		var backgroundColorChangeSetHandler = function(model) {
			var viewModel = model.uiComponent.getViewModel();
			viewModel.components.body.container.on('blur', function(e) {
				if (window.getSelection().baseOffset != 0) {
					viewModel.components.body.currentSelection = window.getSelection().getRangeAt(0);
				}
				viewModel.components.menu.fontSize.dporDown.container.slideUp();
			});

			viewModel.components.menu.backgroundColor.container.off('click');
			viewModel.components.menu.backgroundColor.container.on('click', function(e) {
				restoreSelection(viewModel);
				if (viewModel.components.menu.backgroundColor.dporDown.container.css('display') == 'none') {
					viewModel.components.menu.backgroundColor.dporDown.container.slideDown();

					viewModel.components.menu.fontSize.dporDown.container.slideUp();
				} else {
					viewModel.components.menu.backgroundColor.dporDown.container.slideUp();
				}
			});

			viewModel.components.menu.backgroundColor.dporDown.items.off('click');
			viewModel.components.menu.backgroundColor.dporDown.items.on('click', function() {
				restoreSelection(viewModel);
				var that = $(this);
				model.font.backgroundColor = $(that.children()[0]).css('background-color');
				viewModel.components.menu.backgroundColor.container.find('.indicator').css('background-color', model.font.backgroundColor);
				//viewModel.components.menu.fontSize.container.find('span').text(that.text() + 'px');
				var selection = createSelection(model, viewModel);
				if (selection != null) {
					changeBackGroundColorOnSelection(model, viewModel, selection);
				}
			});
		};

		var replaceAllHtmlSetHandler = function(model) {
			var viewModel = model.uiComponent.getViewModel();
			viewModel.components.body.container.on('blur', function(e) {
				if (window.getSelection().baseOffset != 0) {
					viewModel.components.body.currentSelection = window.getSelection().getRangeAt(0);
				}
			});

			viewModel.components.menu.replaceHtml.container.off('click');
			viewModel.components.menu.replaceHtml.container.on('click', function() {
				var currentText = viewModel.components.body.container.text();
				viewModel.components.body.container.html(currentText);
			});
		};

		var showSource = function(model) {
			var vm = model.uiComponent.getViewModel();
			vm.components.menu.showSource.container.off('click');
			vm.components.menu.showSource.container.on('click', function() {
				if (vm.components.menu.showSource.showSource) {
					var currentText = vm.components.body.container.html();
					vm.components.body.container.text(currentText);
					vm.components.menu.showSource.showSource = false;
					vm.components.menu.showSource.container.find('span').text('Текст');
				} else {
					vm.components.menu.showSource.showSource = true;
					var currentText1 = vm.components.body.container.text();
					vm.components.menu.showSource.container.find('span').text('Источник');
					vm.components.body.container.html(currentText1);
				}

			});
		};
		//без пересичений
		var commitAllChanges = function(model, viewModel) {
			var currentText = viewModel.components.body.container.html();

			_.each(model.changes.fontSizeChanges, function(change) {
				if (change.startSelectedText != undefined) {
					currentText = currentText.replace(change.startSelectedText, '<span style="font-size:' + model.font.fontSize + 'px">' + change.startSelectedText + '</span>');
					currentText = currentText.replace(change.endSelectedText, '<span style="font-size:' + model.font.fontSize + 'px">' + change.endSelectedText + '</span>');
				} else if (change.startOffset > 0) {

					var substr = change.parent.text().substring(change.startOffset, change.endOffset);
					var newText = change.parent.html().replace(substr, change.newSelectedText);
					change.parent.html(newText);

				} else {
					currentText = currentText.replace(change.selectedText, change.newSelectedText);
					viewModel.components.body.container.html(currentText);
				}
				model.changes.backGroundColorChanged.pop(change);
			});

			_.each(model.changes.backGroundColorChanged, function(change) {
				if (change.startSelectedText != undefined) {
					currentText = currentText.replace(change.startSelectedText, '<span style="background-color:' + model.font.backgroundColor + 'px">' + change.startSelectedText + '</span>');
					currentText = currentText.replace(change.endSelectedText, '<span style="background-color:' + model.font.backgroundColor + 'px">' + change.endSelectedText + '</span>');
					viewModel.components.body.container.html(currentText);
				} else if (change.startOffset > 0) {

					var substr = change.parent.text().substring(change.startOffset, change.endOffset);
					var newText = change.parent.html().replace(substr, change.newSelectedText);
					change.parent.html(newText);

				} else {
					currentText = currentText.replace(change.selectedText, change.newSelectedText);
					viewModel.components.body.container.html(currentText);
				}

				model.changes.backGroundColorChanged.pop(change);
			});

		};

		return {
			render: function() {
				return getModel().uiComponent.render();
			},
			getViewModel: function() {
				return getModel().uiComponent.getViewModel();
			}
		};

	};

	return editorModelComponent;
});