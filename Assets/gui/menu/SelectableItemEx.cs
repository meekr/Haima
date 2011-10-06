using UnityEngine;
using System.Collections;
using System.Xml;

public class SelectableItemEx : SelectableItem {

	public SelectableItemEx(XmlNode node, MenuElement menuElement){
		XmlNode bg = node.SelectSingleNode("component");
		if (bg == null){
			Debug.Log("XXXXXXXX");
			Texture2D texture = Resources.Load("page/"+node.Attributes["src"].Value, typeof(Texture2D)) as Texture2D;
			setTexutre(texture, 1024f, 686f);
		}
		else{
			Texture2D texture = Resources.Load("page/pbg", typeof(Texture2D)) as Texture2D;
			setTexutre(texture, 1024f, 686f);
			XmlNodeList cs = bg.SelectNodes("c");
			foreach (XmlNode c in cs){
				Texture2D tex = menuElement.GetTextureById(c.Attributes["src"].Value);
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
	}
}
