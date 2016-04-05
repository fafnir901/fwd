define([
	'jquery',
	'underscore',
	'app/helpers/configurator.static.helper',
	'app/helpers/notification.static.helper',
	'app/views/pager.view',
	'app/models/transaction.model'
], function ($, _, conf, errorHelper, PagerView, TransactionModel) {
	var transactionView = function () {
		this.view = {
			pagerView: new PagerView()
		};
		this.view.colorCoding = [
			{ key: 1, color: "linear-gradient(to bottom, #0C7B0E, #0BFD33)" },
			{ key: 2, color: "linear-gradient(rgb(186, 163, 6), rgb(39, 39, 36))" }, //"linear-gradient(to bottom, #262723, #FCF00B)"},
			{ key: 3, color: "linear-gradient(rgb(71, 52, 52), rgb(244, 0, 0))" }
		];

		var getView = function () {
			return this.view;
		}.bind(this);

		var preInit = function () {
			var view = getView();
			view.panelContainer = $('<div class="panel_container"></div>');
			view.panelItemClass = 'panel_item';
			return view;
		};

		var fillRow = function (item, model, view, $planTable) {
			var $tranItem = $('<tr draggable></tr>');
			var $tranId = $('<td class="entity_id_cell"></td>');
			var $tranDescription = $('<td class="entity_decr_cell"></td>');
			var $tranDate = $('<td class="entity_date_cell"></td>');
			var $tranViewParam = $('<td class="entity_view_params"></td>');
			var $tranViewParamBtn = $('<button class="entity_view_params_btn" item-id="' + item.Id + '">Параметры</button>');
			$tranViewParam.append($tranViewParamBtn);
			$tranItem.addClass(view.panelItemClass);

			$tranId.text(item.Id);

			var $tranHead = $('<td></td>');
			$tranHead.append($tranId);

			var color = _.filter(view.colorCoding, function (coding) {
				return coding.key == item.ActionTypeInt;
			});

			if (color[0] != null) {
				$tranId.css("background-image", color[0].color);
			}

			$tranItem.append($tranHead);

			$tranDescription.text(item.Description);
			$tranItem.append($tranDescription);

			var date = new Date(parseInt(item.ActionDateTime.replace(/\/Date\((-?\d+)\)\//, '$1')));
			$tranDate.text(date.toLocaleString());
			$tranItem.append($tranDate);

			$planTable.append($tranItem);
			if (item.Parameters != null) {
				$tranViewParamBtn.off('click');
				$tranViewParamBtn.on('click', function () {
					var itemId = $(this).attr('item-id');

					var table = $('.info_detail_table_container[item-id=' + itemId + ']');
					if (table.css('display') == undefined || table.css('display') == 'none') {
						table.slideDown(200);
					} else {
						table.slideUp(200);
					}
				});
				$tranItem.append($tranViewParam);

				var $paramRow = $('<tr class="panel_item"></tr>');
				var paramContainer = $('<div class="info_detail_table_container" item-id="' + item.Id + '"></div>');
				var paramTable = $('<table class="info_detail_table" item-id="' + item.Id + '"></table>');
				var paramHeader = $('<thead></thead>');
				var paramHeaderRow = $('<tr colspan="2"></tr>');
				var key = $('<th>Параметр</th>');
				var value = $('<th>Значение</th>');
				paramHeaderRow.append(key);
				paramHeaderRow.append(value);
				paramHeader.append(paramHeaderRow);
				paramTable.append(paramHeader);

				if (item.Parameters.length > 0) {
					var json = JSON.parse(item.Parameters);

					_.each(json, function (val, k) {
						var tr = $('<tr></tr>');
						var td1 = $('<td></td>');
						var td2 = $('<td></td>');
						td1.append(k);
						td2.append(val);
						tr.append(td1);
						tr.append(td2);
						paramTable.append(tr);
					});
					paramContainer.append(paramTable);
					$paramRow.append(paramContainer);
					$planTable.append($paramRow);
				}
			}
		};

		var fillContainer = function (data, view, $planTable, model) {
			_.each(data, function (item) {
				fillRow(item, model, view, $planTable);
			});
		};

		var regOrder = function (selector, property, view, model, $planTable) {
			view.panelContainer.find(selector).on('click', function () {
				model.currentData = _.sortBy(model.currentData, function (item) {
					switch (property) {
						case 'Id':
							return item.Id;
						case 'Description':
							return item.Description;
						case 'ActionDateTime':
							return item.ActionDateTime;
					}
				});
				$(view.panelContainer.children().children()[1]).remove();
				fillContainer(model.currentData, view, $planTable, model);
			});
		};

		var initPlanTable = function (model, view) {
			var $planTable = $('<table class="info_table"></table>');
			var th1 = $('<th id="name_order">ID</th>');
			var idSearch = $('<input type="text" class="tran_search_string id"/>');

			var th2 = $('<th id="descr_order">Описание</th>');
			var descSearch = $('<input type="text" class="tran_search_string description"/>');

			var th3 = $('<th id="add_order">Добавлен</th>');
			var th4 = $('<th>Действие</th>');

			var thead = $('<thead></thead>');

			th1.append(idSearch);
			th2.append(descSearch);

			thead.append(th1);
			thead.append(th2);
			thead.append(th3);
			thead.append(th4);

			$planTable.append(thead);


			var timeout = {};
			var getTrans = function (parameter, str, currentModel) {
				timeout = setTimeout(function () {
					model.getTransBySearch(currentModel.pager.getModel(), parameter, str).then(function (items) {
						currentModel = view.pagerView.init(model);
						var totalCount = Math.round(items.TotalCount / conf.pagerScope.take);
						model.totalCount = totalCount;
						model.currentData = items.Trans;
						if (items.Trans.length != 0) {
							$(view.panelContainer.children().children()[1]).remove();
							fillContainer(model.currentData, view, $planTable, model);
							currentModel.currentPage.index = 0;
							var displayIndex = currentModel.currentPage.index + 1;
							$('#' + currentModel.idsOfControls[3]).text(displayIndex + ' of ' + totalCount);
						} else {
							var current = $(view.panelContainer.children().children()[1]);
							var parent = current.parent();
							current.remove();
							if (currentModel.pager.getModel().skip >= conf.pagerScope.skip)
								currentModel.pager.getModel().skip -= conf.pagerScope.skip;
							$('#' + currentModel.idsOfControls[3]).text((currentModel.currentPage.index) + ' of ' + totalCount);
							parent.append('<div></div>');
						}
						view.pagerView.init(model);
					});
				}, 500);
			};

			descSearch.off('keyup');
			descSearch.on('keyup', function (e) {

				clearTimeout(timeout);
				var str = $(this).val();
				var parameter = "Description";
				var currentModel = view.pagerView.init(model);
				getTrans(parameter, str, currentModel);
			});

			idSearch.off('keyup');
			idSearch.on('keyup', function (e) {

				clearTimeout(timeout);
				var str = $(this).val();
				var parameter = "Id";
				var currentModel = view.pagerView.init(model);
				getTrans(parameter, str, currentModel);
			});

			return $planTable;
		};

		var render = function (model, view, fnc) {
			return fnc().then(function (data) {

				model.totalCount = Math.round(data.TotalCount / conf.pagerScope.take);
				model.currentData = data.Trans;
				var $planTable = initPlanTable(model, view);

				fillContainer(data.Trans, view, $planTable, model);
				view.panelContainer.append($planTable);

				regOrder('#name_order', "Id", view, model, $planTable);
				regOrder('#descr_order', "Description", view, model, $planTable);
				regOrder('#add_order', "ActionDateTime", view, model, $planTable);

				var currentModel = view.pagerView.init(model);
				$planTable.after(currentModel.content);
				currentModel.allPages = model.totalCount;
				currentModel.content.find('#' + currentModel.idsOfControls[4]).on('click', function () {
					currentModel.pager.getModel().skip = 0;
					model.getTransForward(currentModel.pager.getModel()).then(function (items) {
						if (items.Trans.length != 0) {
							model.currentData = items.Trans;
							$(view.panelContainer.children().children()[1]).remove();
							fillContainer(model.currentData, view, $planTable, model);
							currentModel.currentPage.index = 0;
							var displayIndex = currentModel.currentPage.index + 1;
							$('#' + currentModel.idsOfControls[3]).text(displayIndex + ' of ' + currentModel.allPages);
						} else {
							if (currentModel.pager.getModel().skip >= conf.pagerScope.skip)
								currentModel.pager.getModel().skip -= conf.pagerScope.skip;
							$('#' + currentModel.idsOfControls[3]).text((currentModel.currentPage.index) + ' of ' + currentModel.allPages);
						}
					});
				});

				currentModel.content.find('#' + currentModel.idsOfControls[5]).on('click', function () {
					currentModel.pager.getModel().skip = currentModel.allPages * conf.pagerScope.skip;
					if (data.TotalCount < currentModel.pager.getModel().skip) currentModel.pager.getModel().skip = data.TotalCount - currentModel.pager.getModel().take;
					model.getTransForward(currentModel.pager.getModel()).then(function (items) {
						if (items.Trans.length != 0) {
							model.currentData = items.Trans;
							$(view.panelContainer.children().children()[1]).remove();
							fillContainer(model.currentData, view, $planTable, model);

							var displayIndex = currentModel.allPages;

							currentModel.currentPage.index = displayIndex;
							$('#' + currentModel.idsOfControls[3]).text(displayIndex + ' of ' + currentModel.allPages);
						} else {
							if (currentModel.pager.getModel().skip >= conf.pagerScope.skip)
								currentModel.pager.getModel().skip -= conf.pagerScope.skip;
							$('#' + currentModel.idsOfControls[3]).text((currentModel.currentPage.index + 1) + ' of ' + currentModel.allPages);
						}
					});
				});

				currentModel.content.find('#' + currentModel.idsOfControls[1]).on('click', function () {
					currentModel.pager.getModel().skip += conf.pagerScope.skip;
					model.getTransForward(currentModel.pager.getModel()).then(function (items) {
						if (items.Trans.length != 0) {
							model.currentData = items.Trans;
							$(view.panelContainer.children().children()[1]).remove();
							fillContainer(model.currentData, view, $planTable, model);
							if (data.TotalCount >= currentModel.pager.getModel().skip)
								currentModel.currentPage.index++;

							var displayIndex = currentModel.allPages > currentModel.currentPage.index
								? currentModel.currentPage.index + 1
								: currentModel.currentPage.index;

							$('#' + currentModel.idsOfControls[3]).text(displayIndex + ' of ' + currentModel.allPages);
						} else {
							if (currentModel.pager.getModel().skip >= conf.pagerScope.skip)
								currentModel.pager.getModel().skip -= conf.pagerScope.skip;

							$('#' + currentModel.idsOfControls[3]).text((currentModel.currentPage.index) + ' of ' + currentModel.allPages);
						}
					});
				});

				currentModel.content.find('#' + currentModel.idsOfControls[2]).on('click', function () {
					if (currentModel.pager.getModel().skip >= conf.pagerScope.skip)
						currentModel.pager.getModel().skip -= conf.pagerScope.skip;
					if (currentModel.currentPage.index > 0) {
						model.getTransForward(currentModel.pager.getModel()).then(function (items) {
							if (items.Trans.length != 0) {
								model.currentData = items.Trans;
								$(view.panelContainer.children().children()[1]).remove();
								fillContainer(model.currentData, view, $planTable, model);
								if (currentModel.currentPage.index > 0)
									currentModel.currentPage.index--;

								var displayIndex = currentModel.allPages == currentModel.currentPage.index + 1
									? currentModel.currentPage.index
									: currentModel.currentPage.index + 1;

								$('#' + currentModel.idsOfControls[3]).text(displayIndex + ' of ' + currentModel.allPages);
							}
						});
					}
				});
			});
		};

		var reRender = function (model, view) {
			var $planTable = $('<table class="info_table"></table>');
			$planTable.append($('<thead><tr colspan="2"><th id="name_order" click="orderByName">ID<input type="text" class="tran_search_string id"/></th><th id="descr_order">Описание<input type="text" class="tran_search_string description"/></th><th id="add_order">Добавлен</th><th>Действие</th></tr></thead>'));
			fillContainer(model.currentData, view, $planTable, model);
			if (view.panelContainer.children().length > 0) {
				$(view.panelContainer.children()[0]).before($planTable);
			} else {
				view.panelContainer.append($planTable);
			}
		};

		return {
			render: function () {
				var model = new TransactionModel();
				return render(model, preInit(), model.getTrans);
			},
			getView: function () {
				return getView();
			},
			reRender: function (model, view) {
				var model = new TransactionModel();
				reRender(model, view);
			}
		};
	};
	return transactionView;
});