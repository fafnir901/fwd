define([
	'jquery',
	'underscore',
	'app/helpers/notification.static.helper',
	'app/helpers/configurator.static.helper',
	'app/models/plan.model',
	'app/views/pager.view'
], function ($, _, errorHelper, conf, PlanModel, PagerView) {
	var planView = function () {
		this.view = {};

		var getView = function () {
			return this.view;
		}.bind(this);

		var preInit = function (model) {

			var view = getView();
			view.panelContainer = $('<div class="panel_container"></div>');
			view.panelItemClass = 'panel_item';
			view.adderContainer = {};
			view.adderContainer.commonContainer = $('<div class="plan_adder_common"></div>');
			view.adderContainer.container = $('<div class="plan_adder"></div>');
			view.adderContainer.showContainerButton = $('<button id="showPlanAdder" title="Добавить"></button>');
			view.adderContainer.planName = $('<input id="adder_plan_name" type="text" placeholder="Название"></input>');
			view.adderContainer.description = $('<input id="adder_description" type="text" placeholder="Описание"></input>');
			view.adderContainer.addBtn = $('<button id="add_plan">Добавить</button>');

			view.adderContainer.container.append(view.adderContainer.planName);
			view.adderContainer.container.append(view.adderContainer.description);
			view.adderContainer.container.append(view.adderContainer.addBtn);

			view.adderContainer.commonContainer.append(view.adderContainer.showContainerButton);
			view.adderContainer.commonContainer.append(view.adderContainer.container);
			view.adderContainer.showContainerButton.off('click');
			view.adderContainer.showContainerButton.on('click', function () {
				var that = $(this);
				var currentText = that.attr('id');
				if (currentText == 'showPlanAdder') {
					that.attr('id', 'hidePlanAdder');
					view.adderContainer.container.css({ 'display': 'block', 'opacity': '0' });
					view.adderContainer.container.animate({
						opacity: 1
					}, 300);
				} else {
					that.attr('id', 'showPlanAdder');
					view.adderContainer.container.animate({
						opacity: 0,
					}, 300, function () {
						view.adderContainer.container.css({ 'display': 'none' });
					});

				}
			});
			view.adderContainer.addBtn.off('click');
			view.adderContainer.addBtn.on('click', function () {
				var that = $(this);
				var dto = validate();
				if (dto != undefined) {
					model.addPlan(dto, view.adderContainer.container).success(function (data) {
						errorHelper.showInfo('Запись успешно добавлена');
						$('#adder_plan_name').val('');
						$('#adder_description').val('');
						if (model.currentData.length < 10) {
							var $planTable = $('.info_table');
							fillRow(data, model, view, $planTable);
						}
					}).fail(function (data) {
						errorHelper.showError(data.responseText);
					});
				}
			});
			return view;
		};

		var validate = function () {
			var errors = '';
			var $planName = $('#adder_plan_name');
			var $descr = $('#adder_description');
			var errorClass = 'error_element';
			$planName.removeClass(errorClass);
			$descr.removeClass(errorClass);
			var dto = {
				name: $planName.val(),
				description: $descr.val(),
			};
			if (dto.name == undefined || dto.name.length == 0) {
				errors += 'Поле "Название" должно быть заполнено. ';
				$planName.addClass(errorClass);
			}
			else if (dto.name.length < 3) {
				errors += 'Поле "Название" должно сожержать не менее 3 символов. ';
				$planName.addClass(errorClass);
			}

			if (dto.description == undefined || dto.description.length == 0) {
				errors += 'Поле "Описание" должно быть заполнено. ';
				$descr.addClass(errorClass);
			}
			else if (dto.description.length < 3) {
				errors += 'Поле "Описание" должно сожержать не менее 3 символов. ';
				$descr.addClass(errorClass);
			}
			if (errors.length > 0) {
				errorHelper.showError(errors);
				return undefined;
			} else {
				return dto;
			}
		};

		var updatePlan = function ($planItem, isDone, rootElement, model) {
			var dto = {
				id: $planItem.find('.entity_id_cell').text(),
				name: $planItem.find('.entity_name_cell').text(),
				description: $planItem.find('.entity_decr_cell').text(),
				addedDate: $planItem.find('.entity_date_cell').text(),
				isDone: isDone
			};
			var date = new Date();
			model.updatePlan(date, rootElement, dto);
		};

		var render = function (model, view) {
			return model.getPlans().then(function (data) {
				model.totalCount = Math.round(data.TotalCount / 10);
				model.currentData = data.Plans;
				var $planTable = $('<table class="info_table"></table>');
				$planTable.append($('<thead><tr colspan="2"><th id="name_order">Название</th><th id="descr_order">Описание</th><th id="add_order">Добавлен</th><th id="is_done_order">Выполнен</th></tr></thead>'));

				fillContainer(data.Plans, view, $planTable, model);
				view.panelContainer.append($planTable);
				regOrder('#name_order', "PlanName", view, model, $planTable);
				regOrder('#descr_order', "Description", view, model, $planTable);
				regOrder('#add_order', "AddedDate", view, model, $planTable);
				regOrder('#is_done_order', "IsDone", view, model, $planTable);

				var pagerView = new PagerView();
				var currentModel = pagerView.init(model);
				$planTable.after(currentModel.content);
				currentModel.content.after(view.adderContainer.commonContainer);
				currentModel.allPages = model.totalCount + 1;
				currentModel.content.find('#' + currentModel.idsOfControls[1]).on('click', function () {
					currentModel.pager.getModel().skip += conf.pagerScope.skip;
					model.getPlansForward(currentModel.pager.getModel()).then(function (items) {
						if (items.Plans.length != 0) {
							model.currentData = items.Plans;
							$(view.panelContainer.children().children()[1]).remove();
							fillContainer(model.currentData, view, $planTable, model);
							currentModel.currentPage.index++;
							var displayIndex = currentModel.currentPage.index + 1;
							if (currentModel.allPages <= displayIndex + 1) {
								currentModel.allPages = displayIndex + 1;
							}
							if (model.currentData.length < conf.pagerScope.skip) {
								currentModel.allPages = displayIndex;
							}
							currentModel.content.find('#' + currentModel.idsOfControls[3]).text(displayIndex + ' of ' + currentModel.allPages);
						} else {
							if (currentModel.pager.getModel().skip >= conf.pagerScope.skip)
								currentModel.pager.getModel().skip -= conf.pagerScope.skip;
							currentModel.allPages = currentModel.currentPage.index + 1;
							currentModel.content.find('#' + currentModel.idsOfControls[3]).text((currentModel.currentPage.index + 1) + ' of ' + currentModel.allPages);
						}
					});
				});

				currentModel.content.find('#' + currentModel.idsOfControls[2]).on('click', function () {
					if (currentModel.pager.getModel().skip >= conf.pagerScope.skip)
						currentModel.pager.getModel().skip -= conf.pagerScope.skip;
					if (currentModel.currentPage.index > 0) {
						model.getPlansForward(currentModel.pager.getModel()).then(function (items) {
							if (items.Plans.length != 0) {
								model.currentData = items.Plans;
								$(view.panelContainer.children().children()[1]).remove();
								fillContainer(model.currentData, view, $planTable, model);
								if (currentModel.currentPage.index > 0)
									currentModel.currentPage.index--;
								var displayIndex = currentModel.currentPage.index + 1;
								currentModel.content.find('#' + currentModel.idsOfControls[3]).text(displayIndex + ' of ' + currentModel.allPages);
							}
						});
					}
				});
			});
		};

		var regOrder = function (selector, property, view, model, $planTable) {
			view.panelContainer.find(selector).on('click', function () {
				model.currentData = _.sortBy(model.currentData, function (item) {
					switch (property) {
						case 'Name':
							return item.PlanName;
						case 'Description':
							return item.Description;
						case 'AddedDate':
							return item.AddedDate;
						case 'IsDone':
							return item.IsDone;
						default:
							return item.planId;
					}

				});
				$(view.panelContainer.children().children()[1]).remove();
				fillContainer(model.currentData, view, $planTable, model);
			});
		};

		var fillRow = function (item, model, view, $planTable) {
			var $planItem = $('<tr draggable></tr>');
			var $planName = $('<td class="entity_name_cell"></td>');
			var $planId = $('<td class="entity_id_cell"></td>');
			var $planDescr = $('<td class="entity_decr_cell"></td>');
			var $planDate = $('<td class="entity_date_cell"></td>');
			var $planIsDone = $('<td></td>');
			var checkClass = 'plan_unchecked';
			var $checkBox = $('<input type="checkbox"/>');
			var $deletePlan = $('<button title="Удалить" class="delete_plan" delete-id="' + item.Id + '"/>');
			$checkBox.off('click');
			$checkBox.on('click', function () {
				var that = $(this);
				var isUnChecked = that.attr('checked') != undefined;
				if (isUnChecked) {
					$planItem.addClass(checkClass);
					that.removeAttr('checked');
					updatePlan($planItem, false, view.panelContainer, model);
				} else {
					$planItem.removeClass(checkClass);
					that.attr('checked', 'checked');
					updatePlan($planItem, true, view.panelContainer, model);
				}

			});

			$deletePlan.one('click', function () {
				var that = $(this);
				var id = that.attr('delete-id');
				model.deletePlan(id, $planTable).fail(function (currentData) {
					errorHelper.showError(currentData.responseText);
				}).success(function (currentData) {
					that.parent().parent().remove();
					errorHelper.showInfo('План успешно удален');
				});

			});

			$planItem.addClass(view.panelItemClass);


			$planId.text(item.Id);
			$planName.text(item.Name);

			var $planHead = $('<td></td>');
			$planHead.append($planId);
			$planHead.append($planName);

			$planItem.append($planHead);

			$planDescr.text(item.Description);
			$planItem.append($planDescr);

			var date = new Date(parseInt(item.AddedDate.replace(/\/Date\((-?\d+)\)\//, '$1')));
			$planDate.text(date.toLocaleString());
			$planItem.append($planDate);

			if (!item.IsDone) {
				$planItem.addClass(checkClass);
			} else {
				$checkBox.attr('checked', 'checked');
			}

			$planIsDone.append($checkBox);
			$planIsDone.append($deletePlan);
			$planItem.append($planIsDone);


			$planTable.append($planItem);

		};

		var fillContainer = function (data, view, $planTable, model) {
			_.each(data, function (item) {
				fillRow(item, model, view, $planTable);
			});
		};

		var reRender = function (model, view) {
			var $planTable = $('<table class="info_table"></table>');
			$planTable.append($('<thead><tr colspan="2"><th id="name_order" click="orderByName">Название</th><th id="descr_order">Описание</th><th id="add_order">Добавлен</th><th id="is_done_order">Выполнен</th></tr></thead>'));
			fillContainer(model.currentData, view, $planTable, model);
			if (view.panelContainer.children().length > 0) {
				$(view.panelContainer.children()[0]).before($planTable);
			} else {
				view.panelContainer.append($planTable);
			}

		};

		return {
			render: function () {
				var model = new PlanModel();
				return render(model, preInit(model));
			},
			getView: function () {
				return getView();
			},
			reRender: function (view) {
				var model = new PlanModel();
				reRender(model, view);
			}
		};
	};

	return planView;
});