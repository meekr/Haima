using UnityEngine;
using System.Collections;

public class InteractImageClip : SelectableItem {
	
	public ZoomableContentType type = ZoomableContentType.Image;
	public string videoSrc;
	
	public InteractImageClip (Texture texture):base(texture) {
		
	}
	
}
