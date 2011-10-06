using UnityEngine;
using System.Collections;

public class Page : SelectableList {

	// Use this for initialization
	public Page (Texture2D texture):base(texture) {
		_alpha	= 0;
		//_scaleX	= 1.2f;
		//_scaleY	= 1.2f;
	}
	public Page(){
	}
	
	public void show(){
		NanoTween.to(this, 0.4f, NanoTween.Pack("alpha", 1.0f));
		Stage.instance.addChildAt(0, this);
	}
	public void hide(){
		NanoTween.to(this, 0.4f, NanoTween.Pack("alpha",0.0f,NanoTween.ON_COMPLETE,new NanoTween.CallBack(hideCompleteCallBack)));
		
	}
	public void hideCompleteCallBack(object[] args){
		Stage.instance.removeChild(this);
		//this.destroy();
		//Resources.UnloadUnusedAssets();
	}
}