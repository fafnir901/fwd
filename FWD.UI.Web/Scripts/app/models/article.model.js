define([
	'jquery',
	'underscore',
	'app/services/article.service',
	'app/components/hubs/chat.hub',
	'app/helpers/configurator.static.helper',
	'app/helpers/appPath.static.helper',
	'app/components/ui/comment.component',
	'app/helpers/window.static.helper',
	'app/helpers/notification.static.helper',
	'app/components/ui/rate.component',
	'app/components/ui/tag.container.component'
], function ($, _, ArticleService, chatHub, conf, appPath, CommentComponent, windowHelper, errorHelper, Rate, TagContainer) {
	var articleModel = function () {
		var service = new ArticleService(conf.beforeSend);
		var that = this;
		that._viewData = {};
		var init = function (item) {

			that._viewData.articleName = item.ArticleName;
			that._viewData.articleId = item.ArticleId;
			that._viewData.description = item.Content;
			that._viewData.link = item.Link;
			that._viewData.images = item.Images;
			that._viewData.rate = item.Rate;
			that._viewData.tags = item.Tags;
		};

		var fontSizeDown = function () {
			var articleDescr = $('.article_description');
			var fontSize = Number(articleDescr.css('font-size').replace('px', ''));
			fontSize--;
			if (fontSize < 12) {
				fontSize = 12;
			}
			articleDescr.stop().animate({ 'font-size': fontSize }, 0);
			$('.font_size_indicator').text(fontSize + 'px');
		};

		var fontSizeUp = function () {
			var articleDescr = $('.article_description');

			var fontSize = Number(articleDescr.css('font-size').replace('px', ''));
			fontSize++;
			if (fontSize > 48) {
				fontSize = 48;
			}
			articleDescr.stop().animate({ 'font-size': fontSize }, 0);
			conf.initialArticleViewSettings.fontSize = fontSize;
			$('.font_size_indicator').text(fontSize + 'px');
		};

		var replaceAllScripts = function (text) {
			if (text != null) {
				text = text.replace(/<script/gmi, '&lt;script').replace(/<\/script>/gmi, '&lt;/script&gt;');
				text = text.replace(/<link/gmi, '&lt;script').replace(/<\/link>/gmi, '&lt;/link&gt;').replace(/link>/gmi, 'link&gt;');
				//text = text.replace(/<.*[\/>]$/g, '&lt;.*[\/&gt]$');
				text = text.replace(/[<]button|[<]input|[<]link.*[\/>]$/g, '[&lt;]button|[&lt;]input.*[\/&gt;]$');
			}
			return text;
		};

		var replaceImagesSrc = function (text, imagesFromServer, articleId) {
			try {
				var images = $(text).find('img');
				if (images != undefined && images.length > 0) {
					for (var i = 0; i < images.length; i++) {
						var image = $(images[i]);
						if (imagesFromServer[i] != undefined) {
							var src = appPath.getAppPath() + '/images/articleID-' + articleId + '/imageID-' + imagesFromServer[i].ImageId;
							text = text.replace(image.attr('src'), src);
						}
					}
				}
				return text;
			} catch (error) {
				return text;
			}
		};

		var checkForHtml = function (text) {
			try {
				var result = $(text);
				if (result == undefined) {
					return false;
				}
				return true;

			} catch (error) {
				return false;
			}
		};

		var render = function (parent) {
			var $a = $('<div class="link_item"><span style="font-weight:bold">Ссылка: </span><a href="' + that._viewData.link + '" target="_blank">' + that._viewData.link + '</a></div>');
			$a.off('click');
			$a.on('click', function (e) {
				var that = $(this);
				var current = $(that.find('a'));
				window.open(current.attr('href'), '_blank');
			});
			var $name = $('<span>' + that._viewData.articleName + '</span>');
			$name.addClass('article_description_name');
			var $textBox = $('<div class="article_description" width="50%"></div>');
			that._viewData.description = replaceImagesSrc(that._viewData.description, that._viewData.images, that._viewData.articleId);
			that._viewData.description = replaceAllScripts(that._viewData.description);
			$textBox.html(that._viewData.description);


			var $buttonFontSizeUp = $('<button class="font_size_up"></button>');
			var $buttonFontSizeDown = $('<button class="font_size_down"></button>');
			var $fontSizeIndicator = $('<span class="font_size_indicator"></span>');
			var isHtml = checkForHtml(that._viewData.description);
			if (isHtml) {
				$buttonFontSizeUp.attr('disabled', 'disabled');
				$buttonFontSizeDown.attr('disabled', 'disabled');
				$buttonFontSizeUp.css('opacity', 0.2);
				$buttonFontSizeDown.css('opacity', 0.2);
			}
			var $container = $('<div class="article_container"></div>');

			var comment = new CommentComponent();
			$(chatHub.init(comment));


			//$(configurator.ReminderHub.init(configurator));

			var $linkContainer = $('<div class="link_container"></div>');
			var $splitter = $('<div class="image_splitter"></div>');
			$splitter.css('left', conf.initialArticleViewSettings.splitterPosition);
			$splitter.off('mousedown');
			var currentPosition = {
				isPressed: false
			};

			$splitter.on('mousedown', function (e) {
				$(document).off('mouseup');
				$(document).off('mousemove');
				currentPosition.isPressed = true;
				currentPosition.initialX = e.pageX;
				$('.common').css('cursor', 'col-resize');
				$splitter.css('z-index', 10);
				$(document).on('mouseup', function (e) {
					currentPosition.isPressed = false;
					$('.common').css('cursor', 'auto');
					$splitter.css('z-index', 0);
					$splitter.css('box-shadow', '');
				});
				$splitter.css('box-shadow', '0px 0px 10px black,0px 0px 10px black,0px 0px 10px black');
				$(document).on('mousemove', function (e) {
					e.preventDefault();
					if (currentPosition.isPressed) {
						var t = e.pageX - 205;

						var dt = $container.width();

						var cdt = (t / dt) * 100;

						if (t - 20 > 40 && t - 20 < dt - 250) {
							$splitter.css('left', cdt + '%');
							$imageContainer.css('width', (100 - cdt - 10) + '%');
							$imageContainer.css('left', cdt + 1 + '%');
							$textBox.css("width", t - 20);
							conf.initialArticleViewSettings.imageContainerWidth = (100 - cdt - 10) + '%';
							conf.initialArticleViewSettings.imageContainerLeft = cdt + 1 + '%';
							conf.initialArticleViewSettings.artilceWidth = t - 20;
							conf.initialArticleViewSettings.splitterPosition = cdt + '%';
						}

					}
				});
			});
			var $imageContainer = $('<div class="image_container"></div>');
			var lst = [];
			var currentLst = [];
			_.each(that._viewData.images, function (image) {
				var shortPath = appPath.getAppPath() + '/images/articleID-' + that._viewData.articleId + '/imageID-' + image.ImageId;
				var path = shortPath + '/size-100x100';
				var $image = $('<img class ="img_icon" article_id="' + that._viewData.articleId + '" image_id="' + image.ImageId + '" image_name="' + image.ImageName + '" src="' + path + '"></img>');
				var $currentImageOuter = $('<img image_name="' + image.ImageName + '" image_id="' + image.ImageId + '" src="' + shortPath + '"></img>');
				currentLst.push($currentImageOuter);
				$image.off('click');
				$image.on('click', function () {
					var currentPath = appPath.getAppPath() + '/images/articleID-' + $(this).attr('article_id') + '/imageID-' + $(this).attr('image_id');
					var $currentImage = $('<img image_name="' + $(this).attr('image_name') + '" image_id="' + $(this).attr('image_id') + '" src="' + currentPath + '"></img>');
					windowHelper.showImage($currentImage, currentLst);
				});

				lst.push($image);
				$imageContainer.append($image);
			}.bind(this));

			$imageContainer.css('width', conf.initialArticleViewSettings.imageContainerWidth);
			$imageContainer.css('left', conf.initialArticleViewSettings.imageContainerLeft);
			$fontSizeIndicator.text(conf.initialArticleViewSettings.fontSize + 'px');
			$textBox.css('width', conf.initialArticleViewSettings.artilceWidth);
			$textBox.css('font-size', conf.initialArticleViewSettings.fontSize);


			$linkContainer.append($a);
			$a.after($name);
			$linkContainer.append(new Rate().render(that._viewData.rate, function (mark) {
				service.updateArticelRate(that._viewData.articleId, mark, $('.rating'))
					.success(function (res) {
						errorHelper.showInfo('Вы поставили оценку ' + mark + ' из 5');
					})
					.error(function (err) {
						errorHelper.showError(err.responseText);
					});
			}));

			var tagCont = new TagContainer();
			$linkContainer.append(tagCont.render(that._viewData.tags));

			$container.append($linkContainer);
			$container.append($textBox);
			$container.append($splitter);
			$container.append($imageContainer);
			$container.append(comment.render(null));
			if (parent.children().length > 0) {
				_.each(parent.children(), function (child) {
					$(child).remove();
				});
			}
			parent.append($container);
			$('.font_size_up').on('click', fontSizeUp);
			$('.font_size_down').on('click', fontSizeDown);
			resizeArticleDescription($container, $textBox);
			window.onresize = function () {
				resizeArticleDescription($container, $textBox);
			};
		};

		var resizeArticleDescription = function ($container, $description) {
			if (!conf.matchMedia) {
				$description.height($container.height() - 184);
			} else {
				$description.height($container.height() - 110);
			}

		};

		return {
			showArticle: function (id, rootElement) {
				var dateTime = new Date();
				return service.getArticleById(id, dateTime, rootElement).error(function (data) {
					errorHelper.showError(data.ErrorMessage);
				}).success(function (data) {
					_.extend(that._viewData, data);
					init(that._viewData);
					document.title = 'Статья "' + data.ArticleName + '" - Статьевод';
					rootElement.filter('.article_loader').remove();
					render(rootElement);
				}).fail(function (data) {
					errorHelper.showError(data.responseText);
				});
			}
		};
	};
	return articleModel;
});