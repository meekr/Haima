using UnityEngine;
using System.Collections;
using System.Xml;

public class SubMenu : SelectableList {
	
	private float _menuHeight	= 44.0f;
	private float _menuWidth	= 1024.0f;
	
	XmlDocument xmlDoc;
	private ArrayList pages = new ArrayList();
	
	private Sprite	_bg;
	private SubMenuBg _subMenuBg;
	private XmlNodeList _nodes;
	private MenuElement _menuElement;
	
	public SubMenu (XmlNodeList nodes) {
		_nodes = nodes;
		
		_bg	= new Sprite(Resources.Load("mainmenu/component/subMenuBg", typeof(Texture2D)) as Texture2D);
		_bg.width	= _menuWidth;
		_bg.height	= _menuHeight;
		addChild(_bg);
		
		_subMenuBg = new SubMenuBg();
		_subMenuBg.y = 6;
		addChild(_subMenuBg);
		
		for (int i=0; i<nodes.Count; i++){
			GuiMenuItem item = new GuiMenuItem();
			item.selectedTexture	= Resources.Load("mainmenu/sub/"+nodes[i].Attributes["icon"].Value, typeof(Texture2D)) as Texture2D;
			item.defaultTexture		= Resources.Load("mainmenu/sub/"+nodes[i].Attributes["icon"].Value, typeof(Texture2D)) as Texture2D;
			addItem(item);
		}
	}
	
	public void populateContent(MenuElement menuElement){
		for (int i=0; i<_nodes.Count; i++){
			InteractImagePage page	= new InteractImagePage(_nodes[i], menuElement);
			pages.Add(page);
		}
	}
	
	private float _currentX = 10;
	override public void addItem(SelectableItem item){
		base.addItem(item);
		item.x	= _currentX;
		item.y	= 5;
		item.id	= "subitem-" + (item.listIndex);
		item.addEventListner(MouseEvent.MOUSE_DOWN, new EventDispatcher.CallBack(itemMouseDownHandler));
		
		_currentX += (item as GuiMenuItem).defaultTexture.width + 20;
	}
	
	protected void itemMouseDownHandler(GuiEvent e){
		SelectableItem item = e.target as SelectableItem;
		selectItem(item);
		_subMenuBg.show(item.x, item.width);
	}
	
	public void hide(){
		unselectItem();
		NanoTween.to(this, 0.4f, NanoTween.Pack("y", 685f, "ease", Ease.easeOutExpo, NanoTween.ON_COMPLETE,new NanoTween.CallBack(hideCompleteCallBack)));
	}
	
	public void hideCompleteCallBack(object[] args){
		//Stage.instance.removeChild(this);
		this.destroy();
		//Resources.UnloadUnusedAssets();
	}
	
	public Page GetPageAtIndex(int index){
		//Debug.Log("curent index="+index+", total="+pages.Count);
		if (pages[index] == null){
			InteractImagePage page	= new InteractImagePage(_nodes[index], _menuElement);
			pages[index] = page;
		}
		return pages[index] as Page;
	}
}