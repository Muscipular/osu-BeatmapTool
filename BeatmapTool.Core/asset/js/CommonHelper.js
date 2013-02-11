(function(w) {
	var ClassCreator = function(fields, prototype, static) {
		var getClass = function() {
			return function() {
				if(fields instanceof Array) {
					for(var i = 0, max = fields.length; i < max; i++) {
						this[fields[i]] = null;
					}
				} else {
					for(var field in fields) {
						this[field] = fields[field];
					}
				}
				var args = Array.prototype.slice.call(arguments);
				this.ctor.apply(this, args);
			}
		}
		var Class = getClass();
		prototype = prototype ? prototype : {};
		if(!prototype.ctor) {
			prototype.ctor = function() {};
		}
		if(static) {
			for(var field in static) {
				Class[field] = static[field];
			}
		}
		Class.prototype = prototype;
		return Class;
	}

	var Extend = function(target, source, overWrite) {
		if(undefined == overWrite) {
			overWrite = true;
		}
		for(var field in source) {
			if(overWrite || !(field in target)) {
				target[field] = source[field];
			}
		}
		return target;
	}

	var ExtendPrototype = function(targetClass, source, overWrite) {
		if(undefined == overWrite) {
			overWrite = false;
		}
		var target = targetClass.prototype;
		for(var field in source) {
			if(overWrite || !(field in target)) {
				target[field] = source[field];
			}
		}
		return target;
	}

	var FindBy = function(array, field, expression, sorted, desc) {
		if(!(expression instanceof Function)) {
			expression = (function(value) {
				return function(obj, fieldValue) {
					return obj && obj[field] == value;
				}
			})(expression);
		}

		if(array.length == 0) {
			return null;
		}
		var enableSort = sorted;
		if(sorted) {
			enableSort = expression(array[0], array[0][field]);
			enableSort = (enableSort instanceof Number || typeof enableSort === 'number');
		}

		function find(start, end) {
			if(start > end) {
				return null;
			}
			var m = (start + end) >> 1;
			var r = expression(array[m], array[m][field]);
			if(r < 0) {
				return desc ? find(start, m - 1) : find(m + 1, end);
			} else if(r > 0) {
				return desc ? find(m + 1, end) : find(start, m - 1);
			}
			return array[m];
		}
		if(sorted && enableSort) {
			return find(0, array.length - 1);
		}
		for(var i = array.length - 1, item; i >= 0; i--) {
			if((item = array[i]) && expression(item, item[field])) {
				return item;
			}
		}
		return null;
	}
	w.CommonHelper = {
		Create: ClassCreator,
		Extend: Extend,
		ExtendPrototype: ExtendPrototype,
		ArrayFindBy: FindBy,
	}
})(window);