define([
	"jquery",
	"underscore",
	"app/helpers/appPath.static.helper",
	"app/helpers/notification.static.helper"
],
function ($, _, appPath, errorHelper) {
	var windowHelper = (function () {
		var preInit = function () {
			this.popUpWindow = {};
			this.popUpWindow.width = '100%';
			this.popUpWindow.height = '100%';
			this.popUpWindow.currentClass = 'pop_up_window';
			this.popUpWindow.root = $('<div></div>');
			this.popUpWindow.hider = $('<div class="hider"></div>');
			this.popUpWindow.root.addClass(this.popUpWindow.currentClass);
			this.popUpWindow.parent = $('.common');
			this.popUpWindow.hideButton = $('<button class="hide_window"></button>');
			$(this.popUpWindow.hideButton).on('click', function () {
				$.ajax({
					url: appPath.getAppPath() + '/clearImage',
					type: 'POST'
				});
				_hidePopUpInner(this.popUpWindow);
			}.bind(this));
			this.popUpWindow.root.append(this.popUpWindow.hideButton);
			this.popUpWindow.content = {};
			this.popUpWindow.parent.append(this.popUpWindow.hider);
			this.popUpWindow.hider.append(this.popUpWindow.root);
			return this.popUpWindow;
		};

		var preInitImage = function () {
			var imageWindow = {};
			imageWindow.width = '';
			imageWindow.height = '';
			imageWindow.currentClass = 'image_window';
			imageWindow.root = $('<div></div>');
			imageWindow.hider = $('<div class="hider"></div>');
			imageWindow.root.addClass(imageWindow.currentClass);
			imageWindow.parent = $('.common');
			imageWindow.hideButton = $('<button class="hide_window"></button>');
			imageWindow.nextImage = $('<div class="next_img"></div>');
			imageWindow.prevImage = $('<div class="prev_img"></div>');
			imageWindow.root.append(imageWindow.hideButton);
			imageWindow.root.append(imageWindow.nextImage);
			imageWindow.root.append(imageWindow.prevImage);
			imageWindow.content = {};
			imageWindow.parent.append(imageWindow.hider);
			imageWindow.hider.append(imageWindow.root);
			imageWindow.name = $('<div class="image_name"></div>');
			$(imageWindow.hideButton).on('click', function () {
				_hideImage(_getImageWindow());
			});
			return imageWindow;
		};

		var declareCenterOfImage = function(width, height) {
			var x = window.innerWidth / 2;
			var y = window.innerHeight / 2;

			return { offsetX: x - width / 2, offsetY: y - height / 2 };
		};

		var appendImageToRoot = function ($content, model) {
			$(document.body).append($content);
			var width = $content.width() == 0 ? $content[0].naturalWidth : $content.width();
			var height = $content.height() == 0 ? $content[0].naturalHeight : $content.height();
			$content.remove();

			var left = 0;
			var top = 0;

			var currentLeft = model.root.position().left == 0 ? left : model.root.position().left;
			var currentTop = model.root.position().top == 0 ? top : model.root.position().top;

			var currentWidth = ($(document).width() - currentLeft - 120) < width ? $(document).width() - currentLeft - 120 * 2 : width + 10;
			var currentHeight = ($(document).height() - currentTop) < height ? $(document).height() - currentTop - 120 : height + 10;

			$content.css({ 'width': currentWidth - 10, 'height': currentHeight - 10 });

			var offset = declareCenterOfImage(currentWidth, currentHeight);

			model.root.css({ 'width': currentWidth, 'height': currentHeight, 'padding-left': '10px', 'padding-top': '10px' });
			model.root.css({ 'left': offset.offsetX + 'px', 'top': offset.offsetY + 'px' });
			var prevModelName = model.root.find(model.name);
			if (prevModelName != undefined) {
				$(prevModelName).remove();
			}

			model.root.find('img[image_name]').remove();
			setTimeout(function () {
				model.root.find('img[image_name]').remove();
				$content.css('visibility', 'hidden');
				model.root.append($content);
				model.name.text($content.attr("image_name"));
				model.root.append(model.name);
				$content.css('visibility', 'visible');
			}, 500);


			model.content = $content;
			return model;
		};

		var _initImage = function (model, $firstImage) {
			this.iterator = 0;
			model.nextImage.off('click');
			model.prevImage.off('click');
			$(model.imgList).each(function (index, value) {
				if (value.attr('image_id') == $firstImage.attr('image_id')) {
					this.iterator = index;
				}
			}.bind(this));

			model = appendImageToRoot(model.imgList[this.iterator], model);
			if (model.imgList != undefined && model.imgList.length > 1) {
				model.nextImage.removeAttr("disabled");
				model.prevImage.removeAttr("disabled");

				model.nextImage.on('click', function () {
					//model.root.find('img[image_name]').remove();
					//model.imgList[this.iterator].remove();
					this.iterator++;
					if (model.imgList[this.iterator] != undefined) {
						model = appendImageToRoot(model.imgList[this.iterator], model);
					} else {
						this.iterator = 0;
						model = appendImageToRoot(model.imgList[this.iterator], model);
					}
				}.bind(this));
				model.prevImage.on('click', function () {
					//model.root.find('img[image_name]').remove();
					//model.imgList[this.iterator].remove();
					this.iterator--;
					if (model.imgList[this.iterator] != undefined) {
						model = appendImageToRoot(model.imgList[this.iterator], model);
					} else {
						this.iterator = model.imgList.length - 1;
						model = appendImageToRoot(model.imgList[this.iterator], model);
					}

				}.bind(this));
			}
			else {
				model.nextImage.attr("disabled", "disabled");
				model.prevImage.attr("disabled", "disabled");
			}
			return model;
		}.bind(this);

		var _hideImage = function (model) {
			if (model.content != undefined) {
				model.content.remove();
				model.name.remove();
				$(model.root).stop().toggle(400);
				setTimeout(function () {
					$(model.hider).stop().hide();
				}, 500);
			}
		};

		var _getPopUp = function () {
			if (this.popUpWindow == undefined) {
				this.popUpWindow = preInit();
			}
			return this.popUpWindow;
		};

		var _getImageWindow = function () {
			if (this.imageWindow == undefined) {
				this.imageWindow = preInitImage();
			}
			return this.imageWindow;
		};

		var _hidePopUpInner = function (model) {
			if (model.content != undefined) {
				model.content.remove();
			}
			$(model.root).stop().animate({ 'left': '100%' }, 500);
			setTimeout(function () {
				$(model.hider).stop().hide();
			}, 500);
		};

		var _appendContent = function ($content) {
			_getPopUp().content = $content;
			_getPopUp().root.append($content);
		}.bind(this);

		var _createNewInstance = function ($content) {
			var newModel = {};
			newModel.hider = $('<div class="hider"></div>');
			newModel.content = $content;
			newModel.root = $('<div></div>');
			newModel.hideButton = $('<button class="hide_window"></button>');
			$(newModel.hideButton).on('click', function () {
				_hidePopUpInner(newModel);
			});
			newModel.root.addClass(_getPopUp().currentClass);
			newModel.root.append(newModel.content);
			newModel.root.append(newModel.hideButton);
			newModel.hider.append(newModel.root);
			return newModel;
		};

		return {
			showPopUp: function ($content, secondTime, callback) {
				secondTime = secondTime || false;
				if (secondTime) {
					var newModel = _createNewInstance($content);
					_getPopUp().hider.append(newModel.hider);

					newModel.hider.show();
					newModel.root.stop().animate({ 'left': '10%' }, 500, function () {
						if (callback != undefined)
							callback();
					});
				} else {
					_appendContent($content);
					_getPopUp().hider.show();
					_getPopUp().root.stop().animate({ 'left': '10%' }, 500, function () {
						if (callback != undefined)
							callback();
					});

				}
			},
			hidePopUp: function () {
				_hidePopUpInner(_getPopUp());
				$(document).off('mousedown');
				$(document).off('mousemove');
				$(document).off('mouseup');
			},
			showImage: function ($content, imaglst) {
				var model = _getImageWindow();
				model.imgList = imaglst;
				_initImage(model, $content);
				model.hider.stop().show();
				model.root.stop().show(500);

				//imaglst[it][0].onload = function () {
				//	var wind = $('.image_window');
				//	$('.image_window').css('left', '');
				//	$('.image_window').css('width', '');
				//	$('.image_window').css('height', '');
				//	wind.width(this.width + 20);
				//	wind.height(this.height + 20);

				//	wind.css('position', 'relative');
				//	wind.css('margin', '0 auto');
				//	var offset = wind.offset();

				//	wind.css('position', 'absolute');
				//	wind.css('left', offset.left-20);
				//};
				//$content.on('load', function () {

				//});
			},
			hideImage: function () {
				_hideImage(_getImageWindow());
			}
		};
	})();
	return windowHelper;
});