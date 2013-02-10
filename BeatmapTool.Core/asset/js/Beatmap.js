(function(w) {
	var fields = ['id', 'title', 'artist', 'creator', 'date', 'genre', 'language', 'pack', 'source', 'diffCount', 'video', 'status']

	var Beatmap = CommonHelper.Create(fields, {
		ctor: function(data) {
			if(!data) return;
			CommonHelper.Extend(this, data);
		},
		toString: function() {
			return JSON.stringify(this);
		},
		compareTo: function(other) {
			return Beatmap.compare(this, other);
		},
		rank: function(flag) {
			var code = Beatmap.StatusCode.Rank;
			if(flag == undefined) {
				return(this.status & code) == code;
			}
			this.status = flag ? this.status | code : this.status & (0xfffffff ^ code);
			return flag;
		},
		approved: function(flag) {
			var code = Beatmap.StatusCode.Approved;
			if(flag == undefined) {
				return(this.status & code) == code;
			}
			this.status = flag ? this.status | code : this.status & (0xfffffff ^ code);
			return flag;
		},
		hidden: function(flag) {
			var code = Beatmap.StatusCode.Hidden;
			if(flag == undefined) {
				return(this.status & code) == code;
			}
			this.status = flag ? this.status | code : this.status & (0xfffffff ^ code);
			return flag;
		},
		equals: function(other) {
			if(other == null || other == undefined) {
				return false;
			}
			if(!(other instanceof Beatmap)) {
				return false;
			}
			for(var i = fields.length - 1; i >= 0; i--) {
				var a = this[fields[i]],
					b = other[fields[i]];
				if(a != b && (fields[i] == 'date' && a.getTime() != b.getTime())) {
					return false;
				}
			}
			return true;
		},
	}, {
		compare: function(a, b) {
			var r = b.date.getTime() - a.date.getTime();
			if(r != 0) {
				return r;
			}
			return b.id - a.id;
		},
		compareById: function(a, b) {
			return b.id - a.id;
		},
		StatusCode: {
			Unrank: 0,
			Rank: 1,
			Approved: 2,
			Hidden: 4,
			GetCode: function(status) {
				status = (status ? status : '').toUpperCase();
				switch(status) {
				case 'UNRANK':
					return this.Unrank;
				case "RANK":
					return this.Rank;
				case 'APPROVED':
					return this.Approved;
				case 'HIDDEN':
					return this.Hidden;
				}
				return this.Unset;
			},
		},
		fields: fields,
	});

	w.Beatmap = Beatmap;
})(window)