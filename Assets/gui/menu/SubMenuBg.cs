using UnityEngine;
using System.Collections;

public class SubMenuBg : Sprite {
	
	private Sprite _left;
	private Sprite _middle;
	private Sprite _right;
	
	public SubMenuBg(){
		Texture2D tex	= Resources.Load("mainmenu/component/sub-bg-left", typeof(Texture2D)) as Texture2D;
		_left			= new Sprite(tex);
		_left.width		= 15;
		_left.alpha		= 0;
		addChild(_left);
		
		tex				= Resources.Load("mainmenu/component/sub-bg-middle", typeof(Texture2D)) as Texture2D;
		_middle			= new Sprite(tex);
		_middle.x		= 15;
		_middle.alpha	= 0;
		addChild(_middle);
		
		tex				= Resources.Load("mainmenu/component/sub-bg-right", typeof(Texture2D)) as Texture2D;
		_right			= new Sprite(tex);
		_left.width		= 15;
		_right.alpha	= 0;
		addChild(_right);
	}
	
	public void show(float x, float width){
		//Debug.Log("x="+x+", width="+width);
		
		if (_left.alpha == 0){
			this.x = x;
			_middle.width	= width -_left.width -_right.width;
			_right.x		= width - _right.width;
			
			NanoTween.to(_left, .6f, NanoTween.Pack("alpha", 1f, "ease", Ease.easeOutExpo));
			NanoTween.to(_middle, .6f, NanoTween.Pack("alpha", 1f, "ease", Ease.easeOutExpo));
			NanoTween.to(_right, .6f, NanoTween.Pack("alpha", 1f, "ease", Ease.easeOutExpo));
		}
		else{
			NanoTween.to(this, .6f, NanoTween.Pack("x", x, "ease", Ease.easeOutExpo));
			NanoTween.to(_middle, .6f, NanoTween.Pack("width", width-_left.width-_right.width, "ease", Ease.easeOutExpo));
			NanoTween.to(_right, .6f, NanoTween.Pack("x", width-_right.width, "ease", Ease.easeOutExpo));
		}
	}
}
