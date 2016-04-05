define([], function() {
	var AppPath = (function() {
		return {
			getAppPath: function() {
				var location = window.location;
				var pathToPage = location.protocol + "//" + location.host + '/' + location.pathname.split('/')[1];
				return pathToPage;
			}
		};
	})();
	return AppPath;
});
