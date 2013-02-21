var esu = {
	_init: false,
	user: {
		userId: '',
		password: ''
	},
	login: function(user, pwd) {
		console.log("userId:" + user + "  pwd:" + pwd);
		$('#loading-layer').show();
		$.post('https://osu.ppy.sh/forum/ucp.php?mode=login', {
			username: user,
			password: pwd,
			autologin: 'on',
			login: "Login",
			redirect: "index.php"
		}, function(data) {
			$('#loading-layer').hide();
			if(!(data.toString().match('Announcements'))) {
				$('#esu-nav-1').show();
				$('#esu-nav-2').hide();
				$APP.window.alert("用户名或密码错误.");
			} else {
				$('#esu-nav-1').hide();
				$('#esu-nav-2').show();
				esu.user.userId = user;
				esu.user.password = pwd;
				$('#esu-userid-2').text(esu.user.userId);
				var src = $('img.mini-avatar', data).attr('src');
				$('#esu-avatar').attr('src', 'http://osu.ppy.sh/' + src);
				// $APP.window.alert("登录成功");
			}
		}, 'html');
	},
	init: function() {
		if(this._init) {
			$('#esu-userid-1').focus();
			return;
		}
		this._init = true;
		$.get("./esu_status.html", function(data) {
			$('#content').append(data);
			$('#esu-userid-1').focus();
			$('#esu-userid-1').keydown(function(e) {
				if(e.keyCode == 13) {
					$('#esu-pwd').focus();
				}
			});
			$('#esu-pwd').keydown(function(e) {
				if(e.keyCode == 13) {
					esu.login($('#esu-userid-1').val(), $('#esu-pwd').val());
				}
			});
			$('#esu-login').click(function() {
				esu.login($('#esu-userid-1').val(), $('#esu-pwd').val());
			});
		}, 'html');
	}
}