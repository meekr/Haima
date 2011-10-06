using UnityEngine;
using System.Collections;
using System.Xml;
using System;

public enum PageType{
	Single		= 1,
	Zoomable	= 2,
	Gallary 	= 3,
	GallaryHome	= 4,
	Hotspot		= 5,
	Playable	= 6,
	Timeline	= 7,
	Fadein		= 8
};

public enum ZoomableContentType{
	Image = 1,
	Video = 2
}

public class InteractImagePage : Page{
	
	private float _screenWidth = 1024;
	private PageType _pageType = PageType.Single;
	private XmlNode _node;
	private MenuElement _menuElement;
	
//	public InteractImagePage(XmlNode node)
//		: this(node, null)
//	{
//	}
	
	public InteractImagePage(XmlNode node, MenuElement menuElement) {
		_node = node;
		_menuElement = menuElement;
		
		buildBg();
		if (node.Attributes["type"] != null){
			_pageType = (PageType)Enum.Parse(typeof(PageType), node.Attributes["type"].Value);
			switch (_pageType){
				case PageType.Zoomable:
					buildZoomable();
					break;
				case PageType.Gallary:
					buildGallary();
					break;
				case PageType.GallaryHome:
					break;
				case PageType.Hotspot:
					buildHotspot();
					break;
				case PageType.Playable:
					buildPlayable();
					break;
				case PageType.Timeline:
					buildTimeline();
					break;
				case PageType.Fadein:
					buildFadein();
					break;
			}
		}
	}
	
	public PageType PageType{
		get { return _pageType; }
	}
	
	void buildBg(){
		XmlNode bg = _node.SelectSingleNode("component");
		if (bg != null){
			Texture2D texture = Resources.Load("page/pbg", typeof(Texture2D)) as Texture2D;
			setTexutre(texture, 1024f, 686f);
			XmlNodeList cs = bg.SelectNodes("c");
			foreach (XmlNode c in cs){
				Texture2D tex = _menuElement.GetTextureById(c.Attributes["src"].Value);
				float x = float.Parse(c.Attributes["x"].Value);
				float y = float.Parse(c.Attributes["y"].Value);
				float w = float.Parse(c.Attributes["w"].Value);
				float h = float.Parse(c.Attributes["h"].Value);
				Sprite s = new Sprite(tex, w, h);
				s.x = x;
				s.y = y;
				addChild(s);
			}
		}
		else if (_node.Attributes["src"] != null){
			Texture2D texture = _menuElement.GetTextureById(_node.Attributes["src"].Value);
			setTexutre(texture, 1024f, 686f);
		}
	}
	
	#region zoomable
	private Vector2[] _clipPos;
	
	protected void buildZoomable(){
		XmlNodeList imgs = _node.SelectNodes("img");
		_clipPos = new Vector2[imgs.Count];
		for (int i=0; i<_clipPos.Length; i++){
			float x = float.Parse(imgs[i].Attributes["x"].Value);
			float y = float.Parse(imgs[i].Attributes["y"].Value);
			
			_clipPos[i] = new Vector2(x, y);
			
			
			InteractImageClip clip	= new InteractImageClip(_menuElement.GetTextureById(imgs[i].Attributes["src"].Value));
			addZoomableItem(clip);
			float w = float.Parse(imgs[i].Attributes["width"].Value);
			float h = float.Parse(imgs[i].Attributes["height"].Value);
			clip.x	= x;
			clip.y	= y;
			clip.width	= w;
			clip.height	= h;
			clip.tag = w+","+h;
			
			if (imgs[i].Attributes["type"] != null){
				ZoomableContentType type = (ZoomableContentType)Enum.Parse(typeof(ZoomableContentType), imgs[i].Attributes["type"].Value);
				clip.type = type;
				
				if (imgs[i].Attributes["videoSrc"] != null){
					clip.videoSrc = imgs[i].Attributes["videoSrc"].Value;
				}
			}
		}
		
		XmlNodeList vids = _node.SelectNodes("video");
		for (int i=0; i<vids.Count; i++){
			Texture2D tex = Resources.Load("page/blank", typeof(Texture2D)) as Texture2D;
			Sprite sprite = new Sprite(tex);
			sprite.x		= float.Parse(vids[i].Attributes["x"].Value);
			sprite.y		= float.Parse(vids[i].Attributes["y"].Value);
			sprite.width	= float.Parse(vids[i].Attributes["width"].Value);
			sprite.height	= float.Parse(vids[i].Attributes["height"].Value);
			sprite.tag		= vids[i].Attributes["src"].Value;
			addChild(sprite);
			sprite.addEventListner(MouseEvent.MOUSE_DOWN, new EventDispatcher.CallBack(playableClicked));
		}
	}
	
