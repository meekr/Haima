using UnityEngine;
using System.Collections;
using System.Xml;
using System.Threading;

public class MainScript : MonoBehaviour {
	
	private MainMenu _mainmMenu;
	private SubMenu _subMenu;
	private XmlDocument _xmlDoc;
	
	private float _mainMenuY = 685f;
	private float _subMenuHeight = 42f;
	
	private Page _lastPage;
	private MenuElement _currentMenuElement;
	private MenuElement _previousMenuElement;
	
	private SceneScript _sceneScript;
	
	IEnumerator Start () {
		_sceneScript = GetComponent("SceneScript") as SceneScript;
		
		TextAsset data = (TextAsset)Resources.Load("mainMenu/menu", typeof(TextAsset));
		_xmlDoc = new XmlDocument();
		_xmlDoc.LoadXml(data.text);
		XmlNodeList nodes = _xmlDoc.DocumentElement.SelectNodes("item");
		
		_mainmMenu	= new MainMenu(nodes);
		Stage.instance.addChild(_mainmMenu);
		_mainmMenu.y = _mainMenuY;
		_mainmMenu.addEventListner(GuiEvent.CHANGE, new EventDispatcher.CallBack(mainMenuChangeHandler));
		
		// pre-generate sub menus to boost performance in realtime
//		Thread t = new Thread(delegate(){
//			
//		});
//		t.Start();
		
		//Debug.Log("start time: "+System.DateTime.Now.ToString("yyyyMMddHHmmss"));
		
		XmlNodeList items = _xmlDoc.DocumentElement.SelectNodes("item");
		for (int i=0; i<items.Count; i++){
			if (items[i].Attributes["assetBundle"] != null){
				string assetBundle = items[i].Attributes["assetBundle"].Value;
				WWW www = new WWW(MenuGenerator.AssetbundleBaseURL + assetBundle + ".assetbundle");
				Debug.Log(assetBundle);
				yield return www;
				MenuGenerator.AddWWW(assetBundle, www);
				
				//MenuElement menuElement = MenuGenerator.GetMenuElement(items[i]);
				//yield return menuElement.IsLoaded;
			}
		}
		//Debug.Log("end time: "+System.DateTime.Now.ToString("yyyyMMddHHmmss"));
	}
	
	void mainMenuChangeHandler(GuiEvent e){
		hidePage();
		hideSubMenu();
		
		_previousMenuElement = _currentMenuElement;
		if (_currentMenuElement != null){
			StartCoroutine(unloadSubMenuContent(_currentMenuElement));
		}
			
		XmlNode node = _xmlDoc.DocumentElement.SelectSingleNode("item["+(_mainmMenu.selectedItem.listIndex+1)+"]");
		if (node.Attributes["assetBundle"] != null){
			_currentMenuElement = MenuGenerator.GetMenuElement(node);
		}
		else
			_currentMenuElement = null;
		
		XmlNodeList nodes = _xmlDoc.DocumentElement.SelectNodes("item["+(_mainmMenu.selectedItem.listIndex+1)+"]/sub");
		if (_mainmMenu.selectedItem != null){
			_mainmMenu.selectedItem.tag = _currentMenuElement;
			if (nodes.Count > 0){
				_subMenu = new SubMenu(nodes);
				Stage.instance.addChildAt(0, _subMenu);
				_subMenu.y	= _mainMenuY;
				_subMenu.addEventListner(GuiEvent.CHANGE, new EventDispatcher.CallBack(subMenuChangeHandler));
				NanoTween.to(_subMenu, 0.3f, NanoTween.Pack("y", _mainMenuY-_subMenuHeight, "ease", Ease.easeOutExpo));
				
				// populate subMenu content
				StartCoroutine(loadSubMenuContent(_subMenu, _currentMenuElement));
			}
			else if (_currentMenuElement != null){
				_sceneScript.hideScene();
				
				// populate this menu content
				StartCoroutine(showMenuContent(node, _currentMenuElement));
			}
			else if (node.Attributes["iOSNativeCode"] != null){
				_sceneScript.hideScene();
			}
		}
	}
	
	IEnumerator loadSubMenuContent(SubMenu subMenu, MenuElement menuElement){
		if (!menuElement.IsLoaded){
			//Debug.Log("wait 1 sec");
			yield return 1;
		}
		//Debug.Log("loaded = "+menuElement.IsLoaded);
		subMenu.populateContent(menuElement);
	}
	
	IEnumerator unloadSubMenuContent(MenuElement menuElement){
		if (menuElement != null){
			menuElement.Unload();
			Debug.Log("delta time: "+Time.deltaTime);
		}
		yield return 0;
	}
	
	IEnumerator showMenuContent(XmlNode node, MenuElement menuElement){
		if (!menuElement.IsLoaded){
			//Debug.Log("wait 1 sec");
			yield return new WaitForSeconds(1);
		}
		
		_lastPage = new InteractImagePage(node, menuElement);
		_lastPage.show();
	}
	
	void subMenuChangeHandler(GuiEvent e){
		//Debug.Log("sub menu changed");
		if (_lastPage != null){
			_lastPage.hide();
		}

		if (_subMenu.selectedItem != null){
			if (_lastPage == null){
				_sceneScript.hideScene();
				this.enabled = false;
			}
			_lastPage = _subMenu.GetPageAtIndex(_subMenu.selectedItem.listIndex);
			_lastPage.show();
		}
	}
	
	void hideSubMenu(){
		if (_subMenu != null){
			_subMenu.removeEventListner(GuiEvent.CHANGE, new EventDispatcher.CallBack(subMenuChangeHandler));
			_subMenu.hide();
			_subMenu = null;
		}
	}
	
	void hidePage(){
		if (_lastPage != null){
			_lastPage.hide();
			_lastPage = null;
		}
		_sceneScript.showScene();
		this.enabled = true;
	}
}