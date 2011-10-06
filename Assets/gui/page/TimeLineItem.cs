using UnityEngine;
using System.Collections;

public class TimeLineItem :SelectableItem {

	private Sprite _bg	;
	public TimeLineItem (MenuElement menuElement) {
		_bg= new Sprite(menuElement.GetTextureById("timelinearrow"));
		_bg.scaleY	= .4f;
		_bg.x		= 80;
		_bg.y		= -5;
		addChild(_bg);
		this.addEventListner(GuiEvent.SELECT,selectHandler);
		this.addEventListner(GuiEvent.UNSELECT,unselectHandler);
	}
	
	private Sprite _label;
	
	public void setLabel(Texture2D labelTexture){
		_label	= new Sprite(labelTexture);
		_label.scaleX = .6f;
		_label.scaleY = .6f;
		_label.y	= -_label.height/2;
		addChild(_label);
	}
	
	
	private void selectHandler(GuiEvent e){
		NanoTween.to(_bg,0.6f,NanoTween.Pack("scaleY",1.0f,"x",60.0f,"y",-12.0f,"ease",Ease.easeOutExpo));
		NanoTween.to(_label,0.6f,NanoTween.Pack("scaleX",0.8f,"scaleY",0.8f,"x",-45.0f,"y",-15.0f,"ease",Ease.easeOutExpo));
	}
	
	private void unselectHandler(GuiEvent e){
		NanoTween.to(_bg,0.6f,NanoTween.Pack("scaleY",0.4f,"x",80.0f,"y",-5.0f,"ease",Ease.easeOutExpo));
		NanoTween.to(_label,0.6f,NanoTween.Pack("scaleX",0.6f,"scaleY",0.6f,"x",0.0f,"y",-12.0f,"ease",Ease.easeOutExpo));
	}
}