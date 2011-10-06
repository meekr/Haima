using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Object=UnityEngine.Object;

[Serializable]
public class MenuElement
{
    public string bundleName;
   
    // The required assets are loaded asynchronously to avoid delays
    // when first using them. A LoadAsync results in an AssetBundleRequest
    // which are stored here so we can check their progress and use the
    // assets they contain once they are loaded.
	private Dictionary<string, AssetBundleRequest> assetRequests = new Dictionary<string, AssetBundleRequest>();
	private List<string> sources = new List<string>();
    
    public MenuElement(XmlNode node)
    {
        this.bundleName = node.Attributes["assetBundle"].Value;
		
		XmlNodeList cs = node.SelectNodes("//c");
		foreach (XmlNode c in cs){
			sources.Add(c.Attributes["src"].Value);
		}
		cs = node.SelectNodes("//img");
		foreach (XmlNode c in cs){
			if (c.Attributes["src"] != null)
				sources.Add(c.Attributes["src"].Value);
		}
		cs = node.SelectNodes("//spot");
		foreach (XmlNode c in cs){
			if (c.Attributes["src"] != null)
				sources.Add(c.Attributes["src"].Value);
		}
		
		// timeline
		cs = node.SelectNodes("//ms");
		if (cs.Count > 0){
			sources.Add("timelinebg");
			sources.Add("timelinearrow");
			foreach (XmlNode c in cs){
				sources.Add(c.Attributes["time"].Value);
				sources.Add(c.Attributes["text"].Value);
				sources.Add(c.Attributes["pic"].Value);
			}
		}
    }
	
	public Texture2D GetTextureById(string id){
		if (!assetRequests.ContainsKey(id)){
			GetRequest(id);
		}
		return assetRequests[id].asset as Texture2D;
	}
	
	void GetRequest(string id){
		AssetBundleRequest req = WWW.assetBundle.LoadAsync(id, typeof(Texture2D));
		assetRequests[id] = req;
	}

    // Returns the WWW for retieving the assetbundle required for this 
    // SubMenuElement, and creates a WWW only if one doesnt exist already.
    public WWW WWW
    {
        get
        {
			/*
            if (!wwws.ContainsKey(bundleName)){
                wwws.Add(bundleName, new WWW(MenuGenerator.AssetbundleBaseURL + bundleName + ".assetbundle"));
				Debug.Log("assetBundle file: "+ MenuGenerator.AssetbundleBaseURL + bundleName + ".assetbundle");
				System.IO.FileInfo fi = new System.IO.FileInfo(MenuGenerator.AssetbundleBaseURL + bundleName + ".assetbundle");
				Debug.Log("FI: XX=" + fi.FullName + ", creation: "+fi.CreationTime);
			}
            return wwws[bundleName];
            */
			return MenuGenerator.GetWWW(bundleName);
        }
    }

    // Checks whether all textures for this SubMenuElement are loaded,
	// and starts the asynchronous loading of those assets if it has not started already.
    public bool IsLoaded
    {
        get
        {
            if (!WWW.isDone) return false;
			
			/*
			if (assetRequests.Count == 0){
				foreach (string src in sources){
					if (_isUnloading) return true;
					
					if (!assetRequests.ContainsKey(src)){
						AssetBundleRequest req = WWW.assetBundle.LoadAsync(src, typeof(Texture2D));
						assetRequests.Add(src, req);
					}
				}
			}
			
			foreach (AssetBundleRequest req in assetRequests.Values){
				if (_isUnloading) return true;
				
				if (!req.isDone)
					return false;
			}
			*/
			
            return true;
        }
    }
	
	private bool _isUnloading = false;
	public void Unload(){
		_isUnloading = true;
		assetRequests.Clear();
		WWW.assetBundle.Unload(true);
		_isUnloading = false;
	}
}