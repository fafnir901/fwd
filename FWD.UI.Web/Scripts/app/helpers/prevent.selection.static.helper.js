﻿define([
], function () {
	var preventSelectionHelper = (function () {
		var preventSelection = false;

		var addHandler = function (element, event, handler) {
			if (element.attachEvent)
				element.attachEvent('on' + event, handler);
			else
				if (element.addEventListener)
					element.addEventListener(event, handler, false);
		};

		var removeSelection = function() {
			if (window.getSelection) { window.getSelection().removeAllRanges(); }
			else if (document.selection && document.selection.clear)
				document.selection.clear();
		};

		var killCtrlA = function(event) {
			var event = event || window.event;
			var sender = event.target || event.srcElement;

			if (sender.tagName.match(/INPUT|TEXTAREA/i) || sender.className == 'editor_body')
				return;

			var key = event.keyCode || event.which;
			if (event.ctrlKey && key == 'A'.charCodeAt(0))  // 'A'.charCodeAt(0) можно заменить на 65
			{
				removeSelection();

				if (event.preventDefault)
					event.preventDefault();
				else
					event.returnValue = false;
			}
		};

		return {
			preventSelection: function (element) {
			
				// не даем выделять текст мышкой
				addHandler(element, 'mousemove', function () {
					if (preventSelection)
						removeSelection();
				});

				addHandler(element, 'mousedown', function (event) {
					var event = event || window.event;
					var sender = event.target || event.srcElement;
					var parent = sender.parentElement;
					while (parent != null && parent.className != 'editor_body') {
						parent = parent.parentElement;
					}
					preventSelection = !sender.tagName.match(/INPUT|TEXTAREA/i) && sender.className != 'editor_body' && (sender.parentElement.className != 'editor_body');
				});

				// борем dblclick
				// если вешать функцию не на событие dblclick, можно избежать
				// временное выделение текста в некоторых браузерах
				addHandler(element, 'mouseup', function () {
					if (preventSelection)
						removeSelection();
					preventSelection = false;
				});

				// борем ctrl+A
				// скорей всего это и не надо, к тому же есть подозрение
				// что в случае все же такой необходимости функцию нужно 
				// вешать один раз и на document, а не на элемент
				addHandler(element, 'keydown', killCtrlA);
				addHandler(element, 'keyup', killCtrlA);
			}
		};
	})();
	return preventSelectionHelper;
});