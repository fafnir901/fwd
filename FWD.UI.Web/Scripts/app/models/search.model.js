define([
	'jquery',
	'underscore',
	'app/services/search.service'
], function ($, _, SearchService) {
	var searchModel = function () {
		var service = new SearchService();
		return {
			getResults: function (text, element) {
				return service.search(text, element);
			},
			getResultsForTags: function (tags, element) {
				var tagIds = _.chain(tags).pluck("Id").toArray().value();
				return service.searchByTagIds(tagIds, element);
			}
		};
	};

	return searchModel;
});