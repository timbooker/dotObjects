
/*
 * Superfish v1.4.2 - jQuery menu widget
 * Copyright (c) 2008 Joel Birch
 *
 * Dual licensed under the MIT and GPL licenses:
 * 	http://www.opensource.org/licenses/mit-license.php
 * 	http://www.gnu.org/licenses/gpl.html
 *
 * CHANGELOG: http://users.tpg.com.au/j_birch/plugins/superfish/changelog.txt
 */

(function($){
	$.fn.superfish = function(op){
		var bcClass = 'sfbreadcrumb',
			over = function(){
				var $$ = $(this), menu = getMenu($$);
				getOpts(menu,true);
				clearTimeout(menu.sfTimer);
				$$.showSuperfishUl().siblings().hideSuperfishUl();
			},
			out = function(){
				var $$ = $(this), menu = getMenu($$);
				var o = getOpts(menu,true);
				clearTimeout(menu.sfTimer);
				menu.sfTimer=setTimeout(function(){
					o.retainPath=($.inArray($$[0],o.$path)>-1) ? true : false;
					$$.hideSuperfishUl();
					if (o.$path.length && $$.parents('.'+o.hoverClass).length<1){over.call(o.$path);}
				},o.delay);	
			},
			getMenu = function($el){ return $el.parents('ul.sf-js-enabled:first')[0]; },
			getOpts = function(el,menuFound){ el = menuFound ? el : getMenu(el); return $.fn.superfish.op = $.fn.superfish.o[el.serial]; },
			hasUl = function(){ return $.fn.superfish.op.oldJquery ? 'li[ul]' : 'li:has(ul)'; };

		return this.each(function() {
			var s = this.serial = $.fn.superfish.o.length;
			var o = $.extend({},$.fn.superfish.defaults,op);
			o.$path = $('li.'+o.pathClass,this).slice(0,o.pathLevels).each(function(){
				$(this).addClass(o.hoverClass+' '+bcClass)
					.filter(hasUl()).removeClass(o.pathClass);
			});
			$.fn.superfish.o[s] = $.fn.superfish.op = o;
			
			$(hasUl(),this)[($.fn.hoverIntent && !o.disableHI) ? 'hoverIntent' : 'hover'](over,out)
			.not('.'+bcClass)
				.hideSuperfishUl();
			
			var $a = $('a',this);
			$a.each(function(i){
				var $li = $a.eq(i).parents('li');
				$a.eq(i).focus(function(){over.call($li);}).blur(function(){out.call($li);});
			});
			
			o.onInit.call(this);
			
		}).addClass('sf-js-enabled');
	};
	$.fn.extend({
		hideSuperfishUl : function(){
			var o = $.fn.superfish.op;
			var not = (o.retainPath===true) ? o.$path : '';
			o.retainPath = false;
			var $ul = $('li.'+o.hoverClass,this).add(this).not(not).removeClass(o.hoverClass)
					.find('>ul').hide().css('visibility','hidden');
			o.onHide.call($ul);
			return this;
		},
		showSuperfishUl : function(){
			var o = $.fn.superfish.op,
				$ul = this.addClass(o.hoverClass)
					.find('>ul:hidden').css('visibility','visible');
			o.onBeforeShow.call($ul);
			$ul.animate(o.animation,o.speed,function(){ o.onShow.call(this); });
			return this;
		}
	});
	$.fn.superfish.o = [];
	$.fn.superfish.op = {};
	$.fn.superfish.defaults = {
		hoverClass	: 'sfHover',
		pathClass	: 'overideThisToUse',
		pathLevels	: 1,
		delay		: 800,
		animation	: {opacity:'show'},
		speed		: 'normal',
		oldJquery	: false, /* set to true if using jQuery version below 1.2 */
		disableHI	: false, /* set to true to disable hoverIntent usage */
		// callback functions:
		onInit		: function(){},
		onBeforeShow: function(){},
		onShow		: function(){},
		onHide		: function(){}
	};
	$(window).unload(function(){
		$('ul.sf-js-enabled').each(function(){
			$('li,a',this).unbind('mouseover','mouseout','mouseenter','mouseleave','focus','blur');
		});
	});
})(jQuery);
