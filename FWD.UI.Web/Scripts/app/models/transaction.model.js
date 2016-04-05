define([
	'jquery',
	'underscore',
	'app/helpers/configurator.static.helper',
	'app/helpers/notification.static.helper',
	'app/services/transaction.service'
], function ($, _, conf, errorHelper, TransactionService) {
	var transactionModel = function () {

		this.model = {
			currentData: [],
			totalCount: 0,
			service: new TransactionService(),
			pagerScope: { skip: 0, take: conf.pagerScope.take, order: null },
			loaderArea: $('.common')
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

			getTrans: function (pagerScope) {
				var model = getModel();
				pagerScope = pagerScope || model.pagerScope;
				return model.service.getAllTrans(new Date(), model.loaderArea, pagerScope)
					.fail(function (data) {
						errorHelper.showError(data.responseText);
					}).success(function (data) {
						model.totalCount = data.TotalCount;
						_.each(data.Trans, function (item) {
							model.currentData.push(item);
						});
					});
			},

			getTransBySearch: function (pagerScope, parameter, value) {
				var model = getModel();
				pagerScope = pagerScope || model.pagerScope;
				return model.service.getTransBySearh(model.loaderArea, pagerScope, parameter, value)
					.fail(function (data) {
						errorHelper.showError(data.responseText);
					}).success(function (data) {
						model.totalCount = data.TotalCount;
						_.each(data.Trans, function (item) {
							model.currentData.push(item);
						});
					});
			},
			getModel: function (parameters) {
				return getModel();
			},
			getService: function () {
				return getService();
			},
			getTransForward: function (pagerScope) {
				return this.getTrans(pagerScope);
			},
		};
	};
	return transactionModel;
});