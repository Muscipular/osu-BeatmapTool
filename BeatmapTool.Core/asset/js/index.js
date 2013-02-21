var MessageBoxResult = {
	None: 0,
	OK: 1,
	Cancel: 2,
	Yes: 6,
	No: 7,
}

var NavMenu = CommonHelper.Create(['label', 'callback'], {
	ctor: function(label, callback) {
		this.label = label;
		this.callback = callback;
	},
});
var lastPage;
var NavMenus = [
	new NavMenu("恶俗状态", function(item) {
	if(lastPage != this) {
		lastPage = this;
		esu.init();
	}
}), new NavMenu("统计信息", function(item) {
	if(lastPage != this) {
		lastPage = this;
		beatmap_status.init();
	}
}), new NavMenu("本地列表", function(item) {

}), new NavMenu("Rank列表", function(item) {

}), new NavMenu("下载队列", function(item) {

}), new NavMenu("其他", function(item) {

}),
	];

var app = {
	navMenus: NavMenus,
	init: function() {
		$('#main-nav .boxed').html('');
		for(var i = 0; i < NavMenus.length; i++) {
			var o = $("<li></li>");
			var item = NavMenus[i];
			o.attr('id', 'nav-item-' + i);
			o.text(item.label);
			o.click(item.callback);
			$('#main-nav .boxed').append(o);
		};
	},
	rankList = [],
	localList = [],
}

$(document).ready(function() {
	$('#loading-layer').hide();
	app.init();
})