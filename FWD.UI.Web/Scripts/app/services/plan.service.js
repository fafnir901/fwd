define([
	'jquery',
	'underscore',
	'app/helpers/appPath.static.helper',
	'app/helpers/configurator.static.helper'
], function ($,_, appPath, conf) {
	var planService = function() {

		this.pager = _.clone(conf.pager);

		return {
			getAllPlans: function(dateTime, rootElement, pagerScope) {
				if (pagerScope != undefined) {
					this.pager.setSkip(pagerScope.skip);
					this.pager.setTake(pagerScope.take);
					this.pager.setOrder(pagerScope.order);
				}
				return $.ajax({
					url: appPath.getAppPath() + '/plans' + this.pager.createAdditionalRequest('?'),
					beforeSend: conf.beforeSend(rootElement),
					dataType: 'json',
					type: 'GET'
				});
			}.bind(this),

			getServicePager: function() {
				return this.pager;
			}.bind(this),

			getPlanById: function(dateTime, rootElement, id) {
				return $.ajax({
					url: appPath.getAppPath() + '/plans?planId=' + id,
					beforeSend: conf.beforeSend(rootElement),
					dataType: 'json',
					type: 'GET'
				});
			},
			savePlan: function(planDto, rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/plans/savePlan',
					beforeSend: conf.beforeSend(rootElement),
					data: JSON.stringify(planDto),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json',
				});
			},
			updatePlan: function(dateTime, rootElement, planDto) {
				return $.ajax({
					url: appPath.getAppPath() + '/updatePlan',
					beforeSend: conf.beforeSend(rootElement),
					data: JSON.stringify(planDto),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json',
				});
			},
			deletePlan: function(id, rootElement) {
				return $.ajax({
					url: appPath.getAppPath() + '/plans/deletePlan',
					beforeSend: conf.beforeSend(rootElement),
					data: JSON.stringify({ id: id }),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json',
				});
			}
		};

	};

	return planService;
});