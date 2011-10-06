using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;

public class MenuGenerator
{
	static Dictionary<string, WWW> wwws = new Dictionary<string, WWW>();
	static Dictionary<string, MenuElement> menus = new Dictionary<string, MenuElement>();
	
	private MenuGenerator()
    {
    }
	
	public static void AddWWW(string bundle, WWW www){
		wwws.Add(bundle, www);
	}
	
	public static WWW GetWWW(string bundle){
		return wwws[bundle];
	}
	
	public static MenuElement GetMenuElement(XmlNode node){
		string bundle = node.Attributes["assetBundle"].Value;
		if (!menus.ContainsKey(bundle))
			menus.Add(bundle, new MenuElement(node));
		return menus[bundle];
	}
	
	// Returns correct assetbundle base url, whether in the editor, standalone or
    // webplayer, on Mac or Windows.
    public static string AssetbundleBaseURL
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
                return Application.dataPath+"/assetbundles/";
            else
                return "file://" + Application.dataPath + "/../assetbundles/";
        }
    }
}