	void addZoomableItem (SelectableItem item)
	{
		base.addItem (item);
		item.addEventListner(MouseEvent.MOUSE_DOWN, zoomableItemClickHandler);
	}
	
	void zoomableItemClickHandler(GuiEvent e){
		InteractImageClip clip = e.target as InteractImageClip;
		
		Stage.instance.addChild(clip);
		if (!clip.isSelected){
			NanoTween.to(clip, .6f, NanoTween.Pack("x", 0.0f,
			                                       "y", 0.0f,
			                                       "width", 1024f,
			                                       "height", 768f,
			                                       "ease", Ease.easeOutExpo,
			                                       "onComplete", new NanoTween.CallBack(zoomOutComplete),
			                                       "onCompleteParams", NanoTween.Pack(clip)));
			selectItem(clip);
		}
		else{
			string tag = clip.tag as string;
			float w = float.Parse(tag.Split(',')[0]);
			float h = float.Parse(tag.Split(',')[1]);
			NanoTween.to(clip, .6f, NanoTween.Pack("x", _clipPos[clip.listIndex].x,
			                                       "y", _clipPos[clip.listIndex].y,
			                                       "width", w,
			                                       "height", h,
			                                       "ease",Ease.easeOutExpo));
			addChild(clip);
			unselectItem();
		}
	}
	
	void zoomOutComplete(object[] args){
		InteractImageClip clip = args[0] as InteractImageClip;
		if (clip.type == ZoomableContentType.Video)
			iPhoneUtils.PlayMovie(clip.videoSrc, Color.clear, iPhoneMovieControlMode.Full, iPhoneMovieScalingMode.None);
	}
	#endregion
	
	#region gallary items
	private SelectableItem[] _gallaryItems;
	private int _currentGallaryItemIndex = 0;
	private Sprite _left, _right;
	protected void buildGallary(){
		XmlNodeList imgs = _node.SelectNodes("img");
		_gallaryItems = new SelectableItem[imgs.Count];
		for (int i=0; i<Math.Min(2, imgs.Count); i++){
			PageType subType = PageType.Single;
			if (imgs[i].Attributes["type"] != null)
				subType = (PageType)Enum.Parse(typeof(PageType), imgs[i].Attributes["type"].Value);
			
			if (subType != PageType.Single){
				_gallaryItems[i] = new InteractImagePage(imgs[i], _menuElement);
			}
			else{
				_gallaryItems[i] = new SelectableItemEx(imgs[i], _menuElement);	//,1024f,668f);
			}
			
			_gallaryItems[i].x = (i==0) ? 0 : _screenWidth;
			_gallaryItems[i].id = "gallaryItem-" + i;
			addGallaryItem(_gallaryItems[i]);
		}
		
		if (imgs.Count > 0){
			Texture2D arrow = Resources.Load("mainMenu/component/arrow-left", typeof(Texture2D)) as Texture2D;
			_left = new SelectableItem(arrow);
			_left.alpha = 0;
			_left.x = 0;
			_left.y = (668-arrow.height)/2f;
			_left.id = "gallary-left-arrow";
			_left.addEventListner(MouseEvent.MOUSE_DOWN, new EventDispatcher.CallBack(leftArrowClicked));
			base.addChild(_left);
			
			arrow = Resources.Load("mainMenu/component/arrow-right", typeof(Texture2D)) as Texture2D;
			_right = new SelectableItem(arrow);
			_right.alpha = 1f;
			_right.x = _screenWidth-arrow.width;
			_right.y = (668-arrow.height)/2f;
			_right.id = "gallary-right-arrow";
			_right.addEventListner(MouseEvent.MOUSE_DOWN, new EventDispatcher.CallBack(rightArrowClicked));
			base.addChild(_right);
		}
	}
	
