define([
	'jquery',
	'underscore',
	'app/helpers/configurator.static.helper'
], function ($, _, conf) {
	var tagContainerComponet = function () {

		var makeTagContainer = function (tags, option) {
			var cssClass = "tag_container_view";
			var smallClass = '';
			var additionalStyle = '';
			if (option != undefined) {
				var preventDefaultStyle = option.preventDefaultStyle || false;
				if (preventDefaultStyle) {
					cssClass = '';
				}
				if (option.anotherClass != undefined) {
					cssClass = option.anotherClass;
				}
				if (option.additionalStyle != undefined) {
					additionalStyle = option.additionalStyle;
				}
				var isSmall = option.isSmall || false;
				if (isSmall) {
					smallClass = 'tag_small';
				}
			}

			if (conf.featureList.Tag) {
				var $tagContainer = $('<div style="' + additionalStyle + '" class="' + cssClass + '"></div>');
				var currentTags = _.sortBy(tags, 'Priority').reverse();
				_.each(currentTags, function (item) {
					var $tag = $('<a class="tag"></a>');
					$tag.addClass(item.TagColor);
					$tag.addClass(smallClass);
					$tag.text(item.Name);
					$tag.attr('priority', item.Priority);
					$tagContainer.append($tag);

				});
				return $tagContainer;
			}
		};

		var makeDtosFromString = function (str) {
			str = str.replace(new RegExp('\'', 'g'), '"');
			var tags = JSON.parse(str);
			return tags;
		};

		return {
			render: function (tags, option) {
				
				return makeTagContainer(tags, option);
			},

			renderFromString: function (str, option) {
				var tags = makeDtosFromString(str);
				return makeTagContainer(tags, option);
			}
		};
	};

	return tagContainerComponet;
});