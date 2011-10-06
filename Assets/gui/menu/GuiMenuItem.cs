using UnityEngine;
using System.Collections;

public class GuiMenuItem : SelectableItem {
	
	public string iOSNativeCode;
	protected Sprite _selectedBg;
	protected Sprite _defaultBg;
	
	public GuiMenuItem () {
		this.addEventListner(GuiEvent.SELECT,new EventDispatcher.CallBack(selectHandler));
		this.addEventListner(GuiEvent.UNSELECT,new EventDispatcher.CallBack(unselectHandler));
	}
	protected Texture2D	_selectedTexture;
	public Texture2D selectedTexture{
		get {return _selectedTexture;}
		set {_selectedTexture= value;
			_selectedBg	= new Sprite(_selectedTexture);
			_selectedBg.alpha	= 0;
			addChild(_selectedBg);}
	}
	protected Texture2D	_defaultTexture;
	public Texture2D defaultTexture{
		get {return _defaultTexture;}
		set {_defaultTexture= value;
			_defaultBg	= new Sprite(_defaultTexture);
			addChild(_defaultBg);}
	}
	protected void selectHandler(GuiEvent e){
		NanoTween.to(_selectedBg,.3f,NanoTween.Pack("alpha",1.0f));
		NanoTween.to(_defaultBg,.3f,NanoTween.Pack("alpha",0.0f));
	}
	protected void unselectHandler(GuiEvent e){
		NanoTween.to(_selectedBg,.3f,NanoTween.Pack("alpha",0.0f));
		NanoTween.to(_defaultBg,.3f,NanoTween.Pack("alpha",1.0f));
	}
}