	void addGallaryItem(SelectableItem item){
		base.addItem(item);
		setChildIndex(item, 0);
		item.height = 686;
		item.addEventListner(GestureEvent.SWIPE, new EventDispatcher.CallBack(gallaryItemTouchEnded));
	}
	
	void gallaryItemTouchEnded(GuiEvent e){
		GestureEvent ge = e as GestureEvent;
		if (ge.swipeDirection == "left")
			rightArrowClicked(null);
		else
			leftArrowClicked(null);
	}
	
	void leftArrowClicked(GuiEvent e){
		if (_currentGallaryItemIndex == 0) return;
		
		NanoTween.to(getGallaryItem(_currentGallaryItemIndex), .6f, NanoTween.Pack("x", _screenWidth, "ease", Ease.easeOutExpo));
		_currentGallaryItemIndex--;
		NanoTween.to(getGallaryItem(_currentGallaryItemIndex), .6f, NanoTween.Pack("x", 0f, "ease", Ease.easeOutExpo));
		
		if (_currentGallaryItemIndex == 0)
			NanoTween.to(_left, .6f, NanoTween.Pack("alpha", 0f, "ease", Ease.easeOutExpo));
		if (_currentGallaryItemIndex < _gallaryItems.Length - 1)
			NanoTween.to(_right, .6f, NanoTween.Pack("alpha", 1f, "ease", Ease.easeOutExpo));
		
		// clean far away image
		//cleanFarItems();
	}
	
	void rightArrowClicked(GuiEvent e){
		if (_currentGallaryItemIndex == _gallaryItems.Length-1) return;
		
		NanoTween.to(getGallaryItem(_currentGallaryItemIndex), .6f, NanoTween.Pack("x", -_screenWidth, "ease", Ease.easeOutExpo));
		_currentGallaryItemIndex++;
		NanoTween.to(getGallaryItem(_currentGallaryItemIndex), .6f, NanoTween.Pack("x", 0f, "ease", Ease.easeOutExpo));
		
		
		if (_currentGallaryItemIndex == _gallaryItems.Length-1)
			NanoTween.to(_right, .6f, NanoTween.Pack("alpha", 0f, "ease", Ease.easeOutExpo));
		if (_currentGallaryItemIndex == 1)
			NanoTween.to(_left, .6f, NanoTween.Pack("alpha", 1f, "ease", Ease.easeOutExpo));
		
		// preload next image
		getGallaryItem(_currentGallaryItemIndex+1);
		// clean far away image
		//cleanFarItems();
	}
	
	void cleanFarItems(){
		bool needClean = false;
		for (int i=0; i<_gallaryItems.Length; i++){
			if (i<_currentGallaryItemIndex-1 || i>_currentGallaryItemIndex+1){
				cleanGallaryItem(i);
				needClean = true;
			}
		}
		if (needClean){
			//Debug.Log("unload resource...");
			Resources.UnloadUnusedAssets();
		}
	}
	
	SelectableItem getGallaryItem(int index){
		if (index < 0 || index >= _gallaryItems.Length)
			return null;
		
		if (_gallaryItems[index] == null){
			//Debug.Log("get gallary item: "+index);
			XmlNodeList imgs = _node.SelectNodes("img");
			PageType subType = PageType.Single;
			if (imgs[index].Attributes["type"] != null)
				subType = (PageType)Enum.Parse(typeof(PageType), imgs[index].Attributes["type"].Value);
			
			if (subType != PageType.Single)
				_gallaryItems[index] = new InteractImagePage(imgs[index], _menuElement);
			else
				_gallaryItems[index] = new SelectableItemEx(imgs[index], _menuElement);
			_gallaryItems[index].x = (index==_currentGallaryItemIndex) ? 0 : _screenWidth;
			_gallaryItems[index].id = "gallaryItem-" + index;
			addGallaryItem(_gallaryItems[index]);
		}
		return _gallaryItems[index];
	}
	
