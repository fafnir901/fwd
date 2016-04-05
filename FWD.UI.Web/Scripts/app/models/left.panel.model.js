define([
	'jquery',
	'underscore',
	'app/helpers/configurator.static.helper',
	'app/components/ui/tooltip.component',
	'app/services/left.panel.service',
	'app/helpers/notification.static.helper',
	'app/components/ui/article.sorter.component',
	'app/components/ui/rate.component',
	'app/components/ui/tag.container.component'
],
function ($, _, conf, toolTip, LeftPanelService, errorHelper, ArticleSorter, Rate, TagContainer) {
	var leftPanelModel = function () {
		this.model = {};
		var initialSearchWidth = 145;
		var getModel = function () {
			if (this.model.currentView == undefined) {
				this.model.currentView = 'default';
				this.model.currentSelectedItem = {};
				this.model.articleSorter = new ArticleSorter();
				this.model.articleSorter.init();
			}
			return this.model;
		}.bind(this);

		var currentLeftPanel;
		var searcher;
		var collaps;
		var expand;

		var leftPanel = function () {
			if (currentLeftPanel == undefined || currentLeftPanel == null) {
				currentLeftPanel = $('.left_panel');
			}
			return currentLeftPanel;
		};

		var getSearcher = function () {
			if (searcher == undefined || searcher == null) {
				searcher = $('.searcher');
			}
			return searcher;
		};

		var getCollaps = function () {
			if (collaps == undefined || collaps == null) {
				collaps = $('.collaps');
			}
			return collaps;
		};

		var getExpand = function () {
			if (expand == undefined || expand == null) {
				expand = $('.expand');
			}
			return expand;
		};

		var findAndSetClicableItem = function (articleModel, rootElement) {
			var $clickable = $('.left_panel-item');
			$clickable.on('click', function (item) {
				onClickHanlde($(this), $clickable, articleModel, rootElement);
			});
		};

		var getRootElement = function (rootElement) {
			if ((this._rootElement == undefined || this._rootElement == null) && rootElement != undefined) {
				this._rootElement = rootElement;
			}
			return this._rootElement;
		};

		var onClickHanlde = function (item, clickableArea, articleModel, rootElement) {

			var $class = 'left_panel-item-selection';
			var selected = clickableArea.filter('[selected = "selected"]');
			selected.removeClass($class);
			selected.attr("selected", null);

			var div = $(item);
			div.addClass($class);
			div.attr("selected", "selected");
			var id = div.attr("item-id");
			getModel().currentSelectedItem = {
				id: id,
				selector: '.left_panel-item',
				selectedClass: 'left_panel-item-selection',
				attribute: 'selected',
				leftPanel: leftPanel(),
			};
			articleModel.showArticle(id, rootElement);
		};

		var processExpand = function ($button, $leftPanel, $searcher, $leftButtonContainer, $mainContent, $buttonsContainer) {
			//$button.removeClass('expand');
			//$button.addClass('collaps');
			$leftPanel.stop().animate({ 'width': '200px' });
			$leftButtonContainer.stop().animate({ 'width': '200px' });
			$searcher.stop().animate({ 'width': '145px' });
			$mainContent.stop().animate({ 'margin-left': '201px' });
			$buttonsContainer.stop().animate({ 'left': '221px' });
			$('.image_container').stop().animate({ 'width': '38%' });
			getModel().articleSorter.container.stop().animate({ 'width': '200px' });
		};

		var processCollaps = function ($button, $leftPanel, $searcher, $leftButtonContainer, $mainContent, $buttonsContainer) {
			//$button.removeClass('collaps');
			//$button.addClass('expand');
			$leftPanel.stop().animate({ 'width': '62px' });
			$leftButtonContainer.stop().animate({ 'width': '62px' });
			$searcher.stop().animate({ 'width': '10px' });
			$mainContent.stop().animate({ 'margin-left': '65px' });
			$buttonsContainer.stop().animate({ 'left': '65px' });
			$('.image_container').stop().animate({ 'width': '45%' });
		};

		var makeLeftPanelItemInner = function (id, rate, creationDate, countOfImages, tags, name, isForGroup) {

			var cssClass = "left_panel-item";
			var icon = '<i class="left_panel-item-icon"></i>';
			if (isForGroup) {
				cssClass = "left_panel-item for_group";
				icon = '';
			}

			var $div = $('<div class="' + cssClass + '" item-id="'
					+ id
					+ '" item-rate="'
					+ rate
					+ '"'
					+ ' item-creation-date="'
					+ creationDate
					+ '" item-images-count="'
					+ countOfImages
					+ '"'
					+ '" item-tags="'
					+ tags
					+ '"> '
					+ icon
					+ name
					+ '</div>');

			$div.addClass('animated bounceInRight');
			$div.on('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
				$div.removeClass('animated bounceInRight');
			});
			return $div;
		};

		var makeLeftPanelItem = function (item, afterSearch, isForGroup) {
			isForGroup = isForGroup || false;
			if (afterSearch == undefined) {
				return makeLeftPanelItemInner(item.Id, item.Rate, item.CreationDate, item.CountOfImages, item.Tags, item.Name, isForGroup);
			} else {
				var another = item.Name.split(afterSearch.searchValue);

				var currentName = afterSearch.searchValue.length < 1
					? item.Name
					: afterSearch.searchValue;

				var style = 'background-color: rgba(2, 2, 2, 0.55);';
				if (afterSearch.searchValue.length < 1) {
					another[0] = "";
					another[1] = "";
					style = '';
				}

				var name = another[0]
					+ '<span style="'
					+ style
					+ '">'
					+ currentName
					+ '</span>'
					+ another[1];

				return makeLeftPanelItemInner(item.Id, item.Rate, item.CreationDate, item.CountOfImages, item.Tags, name, isForGroup);
			}
		};

		var updateCounter = function ($buttonsContainer, length) {
			var counter = $('.counter');
			if (counter != undefined) {
				counter.each(function(index,item) {
					var current = $(item);
					if (!current.hasClass('for_group_counter')) {
						current.remove();
					} 
				});
			}
			$buttonsContainer.find('button:first').before($('<div class="counter" title="' + length + '">' + length + '</div>'));
		};

		var processDefaultSearch = function (that, $searcher, $buttonsContainer, isSecond, articleModel, rootElement) {

			var $leftPanel = leftPanel();
			var result = _.filter(leftPanel().currentData, function (item) {
				return item.Name.indexOf(that.val()) != -1;
			});
			var currentValue = that.val();
			$leftPanel.children().remove();

			result.forEach(function (item) {
				$leftPanel.append(makeLeftPanelItem(item, { searchValue: that.val() }));
			});
			getModel().articleSorter.setBehavior(result, function (elements) {
				leftPanel().children().remove();
				$.when(render(elements, articleModel, getRootElement(), isSecond)).then(function () {
					assignTooltip();
				});
				$searcher.val(currentValue);
			});
			findAndSetClicableItem(articleModel, rootElement);
			assignTooltip();
			updateCounter($buttonsContainer, result.length);
		};

		var processGroupSearch = function (that, $searcher, isSecond, articleModel) {
			var result = [];
			var currentValue = that.val();
			var $leftPanel = leftPanel();
			_.each(leftPanel().currentData, function (item) {
				var articles = _.filter(item.Articles, function (art) {
					return art.Name.indexOf(that.val()) != -1;
				});
				var current = {};
				current.Name = item.Name;
				current.IsExpanded = false;
				current.Articles = articles;
				result.push(current);
			});

			var groups = $leftPanel.find('.left_panel-item-group');
			_.each(groups, function (item) {
				var currentResult = _.filter(result, function (res) {
					if ($(item).attr('item_name') == res.Name) {
						return { articles: res.Articles, groupName: res.Name };
					}
				});
				$(item).children().remove();

				_.each(currentResult, function (res) {
					var currentItem = $($leftPanel.find('.counter[item_name="' + res.Name + '"]'));
					currentItem.text(res.Articles.length);
					if (res.Articles.length == 0) {
						$(item).css('display', 'none');
						currentItem.css('display', 'none');
					} else {
						$(item).css('display', 'block');
						currentItem.css('display', 'block');
					}

					_.each(res.Articles, function (art) {
						var $artDiv = makeLeftPanelItem(art, { searchValue: currentValue }, true);
						var $icon = $('<span class="span_icon_for_group_item"></span>');
						$artDiv.append($icon);
						$(item).append($artDiv);
						$artDiv.off('click');
						$artDiv.on('click', function () {
							var $el = $(this);
							clickOnElementWhenTreeViewHandler($el, $leftPanel, articleModel);
						});
					});
				});
				var currentExpanded = function () {
					var element = $(item);
					if (element.attr('isexpanded') == 'true') {
						return element;
					}
					return undefined;
				};
				setAnimateHeight(currentExpanded());
				_.each($leftPanel.find('.left_panel-item-group[isexpanded="true"]'), function (itm) {
					_.each($(itm).find('.span_icon_for_group_item'), function (span) {
						$(span).css({ 'visibility': 'visible' });
					});
				});


			});
			var displayedGroup = _.find(groups, function (group) {
				return $(group).css('display') != 'none';
			});

			var expanded = _.find(groups, function (group) {
				return $(group).attr('isexpanded') == 'true';
			});

			if (displayedGroup != undefined
				&&
				((_.isArray(displayedGroup) && !_.contains(displayedGroup, expanded))
				|| $(displayedGroup).attr('item_name') != $(expanded).attr('item_name'))) {
				animateExpandForTreeView($leftPanel, _.isArray(displayedGroup) ? $(displayedGroup[0]) : $(displayedGroup));
			}
			var currentLength = 0;
			if (displayedGroup != undefined) {
				if (!_.isArray(displayedGroup)) {
					currentLength = $(displayedGroup).children().length;
				} else {
					_.each(displayedGroup, function(g) {
						currentLength += $(g).children().length;
					});
				}
			}

			getModel().articleSorter.setBehavior(result, function (elements) {
				
				var groups = leftPanel().find('.left_panel-item-group');
				var expanded = _.find(groups, function (group) {
					return $(group).attr('isexpanded') == 'true';
				});
				leftPanel().children().remove();
				$.when(renderForGroups(elements, articleModel, getRootElement(), isSecond)).then(function () {
					assignTooltip();
					var lftPanel = $('.left_panel');
					var displayedGroups = lftPanel.find('.left_panel-item-group');
					_.each(displayedGroups, function (gr) {
						var $gr = $(gr);
						var counter = $(lftPanel.find('.counter[item_name="' + $gr.attr('item_name') + '"]'));
						if ($gr.children().length == 0) {
							$gr.css('display', 'none');
							counter.css('display', 'none');
						} else {
							counter.text($gr.children().length);
						}
					});

					var item = $(lftPanel.find('.left_panel-item-group[item_name="' + $(expanded).attr('item_name') + '"]'));
					fullCycleAnimateForGroup(item, lftPanel);
				});
				$searcher.val(currentValue);
			});
			assignTooltip();
			updateCounter($('.buttons_container'), currentLength);
		};

		var render = function (items, articleModel, rootElement, isSecond) {
			getModel().currentView = 'default';
			var $leftPanel = leftPanel();
			$leftPanel.children().remove();
			var $leftButtonContainer = $('.left_panel_searcher');
			var $searcherMover = $('.searcher_mover');
			var $mainContent = $('.main_content');
			var $buttonsContainer = $('.buttons_container');
			items.forEach(function (item) {
				$leftPanel.append(makeLeftPanelItem(item));
			});
			findAndSetClicableItem(articleModel, rootElement);

			updateCounter($buttonsContainer, items.length);
			var $searcher = $(getSearcher());
			$searcher.val('');
			if (isSecond) {
				$searcher.off('focus');
				$searcher.off('blur');
				$searcher.off('keyup');
				$searcherMover.off('mousedown');
				$searcherMover.off('mouseup');

			}

			$searcher.on('focus', function () {
				$(this).addClass('searcher-focused');
				$('.lens').addClass('black_lens');
				$('.lens').removeClass('white_lens');

				if (currentWidth <= currentSearcherWith)
					processExpand(null, $leftPanel, $searcher, $leftButtonContainer, $mainContent, $buttonsContainer);
			});

			var currentWidth = $leftButtonContainer.width();
			var isPressed = false;
			var currentSearcherWith = $searcher.width();

			$searcherMover.on('mousedown', function (e) {
				currentWidth = $leftButtonContainer.width();
				isPressed = true;
				$(this).css('z-index', '10');
				$('.common').css('cursor', 'col-resize');
				$(document).off('mousemove');
				$(document).off('mouseup');
				$(document).on('mouseup', function () {
					isPressed = false;
					$searcherMover.css('z-index', '1');
					$('.common').css('cursor', 'auto');
					processExpand(null, $leftPanel, $searcher, $leftButtonContainer, $mainContent, $buttonsContainer);
				});
				$(document).on('mousemove', function (e) {
					if (isPressed) {
						e.preventDefault();
						currentWidth = e.pageX;
						if (currentWidth >= initialSearchWidth - 60) {
							$searcher.css('width', currentWidth - 50);
							$leftButtonContainer.css('width', currentWidth);
							$leftPanel.css('width', currentWidth);
							$mainContent.css('margin-left', currentWidth);
							$searcherMover.css('left', currentWidth);
							$buttonsContainer.css('left', currentWidth + 20);
							getModel().articleSorter.container.css('width', currentWidth);
						}

					}
				});
			});

			$searcher.on('blur', function () {
				$(this).removeClass('searcher-focused');
				$('.lens').removeClass('black_lens');
				$('.lens').addClass('white_lens');
			});

			$searcher.on('keyup', function (data) {
				var that = $(this);
				processDefaultSearch(that, $searcher, $buttonsContainer, isSecond, articleModel, rootElement);
			});

		};

		var animateExpandForTreeView = function ($leftPanel, element) {
			var def = $.Deferred();
			var $prevElement = $leftPanel.find('.left_panel-item-group[isexpanded="true"]');
			element.attr('isexpanded', 'true');
			setAnimateHeight(element);
			if ($prevElement != undefined && $prevElement.length > 0) {
				$prevElement.attr('isexpanded', 'false');
				$prevElement.stop().animate({ 'height': '30px' }, 100, def.resolve);
			} else {
				def.resolve();
			}
			return def.promise();
		};

		var setAnimateHeight = function (element) {
			if (element != undefined) {
				var curHeight = element.height();
				var autoHeight = element.css({ 'height': 'auto' }).height();
				element.height(curHeight).stop().animate({ height: autoHeight }, 100);
			}
		};

		var clickOnElementWhenTreeViewHandler = function ($el, $leftPanel, articleModel) {
			var id = $el.attr("item-id");
			var $class = 'left_panel-item-selection';
			var selected = $leftPanel.find('[selected = "selected"]');
			selected.removeClass($class);
			selected.attr("selected", null);
			$el.addClass($class);
			$el.attr("selected", "selected");
			getModel().currentSelectedItem = {
				id: id,
				selector: '.left_panel-item',
				selectedClass: 'left_panel-item-selection',
				attribute: 'selected',
				leftPanel: leftPanel(),
				animation: animateExpandForTreeView
			};
			articleModel.showArticle(id, conf.globalRootElement);
		};

		var renderForGroups = function (items, articleModel, rootElement, isSecond) {
			getModel().currentView = 'groupView';
			var $leftPanel = leftPanel();
			$($leftPanel.children()).remove();
			items.forEach(function (item) {

				var $div = $('<div class="left_panel-item-group" isExpanded="false" item_name="'
					+ item.Name + '">'
					+ item.Name + '</div>');

				$div.off('click');
				item.Articles.forEach(function (art) {
					var $artDiv = makeLeftPanelItem(art, undefined, true);
					var $icon = $('<span class="span_icon_for_group_item"></span>');
					$artDiv.append($icon);
					$div.find('.left_panel-item').off('click');
					$div.append($artDiv);
					$div.find('.left_panel-item').on('click', function () {
						var $el = $(this);
						clickOnElementWhenTreeViewHandler($el, $leftPanel, articleModel);
					});

				});
				$div.on('click', function () {
					var element = $(this);
					fullCycleAnimateForGroup(element, $leftPanel);
				});

				$leftPanel.append($('<div class="counter for_group_counter" item_name="'
					+ item.Name + '">'
					+ item.ArticleCount
					+ '</div>'));

				$leftPanel.append($div);
			});
			var $searcher = $(getSearcher());
			$searcher.val('');

			$searcher.off('keyup');

			$searcher.on('keyup', function (data) {
				var that = $(this);
				processGroupSearch(that, $searcher, isSecond, articleModel);
			});
		};

		var fullCycleAnimateForGroup = function (element, $leftPanel) {
			$leftPanel = $leftPanel || leftPanel();
			if (element.attr('isexpanded') == "false") {
				_.each($leftPanel.find('.span_icon_for_group_item'), function (item) {
					$(item).css({ 'visibility': 'hidden' });
				});
				animateExpandForTreeView($leftPanel, element).then(function () {
					_.each(element.find('.span_icon_for_group_item'), function (item) {
						$(item).css({ 'visibility': 'visible' });
					});
				});
			}
		};

		var highlightCurrentItem = function () {
			var path = window.location.pathname.split('/');
			var current = _.find(path, function (element) {
				if (element.indexOf('id') != -1) {
					return element;
				}
			});
			if (current != undefined && current.indexOf('id-') != -1) {
				current = current.replace('id-', '');
				conf.selectItem(current);
			}
		};

		var assignTooltip = function () {
			var $item = $('.left_panel-item');
			toolTip({
				element: $item,
				takeHtmlFromElelement: true,
				takeHtmlAsIs: true,
				additionalContainer: function (item) {
					var rate = new Rate();
					var tagComp = new TagContainer();
					var toolTipElement = $('<div>' + item.text() + '</div>');
					var anothrDdv = $('<div></div>');
					var rateView = rate.render(item.attr('item-rate'));
					rateView.css({ 'font-size': '21px', 'margin-top': '-5px' });
					anothrDdv.append(rateView);
					toolTipElement.append(anothrDdv);
					toolTipElement.append(tagComp.renderFromString(item.attr('item-tags'), { preventDefaultStyle: true, additionalStyle: 'text-align: left;', isSmall: true }));
					return toolTipElement;
				}
			});
		};

		return {
			init: function (articleModel, rootElement, isSecond) {
				var service = new LeftPanelService();
				var root = getRootElement(rootElement);
				return service.getNames(new Date(), rootElement)
					.fail(function (data) {
						errorHelper.showError(data.responseText);
					})
					.success(function (data) {
						leftPanel().currentData = data;
						render(data, articleModel, root, isSecond);
						getModel().articleSorter.setViewMode("default");
						getModel().articleSorter.setBehavior(data, function (items) {
							leftPanel().children().remove();
							$.when(render(items, articleModel, root, isSecond)).then(function () {
								assignTooltip();
								highlightCurrentItem();
							});
						});
						assignTooltip();
					});
			},
			initForGroups: function (articleModel, rootElement, isSecond) {
				var service = new LeftPanelService();
				var root = getRootElement(rootElement);
				return service.getGroups(new Date(), rootElement).fail(function (ex) {
					errorHelper.showError(ex.responseText);
				}).success(function (items) {
					leftPanel().currentData = items;
					getModel().articleSorter.setViewMode("tree");
					renderForGroups(items, articleModel, root, isSecond);
					getModel().articleSorter.setBehavior(items, function (elements) {
						leftPanel().children().remove();
						$.when(renderForGroups(elements, articleModel, root, isSecond)).then(function () {
							assignTooltip();
							highlightCurrentItem();
						});
					});
					assignTooltip();
				});
			},
			getModel: function () {
				return getModel();
			},
			animateForGroup: function (element, $leftPanel) {
				fullCycleAnimateForGroup(element, $leftPanel);
			}
		};
	};

	return leftPanelModel;
});