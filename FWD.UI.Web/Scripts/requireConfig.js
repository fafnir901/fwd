requirejs.config({
	baseUrl: "Scripts",
	paths: {
		'jquery': 'lib/jquery-1.10.2.min',
		'underscore': 'lib/underscorejs',
		'konva': 'lib/konva.min',
		'chart': 'lib/Chart',
		'signalr.core': 'lib/jquery.signalR-2.1.2.min', /*http://stackoverflow.com/questions/17598006/signalr-require-js-configuration*/
		'signalr.hubs': '../signalr/hubs?'
	},
	shim: {
		'underscore': {
			exports: '_'
		},
		"signalr.core": {
			deps: ["jquery"],
			exports: "$.connection"
		},
		"signalr.hubs": {
			deps: ["signalr.core"],
		}
	}
});

require(
	[
		"jquery",
		"application"
	],
	function ($, application) {
		$(document).ready(function () {
			console.debug();
			var app = new application();
			app.run();
		});
	});