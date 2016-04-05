define([
	"jquery",
	"underscore",
	"app/helpers/configurator.static.helper",
	"app/helpers/appPath.static.helper"
], function ($, _, configurator, appPath) {
	var tranService = function () {

		this.pager = _.clone(configurator.pager);

		return {
			getAllTrans: function (dateTime, rootElement, pagerScope) {
				if (pagerScope != undefined) {
					this.pager.setSkip(pagerScope.skip);
					this.pager.setTake(pagerScope.take);
					this.pager.setOrder(pagerScope.order);
				}
				return $.ajax({
					url: appPath.getAppPath() + '/transactions' + this.pager.createAdditionalRequest('?'),
					beforeSend: configurator.beforeSend(rootElement),
					dataType: 'json',
					type: 'GET'
				});
			}.bind(this),

			getServicePager: function () {
				return this.pager;
			}.bind(this),

			getTransBySearh: function (rootElement, pagerScope, parameter, value) {
				if (pagerScope != undefined) {
					this.pager.setSkip(pagerScope.skip);
					this.pager.setTake(pagerScope.take);
					this.pager.setOrder(pagerScope.order);
				}
				return $.ajax({
					url: appPath.getAppPath() + '/transactions/search' + this.pager.createAdditionalRequest('?') + '&parameter=' + parameter + '&containsValue=' + value,
					beforeSend: configurator.beforeSend(rootElement),
					dataType: 'json',
					type: 'GET'
				});
			}.bind(this)
		};
	};
	return tranService;
});