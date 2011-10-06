using UnityEngine;
using System.Collections;
using System.Xml;

public class TimeLinePage : Page {

	private float _currentZ			= 0.0f;
	private ArrayList _itemImgAry	= new ArrayList();
	private ArrayList _itemTxtAry	= new ArrayList();
	private float _itemImgWidth		= 640f;
	private float _itemImgHeight	= 480f;
	private float _focalLength		= 100f;
	
	private	int _totalItemNum;
	private int _totalHeight;
	private string _pathPrefix		= "page/timeline/";
	private MenuElement _menuElement;
	
	public TimeLinePage (XmlNodeList milestones, MenuElement menuElement) {
		_totalItemNum	= milestones.Count;
		_totalHeight	= (_totalItemNum - 1) * 25;
		_menuElement	= menuElement;
		
		Texture2D texture	= _menuElement.GetTextureById("timelinebg");
		setTexutre(texture, 1024f, 686f);
		for (int i=0; i<_totalItemNum; i++){
			TimeLineItem item = new TimeLineItem(_menuElement);
			item.setLabel(_menuElement.GetTextureById(milestones[i].Attributes["time"].Value));
			
			addItem(item);
			
			Sprite itemImg	= new Sprite(_menuElement.GetTextureById(milestones[i].Attributes["pic"].Value));
			_itemImgAry.Add(itemImg);
			addChild(itemImg);
			
			Sprite itemTxt	= new Sprite(_menuElement.GetTextureById(milestones[i].Attributes["text"].Value));
			_itemTxtAry.Add(itemTxt);
			addChild(itemTxt);
			itemTxt.alpha	= 0.0f;
			itemTxt.y		= 640f;
			itemTxt.x		= (_texture.width-itemTxt.width)/2-20;
		}
		this.addEventListner(GuiEvent.CHANGE, new EventDispatcher.CallBack(selectChangeHandler));
		//this.addEventListner(GuiEvent.ENTER_FRAME,new EventDispatcher.CallBack(enterFrameHandler));
		selectItemByIndex(_totalItemNum-1);
	}
	
	public override void render()
	{
		base.render ();
		if (_selectedItem!=null){
			_currentZ += (_totalHeight-_selectedItem.listIndex*25-_currentZ)/16;
		}
		for (int i=0; i<_totalItemNum; i++){
			Sprite img = _itemImgAry[i] as Sprite;
			
			float imgZ	= (_totalItemNum-i-1)*25 - _currentZ;
			float scale	= _focalLength/(imgZ+_focalLength);
			img.width	= _itemImgWidth*scale;
			img.height	= _itemImgHeight*scale;
			img.x		= (_texture.width-img.width)/2-20;
			img.y		= (_texture.height-img.height)/2 + 350*scale-350;
			if(imgZ>=0){
				img.alpha	= (_totalHeight-imgZ)/_totalHeight;
				img.mouseEnable= true;
			}else{
				img.alpha	= (20+imgZ)/20;
				img.mouseEnable= false;
			}
		}
	}
	
	private void enterFrameHandler(GuiEvent e){
		/*if(stage!=null){
			if(stage.mouseX!=0 && stage.mouseY!=0){
				foreach(SelectableItem item in _itemAry){
					if(item.hittest()
				}
			}
		}*/
	}
	
	private void selectChangeHandler(GuiEvent e){
		if (_lastSelectedItem != null){
			Sprite txt	= _itemTxtAry[_lastSelectedItem.listIndex] as Sprite;
			NanoTween.to(txt,0.6f,NanoTween.Pack("alpha",0.0f,"y",640f,"ease",Ease.easeOutExpo));
		}
		Sprite currenttxt	= _itemTxtAry[_selectedItem.listIndex] as Sprite;
		NanoTween.to(currenttxt,0.6f,NanoTween.Pack("alpha",1.0f,"y",600f,"ease",Ease.easeOutExpo));
	}
	                              
	private void itemClickHandler(GuiEvent e){
		selectItem(e.target as SelectableItem);
	}
	
	
	public override void addItem (SelectableItem item)
	{
		base.addItem (item);
		item.addEventListner(TouchEvent.TOUCH_MOVED, itemClickHandler);
		item.addEventListner(TouchEvent.TOUCH_STATIONARY, itemClickHandler);
		item.addEventListner(TouchEvent.TOUCH_BEGAN, itemClickHandler);
		
		item.x	= Stage.instance.stageWidth-120;
		item.y	= (_texture.height-_totalItemNum*50)/2 + item.listIndex*50;
	}
}