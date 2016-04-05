define([
	'jquery',
	'underscore',
	'app/models/search.model',
	'app/helpers/configurator.static.helper',
	'app/helpers/notification.static.helper',
	'app/views/tag.view'
], function ($, _, SearchModel, conf, errorHelper, TagView) {
	var searchView = function () {

		var view = {
			model: new SearchModel(),
			tagModel: {},
			tagView: new TagView(),
			foundResults: {},
			itemTemplate: {
				itemHeader: {
					link: {},
					author: {},
					articleName: {}
				},
				itemMain: {}
			},
			noDataItemTemplate: '<div class="search_no_data"></div>',
			container: {
				searchContainer: {},
				searchContainerHeader: {
					searchInputContainer: {
						searchInput: {},
					},
					searchButton: {},
					totalCount: {},
					tagToggler: {
						toggler: {},
						lable: {}
					}
				},
				searchTagContainerHeader: {
					searchInputContainer: {
						searchInput: {
							tagChooser: {}
						},
					},
					searchButton: {},
					totalCount: {},
					tagToggler: {
						toggler: {},
						lable: {}
					}
				}
			}
		};


		var appendTags = function (parent, currentView) {
			if (conf.featureList.Tag) {

				view.tagView.init(parent).success(function (src) {
					parent.append(view.tagView.renderForAdd(currentView));
					view.tagModel = view.tagView.getModel();
				}).fail(function () {
					parent.append(view.tagView.renderForAdd(currentView));
				});
			} else {
				parent.append($('<div class="tag_content"></div>'));
			}
		};

		var preInitHeader = function () {
			view.container = $('<div class="search_container"></div>');

			view.container.searchContainerHeader = $('<div class="search_container_header"></div>');
			view.container.searchTagContainerHeader = $('<div class="search_tag_container_header search_container_invisible"></div>');

			view.container.searchContainerHeader.searchInputContainer = $('<div class="search_input_container"></div>');
			view.container.searchContainerHeader.searchInputContainer.searchInput = $('<input type="text" class="search_searcher" placeholder="Введите строку поиска"/>');
			view.container.searchContainerHeader.totalCount = $("<div class='search_container_header_total_count'></div>");

			view.container.searchTagContainerHeader.searchInputContainer = $('<div class="search_input_tag_container"></div>');
			view.container.searchTagContainerHeader.searchInputContainer.tagChooser = $('<div class="tag_chooser"></div>');
			view.container.searchTagContainerHeader.searchInputContainer.searchInput = $('<div class="search_searcher_tag" placeholder="Введите строку поиска"></div>');
			view.container.searchTagContainerHeader.totalCount = $("<div class='search_tag_container_header_total_count'></div>");

			appendTags(view.container.searchTagContainerHeader.searchInputContainer.tagChooser, view.container.searchTagContainerHeader.searchInputContainer.searchInput);

			view.container.searchContainerHeader.searchButton = $('<button id="make_search">Поиск</button>');
			view.container.searchTagContainerHeader.searchButton = $('<button id="make_search_by_tag">Поиск</button>');

			view.container.searchContainerHeader.tagToggler = $('<div class="tag_toggler"></div>');
			view.container.searchContainerHeader.tagToggler.toggler = $('<input type="checkbox" id="toggle_tag"/>');
			view.container.searchContainerHeader.tagToggler.label = $('<span>Поиск по тегам:</span>');
			view.container.searchContainerHeader.tagToggler.append(view.container.searchContainerHeader.tagToggler.label);
			view.container.searchContainerHeader.tagToggler.append(view.container.searchContainerHeader.tagToggler.toggler);

			view.container.searchTagContainerHeader.tagToggler = $('<div class="tag_toggler_tag"></div>');
			view.container.searchTagContainerHeader.tagToggler.toggler = $('<input type="checkbox" id="toggle_tag_tag" checked="checked"/>');
			view.container.searchTagContainerHeader.tagToggler.label = $('<span>Поиск по тегам:</span>');
			view.container.searchTagContainerHeader.tagToggler.append(view.container.searchTagContainerHeader.tagToggler.label);
			view.container.searchTagContainerHeader.tagToggler.append(view.container.searchTagContainerHeader.tagToggler.toggler);

			view.container.searchContainerHeader.searchInputContainer.append(view.container.searchContainerHeader.searchInputContainer.searchInput);
			view.container.searchTagContainerHeader.searchInputContainer.append(view.container.searchTagContainerHeader.searchInputContainer.searchInput);
			view.container.searchTagContainerHeader.searchInputContainer.append(view.container.searchTagContainerHeader.searchInputContainer.tagChooser);


			view.container.searchContainerHeader.append(view.container.searchContainerHeader.searchInputContainer);
			view.container.searchContainerHeader.append(view.container.searchContainerHeader.searchButton);
			view.container.searchContainerHeader.append(view.container.searchContainerHeader.totalCount);
			view.container.searchContainerHeader.append(view.container.searchContainerHeader.tagToggler);

			view.container.searchTagContainerHeader.append(view.container.searchTagContainerHeader.searchInputContainer);
			view.container.searchTagContainerHeader.append(view.container.searchTagContainerHeader.searchButton);
			view.container.searchTagContainerHeader.append(view.container.searchTagContainerHeader.totalCount);
			view.container.searchTagContainerHeader.append(view.container.searchTagContainerHeader.tagToggler);

			view.container.append(view.container.searchContainerHeader);
			view.container.append(view.container.searchTagContainerHeader);

			view.container.searchContainerHeader.searchButton.off('click');

			var initialWidth = view.container.searchContainerHeader.searchInputContainer.searchInput.innerWidth();

			view.container.searchContainerHeader.searchInputContainer.searchInput.focus(function (e) {
				initialWidth = view.container.searchContainerHeader.searchInputContainer.searchInput.innerWidth();
				e.preventDefault();
				view.container.searchContainerHeader.searchInputContainer.stop().animate({ "width": "50%" }, 500);
			});

			view.container.searchContainerHeader.searchInputContainer.searchInput.blur(function (e) {
				e.preventDefault();
				view.container.searchContainerHeader.searchInputContainer.stop().animate({ "width": initialWidth }, 500);
			});

			view.container.searchContainerHeader.searchButton.on('click', function () {
				if (validate()) {
					if (view.container.searchContainer.children().length > 0)
						view.container.searchContainer.children().remove();
					view.model.getResults(view.container.searchContainerHeader.searchInputContainer.searchInput.val(), view.container).success(function (res) {
						view.container.searchContainerHeader.totalCount.text("Результаты: " + res.length);
						_.each(res, function (item) {
							var cloned = preInitItemTemplate();

							var tagContainer = $('<div class="search_tag_container"></div>');
							view.tagView.generateTags(item.Tags, tagContainer);

							cloned.itemHeader.link.attr('href', item.GeneratedLink);
							cloned.itemHeader.author.text(item.Author == null ? '' : item.Author);
							cloned.itemHeader.author.append(tagContainer);
							cloned.itemHeader.author.attr(item.Author == null ? '' : item.Author);
							cloned.itemHeader.articleName.text(item.ArtilceName);
							cloned.itemHeader.articleName.attr('title', item.ArtilceName);

							var replacedText = item.PartOfArticleText.replace(new RegExp(view.container.searchContainerHeader.searchInputContainer.searchInput.val(), 'ig'),
								'<span style="background-color:rgba(255, 255, 0, 0.55);">' + view.container.searchContainerHeader.searchInputContainer.searchInput.val() + '</span>');

							cloned.itemMain.html(replacedText);
							view.container.searchContainer.append(cloned);
						});
					}).error(function (resp) {
						errorHelper.showError(resp.responseText);
						var item = $(view.noDataItemTemplate);
						item.append("Нету данных");
						view.container.searchContainerHeader.totalCount.text("Результаты: " + 0);
						view.container.searchContainer.append(item);
					});
				} else {
					applyErrorStyle();
				}
			});

			view.container.searchContainerHeader.tagToggler.toggler.off('click');
			view.container.searchContainerHeader.tagToggler.toggler.on('click', function (e) {
				view.container.searchContainerHeader.addClass('search_container_invisible');
				view.container.searchTagContainerHeader.removeClass('search_container_invisible');
				$(this).prop('checked', false);
			});

			view.container.searchTagContainerHeader.tagToggler.toggler.off('click');
			view.container.searchTagContainerHeader.tagToggler.toggler.on('click', function (e) {
				view.container.searchTagContainerHeader.addClass('search_container_invisible');
				view.container.searchContainerHeader.removeClass('search_container_invisible');
				$(this).prop('checked', true);
			});

			view.container.searchTagContainerHeader.searchButton.off('click');
			view.container.searchTagContainerHeader.searchButton.on('click', function () {
				if (validateTag()) {
					if (view.container.searchContainer.children().length > 0)
						view.container.searchContainer.children().remove();

					view.model.getResultsForTags(view.tagModel.tags, view.container).success(function (res) {
						view.container.searchTagContainerHeader.totalCount.text("Результаты: " + res.length);
						_.each(res, function (item) {
							var cloned = preInitItemTemplate();

							var tagContainer = $('<div class="search_tag_container"></div>');
							view.tagView.generateTags(item.Tags, tagContainer);

							cloned.itemHeader.link.attr('href', item.GeneratedLink);
							cloned.itemHeader.author.text(item.Author == null ? '' : item.Author);
							cloned.itemHeader.author.append(tagContainer);
							cloned.itemHeader.author.attr(item.Author == null ? '' : item.Author);
							cloned.itemHeader.articleName.text(item.ArtilceName);
							cloned.itemHeader.articleName.attr('title', item.ArtilceName);

							cloned.itemMain.html(item.PartOfArticleText);
							view.container.searchContainer.append(cloned);
						});
					}).error(function (resp) {
						errorHelper.showError(resp.responseText);
						var item = $(view.noDataItemTemplate);
						item.append("Нету данных");
						view.container.searchContainerHeader.totalCount.text("Результаты: " + 0);
						view.container.searchContainer.append(item);
					});
				} else {
					applyErrorStyleForTags();
				}
			});
		};

		var preInitMainContainer = function () {
			view.container.searchContainer = $('<div class="search_search-container"></div>');
			view.container.append(view.container.searchContainer);
		};

		var preInitItemTemplate = function () {
			view.itemTemplate = $('<div class="search_data_container"></div>');
			view.itemTemplate.itemHeader = $('<div class="search_data_container_header"></div>');
			view.itemTemplate.itemHeader.link = $('<button href="">Перейти</button>');
			view.itemTemplate.itemHeader.author = $('<div class="search_data_container_header_author"></div>');
			view.itemTemplate.itemHeader.articleName = $('<div class="search_data_container_header_articleName"></div>');

			view.itemTemplate.itemHeader.append(view.itemTemplate.itemHeader.link);
			view.itemTemplate.itemHeader.append(view.itemTemplate.itemHeader.author);
			view.itemTemplate.itemHeader.append(view.itemTemplate.itemHeader.articleName);

			view.itemTemplate.itemMain = $('<div class="search_data_container_item_text"></div>');

			view.itemTemplate.append(view.itemTemplate.itemHeader);
			view.itemTemplate.append(view.itemTemplate.itemMain);

			view.itemTemplate.itemHeader.link.off('click');
			view.itemTemplate.itemHeader.link.on('click', function () {
				var href = $(this).attr('href');
				window.open(href);
			});
			return view.itemTemplate;
		};

		var validate = function () {
			view.container.searchContainerHeader.searchInputContainer.searchInput.removeClass('error_element');
			return view.container.searchContainerHeader.searchInputContainer.searchInput.val().length != 0;
		};

		var validateTag = function () {
			view.container.searchTagContainerHeader.searchInputContainer.searchInput.removeClass('error_element');
			return view.tagModel.tags.length != 0;
		};

		var applyErrorStyle = function () {
			errorHelper.showError("Вы ничего не ввели в строку поиска");
			view.container.searchContainerHeader.searchInputContainer.searchInput.addClass('error_element');
		};

		var applyErrorStyleForTags = function () {
			errorHelper.showError("Вы не выбрали тэгов для посика");
			view.container.searchTagContainerHeader.searchInputContainer.searchInput.addClass('error_element');
		};

		var init = function () {
			preInitHeader();
			preInitMainContainer();
			preInitItemTemplate();
		};

		return {
			getViewModel: function () {
				return view;
			},
			render: function () {
				init();
				return view.container;
			}
		};
	};
	return searchView;
});