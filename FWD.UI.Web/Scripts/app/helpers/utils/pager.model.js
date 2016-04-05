define([
],
function () {
	var pager = (function () {

		var model = {
			skip: 0,
			take: 999,
			order:null
		};

		var createAdditionalRequest = function(firstSimbol) {
			return firstSimbol + 'skip=' + model.skip + '&take=' + model.take + '&order=' + model.order;
		};

		return {
			createAdditionalRequest: function(firstSimbol) {
				return createAdditionalRequest(firstSimbol || '?');
			},

			setSkip: function(skip) {
				model.skip = skip;
			},

			setTake: function(take) {
				model.take = take;
			},

			setOrder: function(order) {
				model.order = order;
			},

			getModel: function() {
				return model;
			}
		};
	})();
	return pager;
});