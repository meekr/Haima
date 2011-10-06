using UnityEngine;
using System.Collections;

public class PopupImageClip : SelectableItem {
	
	private Sprite _bg;
	private Sprite _fg;
	
	public PopupImageClip(Texture texture)
		: base(texture)
	{
		_bg	= new Sprite(Resources.Load("mainmenu/component/white-dot", typeof(Texture2D)) as Texture2D);
		_bg.width = texture.width;
		_bg.height = texture.height;
		addChild(_bg);
		
		_fg = new Sprite(texture);
		_fg.x = 0;
		_fg.y = 0;
		addChild(_fg);
	}
	
	//override public void updateOriginalSize(){
	//}
}
