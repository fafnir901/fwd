define([
	'jquery',
	'underscore',
	'konva',
	'chart',
	'app/services/statistics.service',
	'app/helpers/notification.static.helper',
	'app/helpers/common.static.helper'
], function ($, _, Konva, Chart, StatisticsService, errorHelper, commonHelper) {
	var statModel = function () {
		var conva = Konva;
		var chart = Chart;

		this.model = { chart: null };

		var getModel = function () {
			return this.model;
		}.bind(this);

		var init = function (convaIdSelector, width, height, rootElement) {
			var model = getModel();
			model.service = new StatisticsService();
			model.rootElement = rootElement;
			model.data = { dataForShedule: {} };
			model.iterator = 5;
			model.currentItem = {};
			model.convaSelector = convaIdSelector;
			model.stage = new conva.Stage({
				container: convaIdSelector,
				width: width,
				height: height,
				step: 20
			});

			model.config = {
				maxHeight: height - 50,
				maxContentWidth: width,
				maxContentHeight: height,
				width: 15,
				common: {
					strokeWidth: 2,
					shadowBlur: 10,
					opacity: 0.8,
					shadowOffset: {
						x: 5,
						y: 5
					},
					scale: {
						x: 1,
						y: 1
					}
				},
				another: {
					fill: 'green',
					stroke: 'black',
				},
				current: {
					fillLinearGradientStartPoint: { x: -50, y: -50 },
					fillLinearGradientEndPoint: { x: 50, y: 50 },
					fillLinearGradientColorStops: [0, 'red', 1, 'yellow'],
					stroke: 'black',
				}
			};
			model.mainLayer = new conva.Layer();
			model.toolTipLayer = new conva.Layer();
			model.staticLayer = new conva.Layer();
		};

		var getData = function (rootElement) {
			var model = getModel();
			return model.service.getData(rootElement)
				.success(function (data) {
					model.data = data;
				})
				.fail(function (text) {
					errorHelper.showError(text.responseText);
				});
		};

		var prepareCountString = function (str) {
			var current = '';
			var rev = reverse(str);
			for (var i = 0; i < rev.length; i++) {
				if (i % 3 > 0) {
					current += rev[i];
				} else {
					current += ' ' + rev[i];
				}
			}
			str = reverse(current);
			return str;
		};

		var reverse = function (str) {
			return str.split('').reverse().join('');
		};

		var createToolTip = function () {
			var tooltip = new conva.Label({
				opacity: 0.75,
				visible: false,
				listening: false
			});
			tooltip.add(new conva.Tag({
				fill: 'black',
				pointerDirection: 'down',
				pointerWidth: 10,
				pointerHeight: 10,
				lineJoin: 'round',
				shadowColor: 'black',
				shadowBlur: 10,
				shadowOffset: 10,
				shadowOpacity: 0.2
			}));
			tooltip.add(new conva.Text({
				text: '',
				fontFamily: 'Calibri',
				fontSize: 18,
				padding: 5,
				fill: 'white'
			}));
			return tooltip;
		};

		var createGrid = function (maxHeight, maxWidth, layer) {
			var model = getModel();
			var step = model.stage.attrs.step;
			var maxIteratorValue = parseInt(model.stage.attrs.width / step);

			for (var i = 0; i < maxIteratorValue; i++) {
				var gridHorizontalLine = new conva.Line({
					points: [0, i * step, model.stage.attrs.width, i * step],
					stroke: 'black',
					strokeWidth: 1,
					lineCap: 'round',
					lineJoin: 'round',
					opacity: 0.3

				});

				var gridVerticalLine = new conva.Line({
					points: [i * step, 0, i * step, maxHeight],
					stroke: 'black',
					strokeWidth: 1,
					lineCap: 'round',
					lineJoin: 'round',
					opacity: 0.3
				});
				layer.add(gridHorizontalLine);
				layer.add(gridVerticalLine);
			}
		};

		var drawBar = function (data, model, iteratorX, maxHeight, currentItem) {

			var toolTip = createToolTip();
			var con = 20;
			var staticText = new conva.Text({
				text: 'Общее количество букв: ' + prepareCountString(data.TotalCount.toString()),
				fontFamily: 'Calibri',
				fontSize: 18,
				fontStyle: 'bold',
				padding: 5,
				fill: 'black'
			});
			var staticText1 = new conva.Text({
				text: 'Процент чтения статей: ' + data.ReadablePercent.toFixed(2).toString() + '% - ' + data.ReadableCount + ' раз',
				fontFamily: 'Calibri',
				fontSize: 18,
				fontStyle: 'bold',
				padding: 5,
				fill: 'black',
				y: con
			});
			var staticText2 = new conva.Text({
				text: 'Процент обновления статей: ' + data.UpdatablePercent.toFixed(2).toString() + '% - ' + data.UpdatableCount + ' раз',
				fontFamily: 'Calibri',
				fontSize: 18,
				fontStyle: 'bold',
				padding: 5,
				fill: 'black',
				y: con * 2
			});
			var staticText3 = new conva.Text({
				text: 'Процент добавления статей: ' + data.WritablePercent.toFixed(2).toString() + '% - ' + data.WritableCount + ' раз',
				fontFamily: 'Calibri',
				fontSize: 18,
				fontStyle: 'bold',
				padding: 5,
				fill: 'black',
				y: con * 3
			});
			var staticText4 = new conva.Text({
				text: 'Процент удаления статей: ' + data.DeletablePercent.toFixed(2).toString() + '% - ' + data.DeletableCount + ' раз',
				fontFamily: 'Calibri',
				fontSize: 18,
				fontStyle: 'bold',
				padding: 5,
				fill: 'black',
				y: con * 4
			});
			model.staticLayer.add(staticText);
			model.staticLayer.add(staticText1);
			model.staticLayer.add(staticText2);
			model.staticLayer.add(staticText3);
			model.staticLayer.add(staticText4);
			model.toolTipLayer.add(toolTip);

			_.each(data.TranViewModels, function (item) {
				var config = currentItem.Name == item.Name
					? _.clone(model.config.current)
					: _.clone(model.config.another);

				var currentHeight = -(model.config.maxHeight * (((item.LetterCount * 100) / maxHeight.LetterCount) / 100));
				config.x = iteratorX;
				config.y = model.config.maxHeight + 50;
				config.width = model.config.width;
				config.height = currentHeight;
				config.strokeWidth = model.config.common.strokeWidth;
				//config.shadowBlur = model.config.common.shadowBlur;
				config.opacity = model.config.common.opacity;
				//config.shadowOffset = model.config.common.shadowOffset;
				config.scale = model.config.common.scale;
				config.item = item;

				var rect = new conva.Rect(config);
				rect.id = "Rect" + iteratorX;

				iteratorX = iteratorX + model.config.width + 5;
				model.mainLayer.add(rect);
			});

			model.mainLayer.off("mouseover");
			model.mainLayer.off("mouseout");
			model.mainLayer.on("mouseover", function (evt) {
				var node = evt.target;
				if (node.className == 'Rect') {
					node.setAttrs({
						shadowColor: 'white',
						//scale: {
						//	x: model.config.common.scale.x * 1.2,
						//	y: model.config.common.scale.y * 1.2
						//}
					});

					node.draw();
					//var mousePos = node.getStage().getPointerPosition();
					//node.tween.play();
					toolTip.position({
						x: node.attrs.x + model.config.width / 2,
						y: model.config.maxHeight + 50 + node.attrs.height
					});
					toolTip.getText().setText(node.attrs.item.Name + ".Кол-во симоволов:" + prepareCountString(node.attrs.item.LetterCount.toString()));
					toolTip.show();
					model.toolTipLayer.batchDraw();
				}
			});

			model.mainLayer.on("mouseout", function (evt) {
				var node = evt.target;
				if (node.className == 'Rect') {
					node.setAttrs({
						shadowColor: 'black',
						//scale: model.config.common.scale
					});
					node.draw();
					//node.tween.reverse();

					toolTip.hide();
					model.toolTipLayer.draw();
					//model.mainLayer.batchDraw();
				}
			});

			model.stage.add(model.staticLayer);
			model.stage.add(model.mainLayer);
			model.stage.add(model.toolTipLayer);


			model.stage.setAttrs({
				width: data.TranViewModels.length * (model.config.width + 5)
			});
			createGrid(model.config.maxContentHeight, model.config.maxContentWidth, model.staticLayer);
			model.staticLayer.draw();
			model.mainLayer.draw();

		};

		var drawChartBar = function (data) {
			var model = getModel();
			clearStageLayers();
			var first = getFirstCanvasFromKonva(model);

			var container = $('#' + model.convaSelector).find('.konvajs-content');

			var width = data.TranViewModels.length * (model.config.width + 5);

			var canvas = $('<canvas id="chart_canvas"></canvas>');
			canvas.width(width);
			canvas.height(container.height());

			canvas[0].height = container.height();
			canvas[0].width = width;

			canvas.css({
				'width': width + 'px',
				'height': container.height() + 'px',
				'padding': '0px',
				'margin': '0px',
				'border': '0px',
				'top': '0px',
				'left': '0px',
				'position': 'absolute',
				'background': 'transparent'

			});



			$.when(container.append(canvas)).then(function () {
				first.css('display', 'none');

				chart.defaults.global.scaleLineColor = 'rgba(0,0,0,1)';
				chart.defaults.global.scaleLineWidth = 4;
				chart.defaults.global.scaleFontColor = 'rgba(0,0,0,1)';
				//chart.defaults.global.scaleShowLabels = true;
				//chart.defaults.global.showXLabels = true;
				//chart.defaults.global.scaleShowLabels = false;

				var currentData = [];
				var labels = [];
				_.each(data.TranViewModels, function (item) {
					labels.push(item.Name);
					currentData.push(item.LetterCount);
				});


				var myData = {
					labels: labels,
					datasets: [
						{
							label: "My First dataset",
							fillColor: "rgba(0,255,0,0.5)",
							strokeColor: "rgba(0,0,0,0.8)",
							highlightFill: "rgba(255,255,255,0.75)",
							highlightStroke: "rgba(0,0,0,1)",
							data: currentData
						}
					]
				};

				$(document).ready(function () {
					var ctx = prepareCtx(canvas);
					var myPieChart = new chart(ctx).Bar(myData, {
						barValueSpacing: 1,
						showXLabels: false,
						scaleGridLineColor: 'rgba(0,0,0,0.3)',
						scaleGridLineWidth: 1,
						tooltipTemplate: "<%if (label){%><%=label%>: <%}%><%= value %> letters",
						/*showScale: true,*/
					});
					model.chart = myPieChart;
				});

			});
		};

		var clearStageLayers = function () {
			var model = getModel();

			if (model.mainLayer != undefined)
				model.mainLayer.destroyChildren();

			if (model.staticLayer != undefined)
				model.staticLayer.destroyChildren();

			if (model.toolTipLayer != undefined)
				model.toolTipLayer.destroyChildren();

			var container = $('#' + model.convaSelector).find('.konvajs-content');
			if (container.length > 0 && model.chart != null) {
				$('#chart_canvas').remove();//.css('display', 'none');
				var first = $(container.children().get(0));
				if (first.length != 0) {
					var ctx = first.get(0).getContext("2d"); //document.getElementById("myChart").getContext("2d");
					first.css('display', '');
					ctx.clearRect(0, 0, first.get(0).width, first.get(0).height);
				}
				container.children().css('display', '');
				model.chart.clear();
				model.chart.destroy();
			}

			chart.defaults.global.scaleShowLabels = true;
		};

		var prepareData = function (currentItem, rootElement) {
			var model = getModel();
			var newRootElement = $('#' + model.convaSelector);
			getData(newRootElement).then(function (data) {
				model.data = data;
				model.currentItem = currentItem;
				var maxHeight = _.max(model.data.TranViewModels, function (item) {
					return item.LetterCount;
				});
				var iteratorX = 2;
				drawBar(model.data, model, iteratorX, maxHeight, model.currentItem);

				//_drawChartBar(model.data);
			});
		};

		var drawWedge = function (currentGrad, anotherGrad, model) {

			var conf = _.clone(model.config.current);

			conf.rotationDeg = anotherGrad;
			conf.radius = 200;
			conf.angleDeg = currentGrad;
			conf.x = conf.radius * 2;//model.stage.getWidth() / 2;
			conf.y = model.stage.getHeight() / 2;

			var another = _.clone(model.config.another);
			another.radius = 200;
			another.angleDeg = anotherGrad;
			another.x = another.radius * 2;//model.stage.getWidth() / 2;
			another.y = model.stage.getHeight() / 2;



			var wedge = new Konva.Wedge(conf);
			var wedge1 = new Konva.Wedge(another);

			model.mainLayer.add(wedge1);
			model.mainLayer.add(wedge);


			createGrid(model.config.maxContentHeight, model.config.maxContentWidth, model.staticLayer);

			model.stage.add(model.staticLayer);
			model.stage.add(model.mainLayer);
			model.stage.add(model.toolTipLayer);


			model.stage.setAttrs({
				width: model.data.TranViewModels.length * (model.config.width + 5)
			});

			model.staticLayer.draw();
			model.mainLayer.draw();

		};

		var normalizeCssForPie = function (canvas, first) {
			canvas.width(first.width() / 5);
			canvas.height(first.height());
			canvas[0].height = first.height();
			canvas[0].width = first.width() / 5;
			canvas.css({
				'width': first.width() / 5 + 'px',
				'height': first.height() + 'px',
				'padding': '0px',
				'margin': '0px',
				'border': '0px',
				'top': '0px',
				'left': '0px',
				'position': 'absolute',
				'background': 'transparent'
			});
		};

		var drawTagPie = function (tagsInfo) {
			var model = getModel();
			var first = getFirstCanvasFromKonva(model);
			var canvas = $('<canvas id="chart_canvas"></canvas>');
			normalizeCssForPie(canvas, first);

			var container = $('#' + model.convaSelector).find('.konvajs-content');

			$.when(container.append(canvas)).then(function () {
				first.css('display', 'none');

				chart.defaults.global.scaleLineColor = 'rgba(0,0,0,1)';
				chart.defaults.global.scaleLineWidth = 4;
				chart.defaults.global.scaleFontColor = 'rgba(0,0,0,1)';

				var dataSets = [];
				var totalCount = _.reduce(tagsInfo, function (memo, num) { return num.Count + memo; }, 0);


				_.each(tagsInfo, function (item) {

					var rRandom = commonHelper.getRandomInt(0, 255);
					var gRandom = commonHelper.getRandomInt(0, 255);
					var bRandom = commonHelper.getRandomInt(0, 255);

					var color = 'rgba(' + rRandom + ',' + gRandom + ',' + bRandom + ',0.3)';
					var higth = 'rgba(' + rRandom + ',' + gRandom + ',' + bRandom + ',0.8)';

					dataSets.push({
						value: item.Count,
						label: item.Name + ' [' + ((item.Count * 100) / (totalCount)).toFixed(2) + '%' + ']',
						color: color,
						highlight: higth
					});
				});


				$(document).ready(function () {
					var ctx = prepareCtx(canvas);
					var myPieChart = new chart(ctx).Doughnut(dataSets, {
						segmentStrokeColor: 'black',
						tooltipTemplate: "<%if (label){%><%=label%>: <%}%><%= value %>",
					});
					model.chart = myPieChart;
				});

			});
		};

		var drawPie = function (name, count1, anotherCount) {
			var model = getModel();
			//_clearStageLayers();

			var first = getFirstCanvasFromKonva(model);
			//var ctx = _prepareCtx(first);

			var canvas = $('<canvas id="chart_canvas"></canvas>');
			normalizeCssForPie(canvas, first);

			var container = $('#' + model.convaSelector).find('.konvajs-content');
			$.when(container.append(canvas)).then(function () {
				first.css('display', 'none');

				chart.defaults.global.scaleLineColor = 'rgba(0,0,0,1)';
				chart.defaults.global.scaleLineWidth = 4;
				chart.defaults.global.scaleFontColor = 'rgba(0,0,0,1)';

				var data = [
					{
						value: count1,
						color: "yellow",
						highlight: "lightyellow",
						label: name + ' [' + ((count1 * 100) / (anotherCount + count1)).toFixed(2) + '%' + ']'
					},
					{
						value: anotherCount,
						color: "green",
						highlight: "lightgreen",
						label: "Other [" + ((anotherCount * 100) / (anotherCount + count1)).toFixed(2) + '%' + ']'
					}
				];


				$(document).ready(function () {
					var ctx = prepareCtx(canvas);
					var myPieChart = new chart(ctx).Pie(data, {
						segmentStrokeColor: 'black',
						tooltipTemplate: "<%if (label){%><%=label%>: <%}%><%= value %> letters",
					});
					model.chart = myPieChart;
				});

			});
		};

		var createAxisForSchedule = function (model, layer, maxHeight, maxWidth) {

			var gridHorizontalLine = new conva.Line({
				points: [10, maxHeight - 25, model.stage.attrs.width - 20, maxHeight - 25],
				stroke: 'black',
				strokeWidth: 4,
				lineCap: 'round',
				lineJoin: 'round',
				opacity: 1

			});

			var gridVerticalLine = new conva.Line({
				points: [40, 20, 40, maxHeight + 20],
				stroke: 'black',
				strokeWidth: 4,
				lineCap: 'round',
				lineJoin: 'round',
				opacity: 1
			});

			var vertArrow = new conva.Line({
				points: [45, 45, 40, 10, 35, 45],
				stroke: 'black',
				strokeWidth: 4,
				lineCap: 'round',
				lineJoin: 'round',
				opacity: 1
			});


			layer.add(gridHorizontalLine);
			layer.add(gridVerticalLine);
			layer.add(vertArrow);
		};

		var createMarksForAxis = function (model) {
			var totalCount = _.keys(model.data.dataForShedule).length;
			var array = _.chain(model.data.dataForShedule).values().pluck('length').value();
			var max = _.max(array);
			var maxHeight = model.config.maxContentHeight;
			var maxWidth = model.config.maxContentWidth;
			var step = model.stage.attrs.step * 4;
			var offSet = 0;

			for (var i = 1; i < totalCount - 1 ; i++) {
				var gridVerticalLine = new conva.Line({
					points: [step * i + offSet, maxHeight - 35, step * i + offSet, maxHeight - 15],
					stroke: 'black',
					strokeWidth: 2,
					lineCap: 'round',
					lineJoin: 'round',
					opacity: 0.7
				});
				model.staticLayer.add(gridVerticalLine);

			}

			step = step / 2;
			max = max / 10;
			offSet = 8;
			for (var i = max + 1; i > 1; i--) {
				var horizontalMarker = new conva.Line({
					points: [30, maxHeight - offSet - step * i, 50, maxHeight - offSet - step * i],
					stroke: 'black',
					strokeWidth: 2,
					lineCap: 'round',
					lineJoin: 'round',
					opacity: 0.7
				});
				model.staticLayer.add(horizontalMarker);
			}
		};

		var getFirstCanvasFromKonva = function (model) {
			var container = $('#' + model.convaSelector).find('.konvajs-content');
			container.children().css('display', 'none');
			return $(container.children().get(0));
		};

		var prepareCtx = function (first) {
			var ctx = first.get(0).getContext("2d");//document.getElementById("myChart").getContext("2d");
			$(first).css('display', '');
			ctx.clearRect(0, 0, first.width, first.height);

			return ctx;
		};

		var drawSchedule = function () {
			var model = getModel();
			clearStageLayers();

			//_createGrid(model.config.maxContentHeight, model.config.maxContentWidth, model.staticLayer);
			//_createAxisForSchedule(model, model.staticLayer, model.config.maxContentHeight, model.config.maxContentWidth);
			//_createMarksForAxis(model);
			//model.staticLayer.draw();
			//model.mainLayer.draw();

			var first = getFirstCanvasFromKonva(model);

			//var canvas = $('<canvas id="chart_canvas"></canvas>');
			//canvas.width(first.width());
			//canvas.height(first.height());



			//container.append(canvas);
			//canvas.css({ 'width': first.width() + 'px', 'height': first.height() + 'px' });

			var ctx = prepareCtx(first);
			var dtos = model.data.dataForShedule;
			var currentData = [];

			for (var k in dtos) {
				currentData.push(dtos[k].length);
			}
			var data = {
				labels: Object.keys(dtos),
				datasets: [
					{
						label: "My First dataset",
						fillColor: "rgba(0,0,0,0.3)",//"rgba(220,220,220,0.2)",
						strokeColor: "rgba(0,0,0,1)",
						pointColor: "rgba(220,220,220,1)",
						pointStrokeColor: "#fff",
						pointHighlightFill: "#fff",
						pointHighlightStroke: "rgba(220,220,220,1)",
						data: currentData
					}
				]
			};
			chart.defaults.global.scaleLineColor = 'rgba(0,0,0,1)';
			chart.defaults.global.scaleLineWidth = 4;
			chart.defaults.global.scaleFontColor = 'rgba(0,0,0,1)';
			var myNewChart = new chart(ctx).Line(data,
			{
				scaleGridLineColor: 'rgba(0,0,0,0.3)',
				scaleGridLineWidth: 1,
				tooltipTemplate: "<%if (label){%><%=label%>: <%}%><%= value %> articles were added",
				showXLabels: true
			});
			model.chart = myNewChart;

		};

		var createAxisForScatter = function (maxHeight, maxWidth, layer) {
			var model = getModel();
			var currentMaxHeight = maxHeight - 50;
			var totalCount = _.max(model.data.TranViewModels, function (item) {
				return item.LetterCount;
			}).LetterCount;

			var diapasone = [];

			for (var i = 0; i < model.iterator; i++) {
				var item = {
					count: parseInt((totalCount / model.iterator) * i),
					width: (((totalCount / model.iterator) * i) * maxWidth) / totalCount
				};
				diapasone.push(item);

				var verticalLine = new conva.Line({
					points: [item.width, currentMaxHeight, item.width, 0],
					stroke: 'black',
					strokeWidth: 2,
					lineCap: 'round',
					lineJoin: 'round',
					opacity: 0.8,
					dash: [10, 10]
				});

				var staticText = new conva.Text({
					text: prepareCountString(item.count.toString()),
					fontFamily: 'Calibri',
					fontSize: 36 - model.iterator > 10 ? 36 - model.iterator : 10,
					fontStyle: 'bold',
					padding: 5,
					fill: 'black',
					x: item.width,
					y: currentMaxHeight + 10
				});

				layer.add(staticText);
				layer.add(verticalLine);
			}

			var verticalLine1 = new conva.Line({
				points: [maxWidth, currentMaxHeight, maxWidth, 0],
				stroke: 'black',
				strokeWidth: 2,
				lineCap: 'round',
				lineJoin: 'round',
				opacity: 0.8,
				dash: [10, 10]
			});

			var staticText1 = new conva.Text({
				text: prepareCountString(totalCount.toString()),
				fontFamily: 'Calibri',
				fontSize: 36 - model.iterator > 10 ? 36 - model.iterator : 10,
				fontStyle: 'bold',
				padding: 5,
				fill: 'black',
				x: maxWidth,
				y: currentMaxHeight + 10
			});

			layer.add(staticText1);
			layer.add(verticalLine1);

			var gridHorizontalLine = new conva.Line({
				points: [0, currentMaxHeight, maxWidth, currentMaxHeight],
				stroke: 'black',
				strokeWidth: 4,
				lineCap: 'round',
				lineJoin: 'round',
				opacity: 0.8
			});

			layer.add(gridHorizontalLine);
			return diapasone;
		};

		var drawScatter = function (model) {
			createGrid(model.config.maxContentHeight, model.config.maxContentWidth, model.staticLayer);
			var diap = createAxisForScatter(model.config.maxContentHeight, model.stage.attrs.width - 100, model.staticLayer);
			//var initialWidth = diap[1].width;
			var radius = 15;
			var initialHeight = model.config.maxContentHeight - 50 + radius;
			var maxHeight = _.max(model.data.TranViewModels, function (item) {
				return item.LetterCount;
			}).LetterCount;

			_.each(model.data.TranViewModels, function (item) {

				//var currentRes = _.chain(diap)
				//	.filter(function (diapItem) {
				//		return item.LetterCount <= diapItem.count;
				//	}).sortBy('count')
				//	.first()
				//	.value();
				//if (currentRes == null || currentRes == undefined) {
				//	currentRes = _.chain(diap)
				//	.filter(function (diapItem) {
				//		return item.LetterCount >= diapItem.count;
				//	}).sortBy('count')
				//	.first()
				//	.value();
				//}

				//var xPosition = currentRes.width - (initialWidth / 2) - radius;
				var xPosition = parseInt(((item.LetterCount * 100 / maxHeight) / 100) * model.stage.attrs.width);
				if (xPosition >= model.stage.attrs.width) {
					xPosition = model.stage.attrs.width - 100;
				}

				var yPosition = parseInt((((item.LetterCount * 100) / maxHeight) / 100) * initialHeight + radius);
				//var yPosition = (((item.LetterCount * 100) / (currentRes.count == 0 ? 100 : currentRes.count))) * initialHeight / 100 + radius;
				if (yPosition >= initialHeight) {
					yPosition -= 50;
				}
				var circle = new conva.Circle({
					x: xPosition, //+ Math.floor((Math.random() * 20) + 10),
					y: initialHeight - (yPosition + Math.floor((Math.random() * 20) + 10)),
					radius: radius,
					fill: model.currentItem.Name == item.Name ? 'red' : 'blue',
					opacity: model.currentItem.Name == item.Name ? 0.8 : 0.3,
					item: item
				});
				model.mainLayer.add(circle);
			});
			var toolTip = createToolTip();
			model.toolTipLayer.add(toolTip);
			model.stage.add(model.staticLayer);
			model.stage.add(model.mainLayer);
			model.stage.add(model.toolTipLayer);

			model.mainLayer.off("mouseover");
			model.mainLayer.off("mouseout");
			model.mainLayer.on("mouseover", function (evt) {
				var node = evt.target;
				if (node.className == 'Circle') {
					node.setAttrs({
						opacity: 0.5,
						strokeWidth: 2,
						stroke: 'black',
						fill: 'yellow'
					});

					node.draw();
					toolTip.position({
						x: node.attrs.x,
						y: node.attrs.y
					});
					toolTip.getText().setText(node.attrs.item.Name);// + ".Кол-во симоволов:" + _prepareCountString(node.attrs.item.LetterCount.toString()));
					toolTip.show();
					model.toolTipLayer.batchDraw();
				}
			});

			model.mainLayer.on("mouseout", function (evt) {
				var node = evt.target;
				if (node.className == 'Circle') {
					node.setAttrs({
						opacity: model.currentItem.Name == node.attrs.item.Name ? 0.8 : 0.3,
						strokeWidth: 0,
						stroke: 'transperent',
						fill: model.currentItem.Name == node.attrs.item.Name ? 'red' : 'blue',
					});
					node.draw();

					toolTip.hide();
					model.toolTipLayer.draw();
					model.mainLayer.draw();
				}
			});

			model.staticLayer.draw();
			model.mainLayer.draw();

		};

		return {
			init: function (convaIdSelector, width, height, currentItem, rootElement) {
				clearStageLayers();
				init(convaIdSelector, width, height, rootElement);
				prepareData(currentItem, rootElement);
			},

			getModel: function () {
				return getModel();
			},

			reDrawBar: function () {
				var model = getModel();
				var maxHeight = _.max(model.data.TranViewModels, function (item) {
					return item.LetterCount;
				});
				clearStageLayers();
				var iteratorX = 2;
				drawBar(model.data, model, iteratorX, maxHeight, model.currentItem);

			},

			reDrawWedge: function () {
				var model = getModel();
				clearStageLayers();
				var curr = _.filter(model.data.TranViewModels, function (item) {
					return item.Name == model.currentItem.Name;
				});

				//var grad = (curr[0].LetterCount * 360) / model.data.TotalCount;
				//var anotherGrad = 360 - grad;
				//_drawWedge(grad, anotherGrad, model);
				drawPie(curr[0].Name, curr[0].LetterCount, model.data.TotalCount - curr[0].LetterCount);
			},

			reDrawChartBar: function () {
				var model = getModel();
				drawChartBar(model.data);
			},

			reDrawScatter: function () {
				var model = getModel();
				clearStageLayers();

				drawScatter(model);
			},

			reDrawSchedule: function () {
				var model = getModel();
				clearStageLayers();
				if (model.data.dataForShedule == undefined || model.data.dataForShedule == null) {
					model.service.getDataForShedule(model.rootElement).success(function (data) {
						model.data.dataForShedule = data.ArticleDtos;
						drawSchedule();
					}).fail(function (data) {
						errorHelper.showError(data.responseText);
					});
				} else {
					drawSchedule();
				}
			},

			reDrawRadialTagPie: function () {

				var model = getModel();

				model.service.getTagDataForRadialShedule(model.rootElement)
					.success(function (data) {
						clearStageLayers();
						drawTagPie(data);
					}).fail(function (err) {
						errorHelper.showError(err.responseText);
					});
			}
		};
	};
	return statModel;
});