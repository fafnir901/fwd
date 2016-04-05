define([
	'underscore',
	'app/services/tag.service'
], function (_, TagService) {
	var tagModel = function () {
		var model = {};

		var enitity = {
			id: {},
			name: '',
			tagType: {},
			tagColor: {},
			priority: {}
		};

		model.service = new TagService();

		return {
			getAll: function (rootElement) {
				return model.service.getCommonInfo(rootElement);
			},

			getById: function (rootElement) {
				return model.service.getById(rootElement);
			},

			update: function (rootElement, entity) {
				return model.service.update(rootElement, entity);
			},

			save: function (rootElement, entity) {
				return model.service.save(rootElement, entity);
			},
			getTag: function (rootElement, id) {
				return model.service.getTag(rootElement, id);
			},
			delete: function (rootElement, id) {
				return model.service.delete(rootElement, id);
			}
		};
	};
	return tagModel;
});