define([
	'jquery',
	'underscore',
	'app/helpers/notification.static.helper',
	'app/helpers/configurator.static.helper',
	'app/helpers/window.static.helper',
	'app/helpers/appPath.static.helper',
	'app/components/ui/rate.component',
	'app/components/ui/drop.down/drop.down.model.component',
	'app/components/ui/file.uploader.component',
	'app/components/ui/text.editor/text.editor.model.component',
	'app/views/tag.view',
	'app/services/left.panel.service',
	'app/components/hubs/article.hub'
], function ($, _, errorHelper, conf, windowHelper,appPath, Rate, DropDownModel, FileUploader, TextEditorModel, TagView, LeftPanelService, articleHub) {

	var articleView = function () {

		var preInit = function (isForEdit) {
			isForEdit = isForEdit || false;
			var model = {};
			model.uploader = new FileUploader();
			model.mainDiv = $('<div class="create_main"></div>');
			model.headerDiv = $('<div class="header_main"></div>');
			model.headetTable = $('<table></table>');
			model.link = $('<tr><td>Ссылка:</td><td><span><input type="text" id="link"></input></span></td></tr>');
			model.author = $('<tr><td>Автор:</td><td><span><input type="text" id="author"></input></span></td></tr>');
			model.articleName = $('<tr><td>Название:</td><td><span><input type="text" id="articleName"></input></span></td></tr>');

			model.loaderArea = $('.pop_up_window');
			model.loaderCommonArea = $('.common');

			var dropDownModel = new DropDownModel();
			model.group = {};
			model.group.container = $('<tr><td>Группа:</td><td><span></span></td></tr>');
			model.group.dropDown = dropDownModel;
			model.group.button = $('<button class="add_group_show"></button>');

			$(model.group.container.find('span')).append(model.group.button);

			model.rating = {};
			model.rating.container = $('<tr><td>Рейтинг:</td><td><span></span></td></tr>');
			model.rating.model = new Rate();
			model.rating.view = model.rating.model.render(0, function (mark) {
				errorHelper.showInfo('Вы поставили оценку ' + mark + ' из 5');
			});
			$(model.rating.container.find('span')).append(model.rating.view);

			model.save = $('<tr><td></td><td><span><button id="save" type="button">Сохранить</button></span></td></tr>');
			if (!isForEdit) {
				model.saveByRef = $('<tr><td></td><td><span><button id="saveByRef" type="button">Сохранить по ссылке</button></span></td></tr>');
			}
			model.imageContainer = $('<div id="creation_image_container" style="position:initial;"></div>');
			model.content = $('<div class="description_main"></div>');
			var editorModel = new TextEditorModel();

			model.editorModel = editorModel;
			model.content.append(editorModel.render());

			var $div = $('<div class="editor_content"></div>');
			model.content.append(model.editorModel);
			model.content.append($div);

			var uploader = model.uploader.getUploader();
			uploader.css('flex', '1');
			$div.append(uploader);

			model.tagView = {};

			if (conf.featureList.Tag) {
				model.tagView = new TagView();

				model.tagView.init(model.content).success(function (src) {
					$div.append(model.tagView.render(false));
				}).fail(function () {
					$div.append(model.tagView.render(true));
				});
			} else {
				$div.append($('<div class="tag_content"></div>'));
			}


			model.addNewGroup = $('<span><input style="width:10%" id="new_group"></input><button id="addGroup" style="margin-top:0;">Добавить</button> </span>');
			model.groupService = new LeftPanelService();
			return model;
		};

		var validate = function (dto, model) {
			var str = '';
			var errorClass = 'error_element';

			if (dto.dto.ArticleName.length == 0) {
				str += 'Отсутствует название статьи. ';
				model.articleName.find('input').addClass(errorClass);
			} else if (dto.dto.ArticleName.length < 3) {
				str += 'Название статьи должно превышать 3 символа. ';
				model.articleName.find('input').addClass(errorClass);
			} else {
				model.articleName.find('input').removeClass(errorClass);
			}

			if (dto.dto.Link.length == 0) {
				str += 'Отсутствует ссылка на источник. ';
				model.link.find('input').addClass(errorClass);
			} else if (dto.dto.Link.length < 5) {
				str += 'Ссылка на источник должно превышать 5 символов. ';
				model.link.find('input').addClass(errorClass);
			}
			else {
				model.link.find('input').removeClass(errorClass);
			}

			if (dto.dto.Content.length == 0) {
				str += 'Отсутствует текст статьи. ';
				model.editorModel.getViewModel().components.commonContainer.addClass(errorClass);
			}
			else if (dto.dto.Content.length < 100) {
				str += 'Текст статьи должен превышать 100 символов. ';
				model.editorModel.getViewModel().components.commonContainer.addClass(errorClass);
			}
			else {
				model.editorModel.getViewModel().components.commonContainer.removeClass(errorClass);
			}

			return str;
		};

		var replaceAllScripts = function (text) {
			text = text.replace(/<script/gmi, '&lt;script').replace(/<\/script>/gmi, '&lt;/script&gt;');
			//text = text.replace(/<.*[\/>]$/g, '&lt;.*[\/&gt]$');
			text = text.replace(/[<]button|[<]input.*[\/>]$/g, '[&lt;]button|[&lt;]input.*[\/&gt;]$');
			return text;
		};

		var getDtoFromControls = function (viewModel, isUpdate) {
			isUpdate = isUpdate || false;
			var images = $('#creation_image_container').children();
			var dto = {
				dto: {
					ArticleName: $('#articleName').val(),
					isUpdate: isUpdate,
					AuthorName: $('#author').val(),
					Link: $('#link').val(),
					Content: replaceAllScripts($('.editor_body').html()),
					Group: viewModel.group.dropDown.getCurrentValue() || 'Без группы',
					ImageIds: images.length ? _.map(images, function (item) { return $(item).attr('image_id'); }) : null,
					Tags: viewModel.tagView.getModel().tags,
					Rate: viewModel.rating.model.getModel().mark
				}
			};
			return dto;
		};

		var saveHandle = function (viewModel, articleService, model) {
			return function () {
				var dto = getDtoFromControls(viewModel);
				var errorMsg = validate(dto, viewModel);
				if (errorMsg.length == 0) {
					var dateTime = new Date();
					articleService.saveArticle(dto, dateTime, viewModel.loaderCommonArea)
						.success(function (data) {
							errorHelper.showInfo("Статья была добавлена");

							$(articleHub.init(dto));

							windowHelper.hidePopUp();
							model._leftMenuRoot.children().stop().remove();
							if (model._leftMenuModel.currentView == 'default') {
								model._leftMenuModel.init(model._artilceModel, model._leftMenuRoot, true).then(function () {
									var path = window.location.pathname.split('/');
									var current = _.find(path, function (item) {
										if (item.indexOf('id') != -1) {
											return item;
										}
									});
									if (current != undefined && current.indexOf('id-') != -1) {
										current = current.replace('id-', '');
										var $element = conf.selectItem(current);
									}
									var counter = $('.counter').first();
									var currentValue = parseInt($(counter).text());
									currentValue++;
									$(counter).text(currentValue);
								});
							} else {
								model._leftMenuModel.initForGroups(model._artilceModel, model._leftMenuRoot, true).then(function () {
									var path = window.location.pathname.split('/');
									var current = _.find(path, function (item) {
										if (item.indexOf('id') != -1) {
											return item;
										}
									});
									if (current != undefined && current.indexOf('id-') != -1) {
										current = current.replace('id-', '');
										var $element = conf.selectItem(current);
									}
									var counter = $('.counter').first();
									var currentValue = parseInt($(counter).text());
									currentValue++;
									$(counter).text(currentValue);
								});
							}
						})
						.fail(function (data) {
							errorHelper.showError(data.responseText);
						});
				} else {
					errorHelper.showError(errorMsg);
				}
			}.bind(this);
		}.bind(this);

		var updateHandle = function (viewModel, articleService, model, id) {
			return function () {
				var dto = getDtoFromControls(viewModel, true);
				dto.dto.ArticleId = id;

				var errorMsg = validate(dto, viewModel);

				if (errorMsg.length == 0) {
					articleService.updateArticle(dto, null, viewModel.loaderCommonArea)
						.success(function (data) {

							$(articleHub.init(dto));

							errorHelper.showInfo("Статья была обновлена");
							windowHelper.hidePopUp();

							model._leftMenuRoot.children().stop().remove();
							if (model._leftMenuModel.currentView == 'default') {
								model._leftMenuModel.init(model._artilceModel, model._leftMenuRoot, true).then(function () {
									var path = window.location.pathname.split('/');
									var current = _.find(path, function (item) {
										if (item.indexOf('id') != -1) {
											return item;
										}
									});
									if (current != undefined && current.indexOf('id-') != -1) {
										current = current.replace('id-', '');
										var $element = conf.selectItem(current);
										$element.click();
									}
								});
							} else {
								model._leftMenuModel.initForGroups(model._artilceModel, model._leftMenuRoot, true).then(function () {
									var path = window.location.pathname.split('/');
									var current = _.find(path, function (item) {
										if (item.indexOf('id') != -1) {
											return item;
										}
									});
									if (current != undefined && current.indexOf('id-') != -1) {
										current = current.replace('id-', '');
										var $element = conf.selectItem(current);
										$element.click();
									}
								});
							}

						})
						.fail(function (data) {
							errorHelper.showError(data.responseText);
						});
				} else {
					errorHelper.showError(errorMsg);
				}
			}.bind(this);
		}.bind(this);

		var init = function (articleService, model) {
			var viewModel = preInit();
			var isSecond = false;
			var addGroupShow = viewModel.group.button;
			addGroupShow.off('click');
			addGroupShow.on('click', function () {
				if (!isSecond) {
					$(this).animate({ 'margin-left': '25%' }, 200,
						function () {
							viewModel.addNewGroup.css('visibility', 'visible');
							this.before(viewModel.addNewGroup);
							viewModel.addNewGroup.show();
							this.css({ 'margin-left': '5px' });
							this.removeClass('add_group_show');
							this.addClass('add_group_hide');
							isSecond = true;
						}.bind($(this)));
				} else {
					$(viewModel.addNewGroup).animate({ 'margin-left': '0px' }, 400,
						function () {
							viewModel.addNewGroup.css('visibility', 'hidden');

							this.removeClass('add_group_hide');
							this.animate({ 'margin-left': '-21%' }, 400, function () {
								viewModel.addNewGroup.remove();
								this.css({ 'margin-left': '5px' });
								this.css({ 'position': 'relative' });
							}.bind(this));
							this.addClass('add_group_show');
							isSecond = false;
						}.bind($(this)));
				}
			});

			$(viewModel.addNewGroup.find('#addGroup')).on('click', function (e) {
				e.stopPropagation();
				var newGroup = $('#new_group').val();
				var groups = viewModel.group.dropDown.getModel().items;

				var match = _.find(groups, function (item) {
					return item == newGroup;
				});
				if (match != undefined) {
					errorHelper.showError('Группа "' + newGroup + '" уже существует');
				} else {
					articleService.saveGroup(newGroup, viewModel.addNewGroup).success(function (data) {
						errorHelper.showInfo('Новая группа "' + newGroup + '" успешно добавлена');
						viewModel.group.dropDown.addAndRenderItem(newGroup);
					}).fail(function (data) {
						errorHelper.showError(data.responseText);
					});
				}
			});
			$(viewModel.save.find('button')).on('click', function (e) {

				e.stopPropagation();
				e.preventDefault();
				e.cancelBubble = true;
				saveHandle(viewModel, articleService, model, e)();
			});

			$(viewModel.saveByRef.find('button')).on('click', function (e) {
				e.stopPropagation();
				var str = '';
				var errorClass = 'error_element';
				var link = $('#link').val();
				if (link.length < 5) {
					str += 'Ссылка на источник должно превышать 5 символов. ';
					viewModel.link.find('input').addClass(errorClass);
				}
				if (str.length == 0) {
					articleService.saveArticleByRef(link, $('.common'))
						.success(function (data) {
							errorHelper.showInfo("Данные успешно загружены");
							viewModel.content.val(data);
							$('#content').val(data.Content);
						})
						.fail(function (data) {
							errorHelper.showError(data.responseText);
						});
				} else {
					errorHelper.showError(str);
				}
			});

			viewModel.groupService.getGroups(null, viewModel.loaderArea).success(function (data) {
				var items = [];
				_.each(data, function (item) {
					var def = item.Name == 'Без группы' ? true : false;
					items.push({
						text: item.Name,
						default: def
					});
				});

				viewModel.group.dropDown.addItems(items);

				viewModel.headetTable.append(viewModel.link);
				viewModel.headetTable.append(viewModel.author);
				viewModel.headetTable.append(viewModel.articleName);

				$(viewModel.group.container.find('span')).append(viewModel.group.dropDown.render());
				viewModel.headetTable.append(viewModel.group.container);
				viewModel.headetTable.append(viewModel.rating.container);
				viewModel.headetTable.append(viewModel.save);
				viewModel.headetTable.append(viewModel.saveByRef);
				viewModel.headerDiv.append(viewModel.headetTable);
				viewModel.headerDiv.append(viewModel.imageContainer);
				viewModel.mainDiv.append(viewModel.headerDiv);
				viewModel.mainDiv.append(viewModel.content);

			}).fail(function (data) {
				errorHelper.showError(data.responseText);
			});
			return viewModel.mainDiv;
		};

		var initForEdit = function (articleService, topMenuModel, id) {
			var viewModel = preInit(true);
			viewModel.groupService.getGroups(null, viewModel.loaderArea).success(function (data) {
				var items = [];
				_.each(data, function (item) {
					var def = item.Name == 'Без группы' ? true : false;
					items.push({
						text: item.Name,
						default: def
					});
				});

				viewModel.group.dropDown.addItems(items);

				viewModel.headetTable.append(viewModel.link);
				viewModel.headetTable.append(viewModel.author);
				viewModel.headetTable.append(viewModel.articleName);

				$(viewModel.group.container.find('span')).append(viewModel.group.dropDown.render());
				viewModel.headetTable.append(viewModel.group.container);
				viewModel.headetTable.append(viewModel.rating.container);
				viewModel.headetTable.append(viewModel.save);
				viewModel.headetTable.append(viewModel.saveByRef);
				viewModel.headerDiv.append(viewModel.headetTable);
				viewModel.headerDiv.append(viewModel.imageContainer);
				viewModel.mainDiv.append(viewModel.headerDiv);
				viewModel.mainDiv.append(viewModel.content);

			}).fail(function (data) {
				errorHelper.showError(data.responseText);
			}).then(function () {
				articleService.getArticleById(id, null, viewModel.loaderArea).success(function (dto) {
					$('#link').val(dto.Link);
					$('#author').val(dto.AuthorName);
					$('#articleName').val(dto.ArticleName);
					try {
						$('.editor_body').html(dto.Content);
					} catch (e) {
						$('.editor_body').text(dto.Content);
					}

					viewModel.group.dropDown.setCurrentValue(dto.Group);
					viewModel.rating.model.setCurrentMark(dto.Rate);
					var container = $('#creation_image_container');
					_.each(dto.Images, function (item) {
						var scr = appPath.getAppPath() + '/images/articleID-' + id + '/imageID-' + item.ImageId + '/size-100x100';
						var img = $('<image image_id="' + item.ImageId + '" class="img_icon" src="' + scr + '"/>');
						container.append(img);
					});

					$('.img_icon').off('mouseenter');
					$('.img_icon').off('mouseleave');

					$('.img_icon').on('mouseenter', function () {
						var that = $(this);
						var buttonDelete = $('<button class="delete_image" image_id="' + that.attr('image_id') + '"></button>');
						that.before(buttonDelete);
						buttonDelete.on('click', function () {
							var id = $(this).attr('image_id');
							$('.img_icon[image_id=' + id + ']').remove();
							$(this).remove();
						});
					});

					$('.img_icon').on('mouseleave', function () {
						var that = $(this);
						_.delay(function () {
							that.prev().remove();
						}, 1000);

					});

					viewModel.tagView.appendExisting(dto.Tags);
				}).fail(function (data) {
					errorHelper.showError(data.responseText);
				});
			});


			var isSecond = false;
			var currentId = id;
			var addGroupShow = viewModel.group.button;
			addGroupShow.off('click');
			addGroupShow.on('click', function () {
				if (!isSecond) {
					$(this).animate({ 'margin-left': '25%' }, 200,
						function () {
							viewModel.addNewGroup.css('visibility', 'visible');
							this.before(viewModel.addNewGroup);
							viewModel.addNewGroup.show();
							this.css({ 'margin-left': '5px' });
							this.removeClass('add_group_show');
							this.addClass('add_group_hide');
							isSecond = true;
						}.bind($(this)));
				} else {
					$(viewModel.addNewGroup).animate({ 'margin-left': '0px' }, 400,
						function () {
							viewModel.addNewGroup.css('visibility', 'hidden');

							this.removeClass('add_group_hide');
							this.animate({ 'margin-left': '-21%' }, 400, function () {
								viewModel.addNewGroup.remove();
								this.css({ 'margin-left': '5px' });
								this.css({ 'position': 'relative' });
							}.bind(this));
							this.addClass('add_group_show');
							isSecond = false;
						}.bind($(this)));
				}
			});

			$(viewModel.addNewGroup.find('#addGroup')).on('click', function (e) {
				e.stopPropagation();
				var newGroup = $('#new_group').val();
				var groups = viewModel.group.dropDown.getModel().items;
				var match = _.find(groups, function (item) {
					return item == newGroup;
				});
				if (match != undefined) {
					errorHelper.showError('Группа "' + newGroup + '" уже существует');
				} else {
					articleService.saveGroup(newGroup, viewModel.addNewGroup).success(function (data) {
						errorHelper.showInfo('Новая группа "' + newGroup + '" успешно добавлена');
						viewModel.group.dropDown.addAndRenderItem(newGroup);
					}).fail(function (data) {
						errorHelper.showError(data.responseText);
					});
				}
			});
			$(viewModel.save.find('button')).on('click', function (e) {
				e.stopPropagation();
				e.preventDefault();
				e.cancelBubble = true;
				updateHandle(viewModel, articleService, topMenuModel, currentId)();
			});

			return viewModel.mainDiv;
		};

		return {
			getView: function (articleService, topMenuModel, isForEdit, id) {
				isForEdit = isForEdit || false;
				if (!isForEdit) {
					return init(articleService, topMenuModel);
				} else {
					return initForEdit(articleService, topMenuModel, id);
				}
			},
		};
	};

	return articleView;
});