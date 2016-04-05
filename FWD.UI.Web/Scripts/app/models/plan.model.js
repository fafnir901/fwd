define([
	'underscore',
	'app/helpers/configurator.static.helper',
	'app/helpers/notification.static.helper',
	'app/services/plan.service'
], function (_, conf, errorHelper, PlanService) {
	var planModel = function () {
		this.model =
		{
			service: new PlanService(),
			items: [],
			totalCount: 0,
			pagerScope : { skip: 0, take: conf.pagerScope.take, order: null }
		};

		var getModel = function () {
			return this.model;
		}.bind(this);

		var getService = function () {
			return this.model.service;
		}.bind(this);

		return {
			init: function () {
				
			},
			getPlans: function (pagerScope) {
				pagerScope = pagerScope || getModel().pagerScope;
				return getModel().service.getAllPlans(new Date(), $('.common'), pagerScope).fail(function (data) {
					errorHelper.showError(data.responseText);
				}).success(function (data) {
					getModel().totalCount = data.TotalCount;
					_.each(data.Plans, function (item) {
						getModel().items.push(item);
					});
				});
			},
			addPlan: function (planDto, rootElement) {
				return getModel().service.savePlan(planDto, rootElement);
			},
			getModel: function (parameters) {
				return getModel();
			},
			getService: function () {
				return getService();
			},
			getPlansForward: function (pagerScope) {
				return this.getPlans(pagerScope);
			},
			updatePlan: function (date, rootElement, dto) {
				getModel().service.updatePlan(date, rootElement, dto).fail(function (data) {
					errorHelper.showError(data.responseText);
				}).success(function (data) {
					var message = 'План с номером ' + data.Id + ' успешно обновлен';
					errorHelper.showInfo(message);
				});
			},
			deletePlan: function (id, rootElement) {
				return getModel().service.deletePlan(id, rootElement);
			}
		};
	};
	return planModel;
});