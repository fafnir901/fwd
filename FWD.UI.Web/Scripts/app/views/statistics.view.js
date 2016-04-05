define([
	'jquery',
	'app/services/article.service',
	'app/helpers/notification.static.helper'
], function ($, ArticleService, errorHelper) {
	var statisticsView = function() {

		var getViewModel = function() {
			if (this.viewModel == undefined) {
				this.viewModel = {};
				this.viewModel.statModel = {};
				this.viewModel.statistics = {};
				this.viewModel.elements = {};
				this.viewModel.elements.container = $('<div class="stat_view"></div>');
				this.viewModel.elements.countOfSentenses = $('<div><span>Количество предложений: <strong></strong></span></div>');
				this.viewModel.elements.countOfWords = $('<div><span>Количество слов: <strong></strong></span></div>');
				this.viewModel.elements.countOfLetters = $('<div><span>Количество букв: <strong></strong></span></div>');
				this.viewModel.elements.articleName = $('<div><span>Название статьи: <strong></strong></span></div>');
				this.viewModel.elements.redrawBar = $('<button style="display:inline-block">Bar</button>');
				this.viewModel.elements.redrawChartBar = $('<button style="display:inline-block">Chart Bar(new)</button>');
				this.viewModel.elements.redrawWedge = $('<button style="display:inline-block">Wedge</button>');
				this.viewModel.elements.redrawTagWedge = $('<button style="display:inline-block">Tag Wedge</button>');
				this.viewModel.elements.redrawScatter = $('<button style="display:inline-block">Scatter</button>');
				this.viewModel.elements.redrawShedule = $('<button style="display:inline-block">Shedule</button>');

				this.viewModel.elements.container.append(this.viewModel.elements.articleName);
				this.viewModel.elements.container.append(this.viewModel.elements.countOfSentenses);
				this.viewModel.elements.container.append(this.viewModel.elements.countOfWords);
				this.viewModel.elements.container.append(this.viewModel.elements.countOfLetters);
				this.viewModel.elements.container.append(this.viewModel.elements.redrawBar);
				this.viewModel.elements.container.append(this.viewModel.elements.redrawChartBar);
				this.viewModel.elements.container.append(this.viewModel.elements.redrawWedge);
				this.viewModel.elements.container.append(this.viewModel.elements.redrawTagWedge);
				this.viewModel.elements.container.append(this.viewModel.elements.redrawScatter);
				this.viewModel.elements.container.append(this.viewModel.elements.redrawShedule);

				this.viewModel.elements.canvas = $('<div id="container"></div>');
				this.viewModel.elements.container.append(this.viewModel.elements.canvas);
			}
			return this.viewModel;
		};

		return {
			init: function(articleId, rootElement) {
				return new ArticleService().getStat(articleId, rootElement) //_getViewModel().elements.container)
					.fail(function(data) {
						errorHelper.showError(data.responseText);
					})
					.success(function(data) {
						getViewModel().statistics = data;
						return data;
					});
			},

			setStatModel: function(model) {
				var viewModel = getViewModel();
				viewModel.statModel = model;
			},
			getModel: function() {
				return getViewModel();
			},
			render: function() {
				var model = getViewModel();
				model.elements.countOfSentenses.find('strong').text(model.statistics.countOfSentenses);
				model.elements.countOfWords.find('strong').text(model.statistics.CountOfWords);
				model.elements.countOfLetters.find('strong').text(model.statistics.CountOfLetters);
				model.elements.articleName.find('strong').text(model.statistics.ArticleName);

				model.elements.redrawBar.off('click');
				model.elements.redrawBar.on('click', function() {
					model.statModel.getModel().iterator = 5;
					model.statModel.reDrawBar();
				});

				model.elements.redrawChartBar.off('click');
				model.elements.redrawChartBar.on('click', function() {
					model.statModel.getModel().iterator = 5;
					model.statModel.reDrawChartBar();
				});

				model.elements.redrawWedge.off('click');
				model.elements.redrawWedge.on('click', function() {
					model.statModel.getModel().iterator = 5;
					model.statModel.reDrawWedge();
				});

				model.elements.redrawTagWedge.off('click');
				model.elements.redrawTagWedge.on('click', function() {
					model.statModel.getModel().iterator = 5;
					model.statModel.reDrawRadialTagPie();
				});

				model.elements.redrawScatter.off('click');
				model.elements.redrawScatter.on('click', function() {
					if (model.statModel.getModel().iterator < 30) {
						model.statModel.getModel().iterator += 5;
					} else {
						model.statModel.getModel().iterator = 30;
					}

					model.statModel.reDrawScatter();
				});

				model.elements.redrawShedule.off('click');
				model.elements.redrawShedule.on('click', function() {
					model.statModel.getModel().iterator = 5;
					model.statModel.reDrawSchedule();
				});
				return getViewModel().elements.container;
			}
		};
	};

	return statisticsView;
});