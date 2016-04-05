define([
	'jquery',
	'underscore'
], function($,_) {
	var rateComponent = function() {

		var model = {
			container: {
				star1: {},
				star2: {},
				star3: {},
				star4: {},
				star5: {}
			},
			mark: 0,
		};

		var preInit = function(func) {
			model.container = $('<div class="rating"></div>');
			//because, css using revert
			model.star1 = $('<span mark="5" class="rating_star">☆</span>');
			model.star2 = $('<span mark="4" class="rating_star">☆</span>');
			model.star3 = $('<span mark="3" class="rating_star">☆</span>');
			model.star4 = $('<span mark="2" class="rating_star">☆</span>');
			model.star5 = $('<span mark="1" class="rating_star">☆</span>');

			model.container.append(model.star1);
			model.container.append(model.star2);
			model.container.append(model.star3);
			model.container.append(model.star4);
			model.container.append(model.star5);

			$(model.container.find('.rating_star')).on('click', function() {
				var mark = $(this).attr('mark');
				setCurrentMark(mark);
				model.mark = mark;
				func(model.mark);
			});
		};

		var setCurrentMark = function(currentMark) {
			model.mark = currentMark;
			_.each(model.container.children(), function(item) {
				if ($(item).attr('mark') <= currentMark) {
					$(item).addClass('rating_color');
				} else {
					$(item).removeClass('rating_color');
				}
			});
		};

		var init = function(currentMark, func) {
			preInit(func);
			_.each(model.container.children(), function(item) {
				if ($(item).attr('mark') <= currentMark) {
					$(item).addClass('rating_color');
				} else {
					$(item).removeClass('rating_color');
				}
			});
		};

		return {
			render: function(currentMark, callback) {
				init(currentMark, callback);
				return model.container;
			},
			getModel: function() {
				return model;
			},
			setCurrentMark: function(currentMark) {
				setCurrentMark(currentMark);
			}
		};
	};
	return rateComponent;
});