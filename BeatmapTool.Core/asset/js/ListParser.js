(function(w) {
	var beatmap_url = 'http://osu.ppy.sh/p/beatmaplist?l=1&r=0&q=&g=0&la=0&s=4&o=1&page=';
	var beatmap_index = 125;
	// var slice = Array.prototype.slice;
	// var debug = function() {
	// };
	var main = function(callback) {
		var tStart = new Date().getTime();

		var beatmapList = [];

		var onAllDone = function(result) {
			// debug("time : " + (new Date().getTime() - tStart) / 1000);
			// debug(result.length);
			callback(result);
		};

		var getList = function(url, maxIndex, doneCallback) {
			var list = [];
			var currentPage = 1;
			for(var i = 1; i <= maxIndex; i++) {
				(function(i) {
					var pageList = [];
					var tryTime = 0;
					var parseBlock = function(index, item) {
						var beatmap = new Beatmap();
						beatmap.id = parseInt(item.id);
						beatmap.video = $('.icon-film', item).length;
						beatmap.artist = $('.maintext span.artist', item).text();
						beatmap.title = $('.maintext a.title', item).text();
						beatmap.creator = $('.left-aligned div span.light + a', item).text();
						beatmap.date = new Date($('.right-aligned .small-details .initiallyHidden', item).text().match(/\w\w\w\s+\d+,\s\d+/));
						beatmap.genre = $('.right-aligned .tags a:eq(0)', item).text();
						beatmap.language = $('.right-aligned .tags a:eq(1)', item).text();
						beatmap.pack = $('.right-aligned .small-details .initiallyHidden a', item).text().replace("Pack ", "");
						beatmap.source = $('.left-aligned .initiallyHidden:has(span.light)', item).text().replace("from ", "");
						beatmap.diffCount = $('.left-aligned .diffIcon', item).length;
						beatmap.approved(false);
						beatmap.hidden(false);
						beatmap.rank(true);

						if(beatmap.date.getTime == NaN) {
							beatmap.date = new Date('1990-01-01');
						}
						if(beatmap.id == NaN) {
							throw 'id invalid!';
						}
						pageList.push(beatmap);
					};
					var onSuccess = function(data) {
						// debug(['page ', i].join(''));
						if(data) {
							$(".beatmapListing div.beatmap", data).each(parseBlock);
						} else {
							// debug('error : ' + i);
						}
						list[i - 1] = pageList;
						if(currentPage++ >= maxIndex) {
							doneCallback(list);
						}
					};
					var onError = function() {
						if(tryTime >= 3) {
							onSuccess();
						} else {
							get();
						}
					};
					var get = function() {
						tryTime++;
						$.ajax({
							url: [url, i].join(''),
							success: onSuccess,
							error: onError,
						});
					};
					get();
				})(i);
			}
		};

		getList(beatmap_url, beatmap_index, function(result) {
			beatmapList = Array.prototype.concat.apply(beatmapList, result);
			onAllDone(beatmapList);
		});
	};
	w.ListParser = {
		parseList: main,
	}
})(window)