	void cleanGallaryItem(int index){
		if (index < 0 || index >= _gallaryItems.Length)
			return;
		
		SelectableItem item = _gallaryItems[index] as SelectableItem;
		if (item != null){
			_gallaryItems[index] = null;
			removeItem(item);
		}
	}
	#endregion
	
	#region hotspot
	private PopupImageClip[] _spotImages;
	private Sprite[] _spots;
	private int _selectedSpotIndex = -1;
	
	void buildHotspot(){
		this.addEventListner(MouseEvent.MOUSE_DOWN, new EventDispatcher.CallBack(hotspotSceneClicked));
		
		XmlNodeList nodes = _node.SelectNodes("spot");
		_spots = new Sprite[nodes.Count];
		_spotImages = new PopupImageClip[nodes.Count];
		
		for (int i=0; i<nodes.Count; i++){
			Hotspot spot = new Hotspot();
			spot.x	= float.Parse(nodes[i].Attributes["x"].Value)-22;
			spot.y	= float.Parse(nodes[i].Attributes["y"].Value)-22;
			spot.id	= "spot-"+i;
			spot.tag= i;
			spot.addEventListner(MouseEvent.MOUSE_DOWN, new EventDispatcher.CallBack(hotspotClicked));
			addChild(spot);
			spot.startAnimation();
			_spots[i] = spot;
			
			Texture2D texture = _menuElement.GetTextureById(nodes[i].Attributes["src"].Value);
			PopupImageClip image = new PopupImageClip(texture);
			image.alpha = 0;
			image.scaleX = image.scaleY = 0f;
			image.x = spot.x;
			image.y = spot.y;
			image.id = "spot-image-"+i;
			image.tag= i;
			image.addEventListner(MouseEvent.MOUSE_DOWN, new EventDispatcher.CallBack(hotspotImageClicked));
			addChild(image);
			_spotImages[i] = image;
		}
	}
	
	void hotspotSceneClicked(GuiEvent e){
		if (_selectedSpotIndex > -1){
			if (e == null){
				Sprite spot = _spots[_selectedSpotIndex];
				PopupImageClip image = _spotImages[_selectedSpotIndex];
				NanoTween.to(image, .6f, NanoTween.Pack("alpha", 0f, "scaleX", 0f, "scaleY", 0f, "x", spot.x, "y", spot.y, "ease", Ease.easeOutExpo));
			}
		}
	}
	
	void hotspotClicked(GuiEvent e){
		hotspotSceneClicked(null);
		
		Sprite spot = e.target as Sprite;
		float centerX = spot.x - spot.texture.width/2f;
		float centerY = spot.y - spot.texture.height/2f;
		int id = (int)spot.tag;
		Texture tex = _spotImages[id].texture;
		
		
		float gap = (tex.width == 1024) ? 0 : 10;
		float x = Mathf.Max(gap, centerX-tex.width/2f);
		x = Mathf.Min(x, _screenWidth-tex.width-gap);
		float y = Mathf.Max(gap, centerY-tex.height/2f);
		y = Mathf.Min(y, 688-tex.height-gap);
		
	
		if (tex.width == 1024){
			//Debug.Log("tex.width="+tex.width);
			Stage.instance.addChild(_spotImages[id]);
			Stage.instance.setChildIndex(_spotImages[id], Stage.instance.childList.Count-1);
			NanoTween.to(_spotImages[id], .6f, NanoTween.Pack("alpha", 1f, "width", 1024f, "height", 768f, "x", 0f, "y", 0f, "ease", Ease.easeOutExpo));
		}
		else{
			setChildIndex(_spotImages[id], childList.Count-1);
			NanoTween.to(_spotImages[id], .6f, NanoTween.Pack("alpha", 1f, "scaleX", 1f, "scaleY", 1f, "x", x, "y", y, "ease", Ease.easeOutExpo));
		}
		
		_selectedSpotIndex = id;
	}
	
