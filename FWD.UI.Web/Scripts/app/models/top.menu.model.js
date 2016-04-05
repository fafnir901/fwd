define([
	'jquery',
	'underscore',

	'app/helpers/notification.static.helper',
	'app/helpers/window.static.helper',
	'app/helpers/configurator.static.helper',
	'app/helpers/appPath.static.helper',

	'app/services/top.menu.service',
	'app/services/article.service',

	'app/views/article.editor.view',
	'app/views/plan.view',
	'app/views/transaction.view',
	'app/views/search.view',
	'app/views/statistics.view',

	'app/components/hubs/article.hub',
	'app/components/ui/quick.mail.sender.component',

	'app/models/statistics.model'
], function ($, _,
	errorHelper, windowHelper, conf, appPath,
	TopMenuService, ArticleService,
	ArticleEditorView, PlanView, TransactionView, SearchView, StatisticsView,
	articleHub, QuickMailSender,
	StatisticsModel) {
	var topMenuModel = function () {

		var that = this;
		var topMenuPanel;
		var jQuerySub;
		var topMenu = function () {
			if (topMenuPanel == undefined || topMenuPanel == null) {
				topMenuPanel = $('.top_menu');
			}
			return topMenuPanel;
		};

		var getButtonContainer = function () {
			if (jQuerySub == undefined || jQuerySub == null) {
				jQuerySub = $('.buttons_container');
			}
			return jQuerySub;
		};

		var checkForVisibility = function (model, $element, needToFirstPosition) {
			needToFirstPosition = needToFirstPosition == undefined ? false : needToFirstPosition;
			if (conf.currentUser.roleId == conf.admin) {
				if (!needToFirstPosition) {
					model._buttonsContainer.append($element);
				} else {
					var firstChild = $(model._buttonsContainer.children().first());
					firstChild.before($element);
				}
			}
			return model;
		};

		var init = function (model) {
			model._sourceType = 'db';
			model._rootElement = $('.navigate_panel');
			model._buttonsContainer = getButtonContainer();
			model._topMenuService = new TopMenuService();
			model._toggler = 'toggler_db';
			model._altToggler = 'toggler_xml';
			model._currentToggler = model._toggler;
			model.service = new TopMenuService();
			model.mainDiv = model._buttonsContainer;
			this._model = model;
			return model;
		};

		var renderToggler = function (model, isSecond) {
			if (!isSecond) {
				var sourceToggler = $('<button></button>');
				sourceToggler.addClass(model._currentToggler);
				var titleText = '';
				if (model._currentToggler == model._toggler) {
					titleText = "Переключить на XML";
					sourceToggler.removeClass('show_db');
					sourceToggler.addClass('show_xml');

				} else {
					titleText = "Переключить на DB";
					sourceToggler.removeClass('show_xml');
					sourceToggler.addClass('show_db');
				}
				sourceToggler.attr('title', titleText);
				sourceToggler.on('click', function () {
					var that = $(this);

					var currentView = model._leftMenuModel.getModel().currentView;
					if (currentView == 'default') {
						model._leftMenuModel.init(model._artilceModel, model._leftMenuRoot, true);
					} else {
						model._leftMenuModel.initForGroups(model._artilceModel, model._leftMenuRoot, true);
					}
					if (model._currentToggler == model._toggler) {
						$('.save_to_db').hide();
						$('.save_to_xml').show();
					} else {
						$('.save_to_db').show();
						$('.save_to_xml').hide();
					}
				});
				model = checkForVisibility(model, sourceToggler);
			} else {
				var sourceToggler = $('.' + model._currentToggler);
				var titleText = '';
				if (model._currentToggler == model._toggler) {
					titleText = "Переключить на XML";
					sourceToggler.removeClass('show_db');
					sourceToggler.addClass('show_xml');
				} else {
					titleText = "Переключить на DB";
					sourceToggler.removeClass('show_xml');
					sourceToggler.addClass('show_db');
				}
				sourceToggler.attr('title', titleText);
				model = checkForVisibility(model, sourceToggler, true);
			}
			return model;
		};

		var renderEdit = function (model, isSecond) {
			if (!isSecond) {
				var $editButton = $('<button title="Редактировать"></button>');
				$editButton.addClass('edit');
				$($editButton).on('click', function () {

					var selected = $('.left_panel-item').filter('[selected="selected"]');

					if (selected != undefined && selected.length > 0) {
						var id = selected.attr('item-id');
						var view = new ArticleEditorView();
						var mainDiv = $(view.getView(new ArticleService(), model, true, id));
						windowHelper.showPopUp(mainDiv);
					} else {
						errorHelper.showError('Статья не выбрана');
					}

				});
				model = checkForVisibility(model, $editButton);
			}
			return model;
		};

		var renderShowComments = function (model, isSecond) {
			if (!isSecond) {
				var $showCommentButton = $('<button title=""></button>');
				$showCommentButton.attr('title', 'Комментарии');
				$showCommentButton.addClass('comments');
				$($showCommentButton).on('click', function () {
					conf.isShow = !conf.isShow;
					var text = '';
					if (conf.isShow) {
						text = 'Скрыть комментарии';
						$(".common_comment_container").slideDown();
					} else {
						text = 'Показать коментарии';
						$(".common_comment_container").slideUp();
					}

					$(this).attr('title', text);
				});
				model._buttonsContainer.append($showCommentButton);
			}
			return model;
		};

		var renderDelete = function (model, isSecond) {
			if (!isSecond) {
				var $deleteButton = $('<button title="Удалить"></button>');
				$deleteButton.addClass('menu_delete');
				$($deleteButton).on('click', function () {
					var confirmation = confirm('Вы уверены?');
					if (confirmation == true) {
						var selected = $('.left_panel-item').filter('[selected="selected"]');
						new ArticleService().deleteArticle(selected.attr('item-id'), new Date(), $('.navigate_panel'))
							.fail(function (data) {
								errorHelper.showError(data.responseText);
							})
							.success(function (data) {
								var dto = { dto: data };
								dto.dto.ArticleName = selected.text();
								dto.dto.isDeleted = true;
								selected.remove();
								$('.left_panel-item').first().click();
								errorHelper.showInfo('Статья успешно удалена.');

								$(articleHub.init(dto));

								var counter = $('.buttons_container .counter');
								var currentValue = parseInt(counter.text());
								currentValue--;
								counter.text(currentValue);
							});
					}
				});
				model = checkForVisibility(model, $deleteButton);
			}
			return model;
		};

		var renderPlanView = function (model, isSecond) {
			if (!isSecond) {
				var $planButton = $('<button title="План"></button>');
				$planButton.addClass('plan');
				$($planButton).on('click', function () {
					var planView = new PlanView();

					planView.render().then(function () {
						var container = planView.getView().panelContainer;
						windowHelper.showPopUp(container);
					});

				});
				model = checkForVisibility(model, $planButton);
			}
			return model;
		};

		var renderTransView = function (model, isSecond) {
			if (!isSecond) {
				var $tranButton = $('<button title="Журнал транзакций"></button>');
				$tranButton.addClass('tran');
				$($tranButton).on('click', function () {
					var transactionView = new TransactionView();

					transactionView.render().then(function () {
						var container = transactionView.getView().panelContainer;
						windowHelper.showPopUp(container);
					});

				});
				model = checkForVisibility(model, $tranButton);
			}
			return model;
		};

		var renderSendEmail = function (model, isSecond) {
			if (!isSecond) {
				var emailSendButton = $('<button title="Послать Email"></button>');
				emailSendButton.addClass('mail');

				$(emailSendButton).on('click', function () {
					var date = new Date();
					new ArticleService().sendEmail(date, $('.navigate_panel')).fail(function (data) {
						errorHelper.showError(data.responseText);
					}).success(function (data) {
						errorHelper.showInfo('Email на адрес "' + data + '" успешно отправлен.');
					});
				});

				model._buttonsContainer.append(emailSendButton);
			}
			return model;
		};

		var renderSendThisEmail = function (model, isSecond) {
			if (!isSecond) {
				var emailSendButton = $('<button title="Послать текущий Email"></button>');
				emailSendButton.addClass('mail_quick');

				$(emailSendButton).on('click', function () {
					var selected = $('.left_panel-item').filter('[selected="selected"]');
					var artId = selected.attr("item-id");
					var quick = new QuickMailSender(artId, $('.navigate_panel'));

					var item = quick.render(emailSendButton);
					$('.common').find('.quick_mail_sender').remove();
					$('.common').append(item);
					item.slideDown();
				});

				model._buttonsContainer.append(emailSendButton);
			}
			return model;
		};

		var renderCreate = function (model, isSecond) {
			if (!isSecond) {
				var $createButton = $('<button title="Создать"></button>');
				$createButton.addClass('new');
				$($createButton).on('click', function () {
					var view = new ArticleEditorView();
					windowHelper.showPopUp($(view.getView(new ArticleService(), model)));
				});
				model._buttonsContainer.append($createButton);
			}
			return model;
		};

		var renderSearch = function (model, isSecond) {
			if (!isSecond) {
				var $searchBtn = $('<button title="Поиск"></button>');
				$searchBtn.addClass('search');
				$($searchBtn).on('click', function () {
					var searchView = new SearchView();
					windowHelper.showPopUp(searchView.render());
				});
				model._buttonsContainer.append($searchBtn);
			}
			return model;
		};

		var renderStat = function (model, isSecond) {
			if (!isSecond) {
				var $statButton = $('<button title="Статистика"></button>');
				$statButton.addClass('statistics');
				$($statButton).on('click', function () {
					var id = Number(_.last(_.last(window.location.pathname.split('/')).split('-')));
					if (isNaN(id) || id == undefined) {
						errorHelper.showError('Статья не выбрана');
					} else {
						var statView = new StatisticsView();
						statView.init(id, $(".left_panel_searcher"))
							.then(function (data) {
								var statModel = new StatisticsModel();
								$.when(statView.render()).then(function (cont) {

									var rendered = function () {
										statModel.init("container", cont.width(), $('.pop_up_window').height() - 150, { Name: data.ArticleName }, statView.getModel().elements.canvas);
										statView.setStatModel(statModel);
									};
									windowHelper.showPopUp(cont, false, rendered);
								});
							});
					}
				});
				model._buttonsContainer.append($statButton);
			}
			return model;
		};

		var renderSaveToXml = function (model, isSecond) {
			if (!isSecond) {
				var $saveToXmlButton = $('<button title="Сохранить в XML"></button>');
				$saveToXmlButton.addClass('save_to_xml');
				$($saveToXmlButton).on('click', function () {
					var selected = $('.left_panel-item').filter('[selected="selected"]');
					var articleId = selected.attr('item-id');
					model.service.saveToXMl(articleId, $('.navigate_panel'))
						.success(function (data) {
							errorHelper.showInfo('Статья успешно сохранена в XML');
						})
						.fail(function (data) {
							errorHelper.showError(data.responseText);
						});
				});
				model = checkForVisibility(model, $saveToXmlButton);
			}
			return model;
		};

		var renderSaveToDb = function (model, isSecond) {
			if (!isSecond) {
				var $saveToDblButton = $('<button title="Сохранить в БД"></button>');
				$saveToDblButton.addClass('save_to_db');
				$saveToDblButton.css({ 'display': 'none' });
				$($saveToDblButton).on('click', function () {
					var selected = $('.left_panel-item').filter('[selected="selected"]');
					var articleId = selected.attr('item-id');
					model.service.saveToDb(articleId, $('.navigate_panel'))
						.success(function (data) {
							errorHelper.showInfo('Статья успешно сохранена в БД');
						})
						.fail(function (data) {
							errorHelper.showError(data.responseText);
						});
				});
				model = checkForVisibility(model, $saveToDblButton);
			}
			return model;
		};

		var renderTreeSwitcher = function (model, isSecond) {
			if (!isSecond) {
				var $showGroups = $('<button title="Отобразить группы"></button>');
				$($showGroups).off('click');
				$showGroups.addClass('show_group');
				var needShow = true;
				$($showGroups).on('click', function () {
					if (needShow) {
						$(this).attr('title', 'Не отображать группы');
						$(this).removeClass('show_group');
						$(this).addClass('show_list');


						model._leftMenuModel.initForGroups(model._artilceModel, model._leftMenuRoot, true).then(function () {
							var path = window.location.pathname.split('/');
							var current = _.find(path, function (item) {
								if (item.indexOf('id') != -1) {
									return item;
								}
							});
							if (current != undefined && current.indexOf('id-') != -1) {
								current = current.replace('id-', '');
								conf.selectItem(current);
							}
						});

					} else {
						$(this).attr('title', 'Отобразить группы');
						$(this).removeClass('show_list');
						$(this).addClass('show_group');


						model._leftMenuModel.init(model._artilceModel, model._leftMenuRoot, true).then(function () {
							var path = window.location.pathname.split('/');
							var current = _.find(path, function (item) {
								if (item.indexOf('id') != -1) {
									return item;
								}
							});
							if (current != undefined && current.indexOf('id-') != -1) {
								current = current.replace('id-', '');
								conf.selectItem(current);
							}
						});

					}
					needShow = !needShow;
				});
				model._buttonsContainer.append($showGroups);
			}
			return model;
		};

		var renderInfo = function (model, isSecond) {
			if (!isSecond) {
				var $info = $('<button title="Информация"></button>');
				$info.addClass('weight_info');
				$($info).on('click', function () {
					var that = $(this);

					model.service.getInfo($(".left_panel_searcher"))
						.success(function (data) {
							errorHelper.showInfo(data, true);
						})
						.fail(function (data) {
							errorHelper.showError(data.responseText);
						});
				});
				model = checkForVisibility(model, $info);
			}
			return model;
		};

		var renderExport = function (model, isSecond) {
			if (!isSecond) {
				var $export = $('<button title="Экспорт"></button>');
				$export.addClass('export');
				$($export).on('click', function () {
					var form = $('<form id="exportForm" method="POST" action="' + appPath.getAppPath() + '/export" target="_blank"></form>');
					$(this).before(form);
					form.submit();
					form.remove();
				});
				model = checkForVisibility(model, $export);
			}
			return model;
		};

		var renderExportCurrent = function (model, isSecond) {
			if (!isSecond) {
				var $export = $('<button title="Экспорт данной статьи"></button>');
				$export.addClass('export_one');
				$($export).on('click', function () {
					var form = $('<form id="exportForm" method="POST" action="' + appPath.getAppPath() + '/export/current" target="_blank"></form>');
					var id = $('<input type="hidden" id="articleId" name="articleId"/>');
					var name = $('<input type="hidden" id="articleName" name="articleName"/>');

					var selected = $('.left_panel-item').filter('[selected="selected"]');

					id.val(selected.attr("item-id"));
					name.val(selected.text());

					form.append(id);
					form.append(name);

					$(this).before(form);
					form.submit();
					form.remove();
				});
				model = checkForVisibility(model, $export);
			}
			return model;
		};

		var upload = function (file, param) {
			var xhr = new XMLHttpRequest();
			xhr.onload = xhr.onerror = function (e) {
				if (this.status != 200) {
					param.onError(this.responseText);
					return;
				}
				return param.onSuccess(e);
			};

			xhr.upload.onprogress = function (event) {
				//param.onProgress(event.loaded, event.total, param.container);
				conf.beforeSend($('.navigate_panel'));
			};
			xhr.open("POST", param.url, true);

			var formData = new FormData();
			formData.append("files", file);

			return xhr.send(formData);
		};

		var renderImport = function (model, isSecond) {

			var $import = $('<button title="Импорт"></button>');
			$import.addClass('import');
			$($import).on('click', function () {
				var fileUploadHidden = $('<input type="file" id="fileUploader" value="1000000000" style="display:none"/>');
				$import.after(fileUploadHidden);

				fileUploadHidden.off('click');
				fileUploadHidden.click();

				fileUploadHidden.off('change');
				fileUploadHidden.on('change', function () {
					var res = $('#fileUploader')[0].files[0];
					var formData = new FormData();
					formData.append("files", res);

					model.service.importData(formData, $('.left_panel_searcher'))
						.success(function (text) {
							fileUploadHidden = null;
							$('#fileUploader').remove();
							errorHelper.showInfo(text, true);


							var currentView = model._leftMenuModel.getModel().currentView;
							if (currentView == 'default') {
								model._leftMenuModel.init(model._artilceModel, model._leftMenuRoot, true);
							} else {
								model._leftMenuModel.initForGroups(model._artilceModel, model._leftMenuRoot, true);
							}

						})
						.fail(function (e) {
							fileUploadHidden = null;
							$('#fileUploader').remove();
							errorHelper.showError(e.responseText);
						});
					$('#fileUploader').remove();
				});

			});

			if (!isSecond)
				model = checkForVisibility(model, $import);

			return model;
		};

		var makeDefferedCall = function (func) {
			var $def = $.Deferred();
			func($def.resolve);
			return $def.promise();
		};

		var renderMainMenu = function (model) {
			var $mainMenu = $('<button title="Меню"></button>');
			$mainMenu.addClass('main_menu');
			model._buttonsContainer.children().first().before($mainMenu);
			var isClicked = true;
			$mainMenu.off('click');
			$mainMenu.on('click', function () {
				var items = model._buttonsContainer.children();
				if (isClicked) {
					$(this).css({ 'transform': 'rotate(180deg)' });
					isClicked = false;
				} else {
					$(this).css({ 'transform': 'rotate(0deg)' });
					isClicked = true;
				}
				var revertItems = [];
				for (var i = items.length - 1; i >= 0; i--) {
					revertItems.push(items[i]);
				}
				var d = 0;
				_.each(revertItems, function (item) {
					$(item).stop().animate({ 'visibility': 'visible' }, d, function () {
						var $item = $(item);
						if (!$item.hasClass('main_menu') && !$item.hasClass('user_info')) {
							var cl = $item.hasClass('display') ? 'hidden' : 'display';

							$item.hasClass('display') ? $item.removeClass('display') : $item.removeClass('hidden');
							$item.addClass(cl);
						}
					});
					d += 50;
				});
			});

			return model;
		};

		var renderSeparator = function (model) {
			var $separator = $('<div class="separator"></div>');
			model._buttonsContainer.append($separator);
		};

		var renderContainers = function (model, isSecond) {
			if (conf.featureList.SwitchToXml)
				renderToggler(model, isSecond);

			renderSeparator(model);

			if (conf.featureList.Edit)
				renderCreate(model, isSecond);

			if (conf.featureList.Add)
				renderEdit(model, isSecond);

			if (conf.featureList.Remove)
				renderDelete(model, isSecond);

			renderSeparator(model);

			if (conf.featureList.Plan)
				renderPlanView(model, isSecond);

			if (conf.featureList.SwitchEmail) {
				renderSendEmail(model, isSecond);
				renderSendThisEmail(model, isSecond);
			}

			renderSeparator(model);

			if (conf.featureList.SwitchAnotherView)
				renderTreeSwitcher(model, isSecond);

			renderSeparator(model);

			if (conf.featureList.SaveToXml)
				renderSaveToXml(model, isSecond);

			if (conf.featureList.SaveToDb)
				renderSaveToDb(model, isSecond);

			renderSeparator(model);

			if (conf.featureList.ShowComments)
				renderShowComments(model, isSecond);

			if (conf.featureList.Statistics)
				renderStat(model, isSecond);

			if (conf.featureList.Information)
				renderInfo(model, isSecond);

			renderSeparator(model);

			if (conf.featureList.Export)
				renderExport(model, isSecond);

			if (conf.featureList.Import)
				renderImport(model, isSecond);
			if (conf.featureList.Export)
				renderExportCurrent(model, isSecond);

			renderSeparator(model);

			if (conf.featureList.Tranc)
				renderTransView(model, isSecond);

			if (conf.featureList.Search)
				renderSearch(model, isSecond);

			//_renderTagsView(model, isSecond);

			_.each(model._buttonsContainer.children(), function (item) {
				$(item).addClass("top_menu_button");
			});
			//_renderMorze(model, isSecond);//доделать,потом расскоментить
			//model._rootElement.append(model._buttonsContainer);

			renderMainMenu(model);
			return model;
		};

		var render = function (model, isSecond) {
			renderContainers(model, isSecond);
			return model;
		};
		return {
			init: function (leftMenuModel, articleModel, leftMenuRoot, globalRootElement, userContainer) {
				this._leftMenuModel = leftMenuModel;
				this._artilceModel = articleModel;
				this._leftMenuRoot = leftMenuRoot;
				var model = init(this);
				model = render(model, false, leftMenuModel);


				model._buttonsContainer.append(userContainer.view.container);
			}
		};
	};

	return topMenuModel;
});