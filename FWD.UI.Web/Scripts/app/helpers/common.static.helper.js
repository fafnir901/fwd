define([], function() {
	var commonHelper = (function () {
		var getOffsetSum = function (elem) {
			var top = 0, left = 0;
			while (elem) {
				top = top + parseInt(elem.offsetTop);
				left = left + parseInt(elem.offsetLeft);
				elem = elem.offsetParent;
			}

			return { top: top, left: left };
		};

		var getOffsetRect = function (elem) {
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
		return {
			getOffset: function (elem) {
				if (elem.getBoundingClientRect) {
					// "правильный" вариант
					return getOffsetRect(elem);
				} else {
					// пусть работает хоть как-то
					return getOffsetSum(elem);
				}
			},
			getRandomInt: function (min, max) {
				return Math.floor(Math.random() * (max - min + 1)) + min;
			}
		};
	})();

	return commonHelper;
});