	void hotspotImageClicked(GuiEvent e){
		hotspotSceneClicked(null);
		
		Sprite image = e.target as Sprite;
		Sprite spot = _spots[(int)image.tag];
		NanoTween.to(e.target, .6f, NanoTween.Pack("alpha", 0f, "scaleX", 0f, "scaleY", 0f, "x", spot.x, "y", spot.y, "ease", Ease.easeOutExpo));
	}
	#endregion
	
	#region playable
	void buildPlayable(){
		XmlNodeList areas = _node.SelectNodes("area");
		for (int i=0; i<areas.Count; i++){
			
			Texture2D tex = Resources.Load("page/blank", typeof(Texture2D)) as Texture2D;
			Sprite sprite = new Sprite(tex);
			sprite.x		= float.Parse(areas[i].Attributes["x"].Value);
			sprite.y		= float.Parse(areas[i].Attributes["y"].Value);
			sprite.width	= float.Parse(areas[i].Attributes["width"].Value);
			sprite.height	= float.Parse(areas[i].Attributes["height"].Value);
			sprite.tag		= areas[i].Attributes["videoSrc"].Value;
			addChild(sprite);
			sprite.addEventListner(MouseEvent.MOUSE_DOWN, new EventDispatcher.CallBack(playableClicked));
		}
	}
	
	void playableClicked(GuiEvent e){
		Sprite sprite = e.target as Sprite;
		iPhoneUtils.PlayMovie((sprite.tag as  string), Color.clear, iPhoneMovieControlMode.Full, iPhoneMovieScalingMode.None);
	}
	#endregion
	
	#region timeline
	void buildTimeline(){
		XmlNodeList milestones = _node.SelectNodes("ms");
		TimeLinePage page = new TimeLinePage(milestones, _menuElement);
		addChild(page);
	}
	#endregion
	
	#region fade in
	void buildFadein(){
		XmlNodeList nodes = _node.SelectNodes("img");
		for (int i=0; i<nodes.Count; i++){
			Sprite sprite = new Sprite(_menuElement.GetTextureById(nodes[i].Attributes["src"].Value));
			sprite.alpha	= 0f;
			sprite.x		= float.Parse(nodes[i].Attributes["x"].Value);
			sprite.y		= float.Parse(nodes[i].Attributes["y"].Value);
			sprite.width	= float.Parse(nodes[i].Attributes["w"].Value);
			sprite.height	= float.Parse(nodes[i].Attributes["h"].Value);
			addChild(sprite);
			//sprite.addEventListner(MouseEvent.MOUSE_DOWN, new EventDispatcher.CallBack(fadeInClicked));
			fadeInHandler(new object[]{sprite});
		}
	}
	
	void fadeOutHandler(object[] args){
		Sprite sprite = args[0] as Sprite;
		NanoTween.to(sprite, .6f, NanoTween.Pack("alpha", 0f,
		                                           "ease", Ease.easeOutExpo,
			                                       "onComplete", new NanoTween.CallBack(fadeInHandler),
		                                         "onCompleteParams", NanoTween.Pack(sprite)));
	}
	
	void fadeInHandler(object[] args){
		Sprite sprite = args[0] as Sprite;
		NanoTween.to(sprite, .6f, NanoTween.Pack("alpha", 1f,
		                                           "ease", Ease.easeOutExpo,
			                                       "onComplete", new NanoTween.CallBack(fadeOutHandler),
		                                         "onCompleteParams", NanoTween.Pack(sprite)));
	}
	
	void fadeInClicked(GuiEvent e){
		Sprite sprite = e.target as Sprite;
		if (sprite.tag == null){
			NanoTween.to(e.target, .6f, NanoTween.Pack("alpha", 1f, "ease", Ease.easeOutExpo));
			sprite.tag = true;
		}
		else{
			NanoTween.to(e.target, .6f, NanoTween.Pack("alpha", 0f, "ease", Ease.easeOutExpo));
			sprite.tag = null;
		}
	}
	#endregion
}
