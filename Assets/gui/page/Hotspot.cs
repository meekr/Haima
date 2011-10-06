using UnityEngine;
using System.Collections;

public class Hotspot : Sprite {
	
	private Sprite _outer;
	private float size = 34f;
	
	public Hotspot(){
		_texture = Resources.Load("mainmenu/component/hotspot-in", typeof(Texture2D)) as Texture2D;
		Sprite sprite = new Sprite(_texture);
		addChild(sprite);
		
		Texture2D tex = Resources.Load("mainmenu/component/hotspot-out", typeof(Texture2D)) as Texture2D;
		sprite = new Sprite(tex);
		sprite.x = (_texture.width-tex.width)*.5f;
		sprite.y = (_texture.height-tex.height)*.5f;
		addChild(sprite);
		_outer = sprite;
	}
	
	public void startAnimation(){
		fadeOutHotspot(new object[]{_outer});
	}
	
	void fadeOutHotspot(object[] args){
		_outer.x		= (_texture.width-size)*.5f;
		_outer.y		= (_texture.width-size)*.5f;
		_outer.scaleX	= 1f;
		_outer.scaleY	= 1f;
		_outer.alpha	= 1f;
		
		NanoTween.to(_outer, 1.2f, NanoTween.Pack("alpha", 0f,
		                                       "scaleX", 1.3f,
		                                       "scaleY", 1.3f,
		                                       "x", (_texture.width-size*1.3f)/2f,
		                                       "y", (_texture.height-size*1.3f)/2f,
			                                       "ease", Ease.easeOutExpo,
			                                       "onComplete", new NanoTween.CallBack(fadeOutHotspot)));
	}
	
	void fadeInHotspot(object[] args){
		NanoTween.to(_outer, .6f, NanoTween.Pack("alpha", 1f,
		                                       "scaleX", .9f,
		                                       "scaleY", .9f,
		                                       "x", (_texture.width-size*.9f)/2f,
		                                       "y", (_texture.height-size*.9f)/2f,
			                                       "ease", Ease.easeOutExpo,
			                                       "onComplete", new NanoTween.CallBack(fadeOutHotspot)));
	}
}
