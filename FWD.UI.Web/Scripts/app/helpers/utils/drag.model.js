define([

	],
function() {
	var dragModel = function () {

		this.dragModel = {};
		this.dragModel.draggableElement = {};
		this.dragModel.dropabbleElement = {};
		this.dragModel.draggableElement.initialPosition = {
			x: 0,
			y: 0,
			dislplay: 'block'
		};
		this.dragModel.draggableElement.shiftX = 0;
		this.dragModel.draggableElement.shiftY = 0;
		this.dragModel.draggableElement.currentParent = {};
		this.dragModel.draggableElement.rollback = function () {
			var $currentDraggable = $(getModel().draggableElement.current);
			var $parent = $(getModel().draggableElement.currentParent);
			$currentDraggable.css('position', 'relative');
			$currentDraggable.css('display', getModel().draggableElement.initialPosition.display);
			$parent.append($currentDraggable);
		};
		this.dragModel.callBackAfterEnd = {};
		this.dragModel.callBackAfterCancel = {};

		var getModel = function () {
			return this.dragModel;
		}.bind(this);

		var preInit = function ($draggable, event) {
			var model = getModel();
			model.draggableElement.current = $draggable;
			model.draggableElement.currentParent = $draggable.parent();
			model.draggableElement.initialPosition.x = event.pageX;
			model.draggableElement.initialPosition.y = event.pageY;
			model.draggableElement.initialPosition.display = $draggable.css('display');


			//$(document.body).append(model.draggableElement.current);
		};

		var findDraggable = function (event) {
			var elem = event.target;
			while (elem != document && elem.getAttribute('draggable') == null) {
				elem = elem.parentNode;
			}
			return elem == document ? null : elem;
		};

		var onMouseDown = function (event) {
			event = fixEvent(event);
			if (event.which != 1) {
				return;
			}
			var element = findDraggable(event);
			preInit($(element), event);
			return false;
		};

		var onMouseMove = function (event) {
			var model = getModel();
			if (!model.draggableElement.current) return; // элемент не зажат

			event = fixEvent(event);

			//if (!model.draggableElement) { // если перенос не начат...
			//	var moveX = event.pageX - model.draggableElement.initialPosition.x;
			//	var moveY = event.pageY - model.draggableElement.initialPosition.y;

			//	// если мышь передвинулась в нажатом состоянии недостаточно далеко
			//	if (Math.abs(moveX) < 3 && Math.abs(moveY) < 3) {
			//		return;
			//	}

			//	// начинаем перенос
			//	//dragObject.avatar = createAvatar(e); // создать аватар

			//	$(document.body).append(model.draggableElement.current);
			//	model.draggableElement.current.css('z-index', '9999');
			//	model.draggableElement.current.css('position', 'absolute');

			//	if (!model.draggableElement.current) { // отмена переноса, нельзя "захватить" за эту часть элемента
			//		model.draggableElement = {};
			//		return;
			//	}

			//	// аватар создан успешно
			//	// создать вспомогательные свойства shiftX/shiftY
			//	var coords = getCoords(model.draggableElement.current);
			//	model.draggableElement.shiftX = model.draggableElement.initialPosition.x - coords.left;
			//	model.draggableElement.shiftY = model.draggableElement.initialPosition.y - coords.top;

			//	startDrag(event); // отобразить начало переноса
			//}

			// отобразить перенос объекта при каждом движении мыши
			var moveX = event.pageX - model.draggableElement.initialPosition.x;
			var moveY = event.pageY - model.draggableElement.initialPosition.y;
			if (Math.abs(moveX) < 3 && Math.abs(moveY) < 3) {
				return;
			} else {
				model.draggableElement.current.css('position', 'absolute');
			}
			//var coords = getCoords(model.draggableElement.current);
			//model.draggableElement.shiftX = model.draggableElement.initialPosition.x - coords.left;
			//model.draggableElement.shiftY = model.draggableElement.initialPosition.y - coords.top;
			var currentLeft = event.pageX - model.draggableElement.shiftX;
			var currentTop = event.pageY - model.draggableElement.shiftY;
			model.draggableElement.current.css('left', currentLeft);
			model.draggableElement.current.css('top', currentTop);

			return false;
		};

		var onMouseUp = function (event) {
			if (getModel().draggableElement.current.length > 0) { // если перенос идет
				event = fixEvent(event);
				finishDrag(event);
			}

			// перенос либо не начинался, либо завершился
			// в любом случае очистим "состояние переноса" dragObject
			getModel().draggableElement.current = null;
		};

		var finishDrag = function (event) {
			var $dropElem = findDroppable(event);

			if (!$dropElem) {
				onDragCancel(getModel().draggableElement.current);
			} else {
				onDragEnd(getModel().draggableElement.current, $dropElem);
			}
		};

		var onDragCancel = function ($element) {
			getModel().draggableElement.rollback();
			getModel().callBackAfterCancel();
			getModel().draggableElement.current = null;
		};
		var onDragEnd = function ($element) {
			getModel().draggableElement.current.css('display', 'none');
			getModel().callBackAfterEnd();
		};
		var findDroppable = function (event) {

			var elem = getElementUnderClientXY(getModel().draggableElement.current, event.clientX, event.clientY);
			while (elem != document && elem.getAttribute('dropabble') == null) {
				elem = elem.parentNode;
			}

			return elem == document ? null : $(elem);
		};

		var startDrag = function (event) {
			var avatar = getModel().draggableElement.current;
			// инициировать начало переноса
			$(document.body).append(avatar);
			avatar.css('z-index', '9999');
			avatar.css('position', 'absolute');
		};

		var fixEvent = function (e) {
			e = e || window.event;

			if (!e.target) e.target = e.srcElement;

			if (e.pageX == null && e.clientX != null) { // если нет pageX..
				var html = document.documentElement;
				var body = document.body;

				e.pageX = e.clientX + (html.scrollLeft || body && body.scrollLeft || 0);
				e.pageX -= html.clientLeft || 0;

				e.pageY = e.clientY + (html.scrollTop || body && body.scrollTop || 0);
				e.pageY -= html.clientTop || 0;
			}

			if (!e.which && e.button) {
				e.which = e.button & 1 ? 1 : (e.button & 2 ? 3 : (e.button & 4 ? 2 : 0))
			}

			return e;
		};

		var getCoords = function (elem) {
			var box = elem.getBoundingClientRect();

			var body = document.body;
			var docElem = document.documentElement;

			var scrollTop = window.pageYOffset || docElem.scrollTop || body.scrollTop;
			var scrollLeft = window.pageXOffset || docElem.scrollLeft || body.scrollLeft;

			var clientTop = docElem.clientTop || body.clientTop || 0;
			var clientLeft = docElem.clientLeft || body.clientLeft || 0;

			var top = box.top + scrollTop - clientTop;
			var left = box.left + scrollLeft - clientLeft;

			return { top: Math.round(top), left: Math.round(left) };
		};

		var getElementUnderClientXY = function ($elem, clientX, clientY) {
			var display = $elem.css('display') || '';
			$elem.css('display', 'none');
			var target = document.elementFromPoint(clientX, clientY);
			$elem.css('display', display);
			return target;
		};

		return {
			init: function (callBackAfterEnd, callBackAfterCancel) {
				getModel().callBackAfterEnd = callBackAfterEnd;
				getModel().callBackAfterCancel = callBackAfterCancel;
				$(document).off('mousedown');
				$(document).off('mousemove');
				$(document).off('mouseup');
				$(document).on('mousedown', function (e) {
					onMouseDown(e);
				});
				$(document).on('mousemove', function (e) {
					onMouseMove(e);
				});
				$(document).on('mouseup', function (e) {
					onMouseUp(e);
				});
			}
		};
	};
	return dragModel;
});