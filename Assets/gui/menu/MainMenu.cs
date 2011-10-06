using UnityEngine;
using System;
using System.Collections;
using System.Xml;
using System.Reflection;

public class MainMenu : SelectableList {
	
	private float _menuHeight	= 100.0f;
	private float _menuWidth	= 1024.0f;
	private float _iconWidth	= 100f;
	private float _offsetX		= 22f;
	
	private Sprite	_bg;
	
	
	// Use this for initialization
	public MainMenu (XmlNodeList nodes) {
		
		_bg	= new Sprite(Resources.Load("mainMenu/component/mainMenuBg", typeof(Texture2D)) as Texture2D);
		_bg.width	= _menuWidth;
		_bg.height	= _menuHeight;
		addChild(_bg);
		
		for (int i=0; i<nodes.Count; i++){
			GuiMenuItem item = new GuiMenuItem();
			item.selectedTexture	= Resources.Load("mainMenu/icon/" + nodes[i].Attributes["icon"].Value+"-b", typeof(Texture2D)) as Texture2D;
			item.defaultTexture		= Resources.Load("mainMenu/icon/" + nodes[i].Attributes["icon"].Value, typeof(Texture2D)) as Texture2D;
			if (nodes[i].Attributes["iOSNativeCode"] != null){
				item.iOSNativeCode = nodes[i].Attributes["iOSNativeCode"].Value;
				typeof(UIBinding).InvokeMember("ActivateUI"+item.iOSNativeCode, BindingFlags.Default | BindingFlags.InvokeMethod, null, null, new object[]{ });
				typeof(UIBinding).InvokeMember("DeactivateUI"+item.iOSNativeCode, BindingFlags.Default | BindingFlags.InvokeMethod, null, null, new object[]{ });
			}
			addItem(item);
		}
	}
	
	override public void addItem(SelectableItem item){
		base.addItem(item);
		item.x	= _offsetX + item.listIndex * _iconWidth;
		item.addEventListner(MouseEvent.MOUSE_DOWN, itemMouseDownHandler);
		item.addEventListner(GuiEvent.SELECT, selectHandler);
		item.addEventListner(GuiEvent.UNSELECT, unselectHandler);
	}
	
	protected void itemMouseDownHandler(GuiEvent e){
		selectItem(e.target as SelectableItem);
	}
	
	private void selectHandler(GuiEvent e){
		GuiMenuItem item = e.target as GuiMenuItem;
		if (!string.IsNullOrEmpty(item.iOSNativeCode)){
			typeof(UIBinding).InvokeMember("ActivateUI"+item.iOSNativeCode, BindingFlags.Default | BindingFlags.InvokeMethod, null, null, new object[]{ });
		}
	}
	
	private void unselectHandler(GuiEvent e){
		GuiMenuItem item = e.target as GuiMenuItem;
		if (!string.IsNullOrEmpty(item.iOSNativeCode)){
			typeof(UIBinding).InvokeMember("DeactivateUI"+item.iOSNativeCode, BindingFlags.Default | BindingFlags.InvokeMethod, null, null, new object[]{ });
		}
	}
	
	override public void destroy(){
		foreach (GuiMenuItem item in _itemAry){
			item.removeEventListner(MouseEvent.MOUSE_DOWN, itemMouseDownHandler);
		}
		base.destroy();
	}
}