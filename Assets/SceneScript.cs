using UnityEngine;
using System.Collections;
using System.Collections.Generic;

 
public class SceneScript : MonoBehaviour {
	public GameObject car1;
	public GameObject car2;
	public GameObject car3;
	public GameObject car4;
	public GameObject car5;
	public GameObject car6;
	public GameObject car7;
	private ArrayList _carSourceAry	= new ArrayList();
	private ArrayList _carAry	= new ArrayList();
	private float _camRadius	= 15.0f;
	private float _currentRadius = 50.0f;
	
	private Vector3 _camAng		= new Vector3(0,20,0);
	private Vector3 _currentAng	= new Vector3(0,20,0);
	
	private GameObject _selectedObj;
	
	public GameObject selectedObj {
		set
		{
			if (_selectedObj != value) {
				_selectedObj = value;
				if (_selectedObj != null) {
					for (int i=0; i<_carAry.Count; i++) {
						if (_carAry[i] != _selectedObj)
							(_carAry[i] as GameObject).SetActiveRecursively(false);
					}
					setCamRadius(5);
				}
				else {
					for (int i=0; i<_carAry.Count; i++) {
						(_carAry[i] as GameObject).SetActiveRecursively(true);
					}
					setCamRadius(20);
				}
			}
		}
	}

	private bool _isShowing	= true;
	
	public void showScene(){
		_isShowing	= true;
	}

	public void hideScene(){
		//Debug.Log("hide scene");
		_isShowing	= false;
		Camera.mainCamera.transform.position	= new Vector3(999999,999999,999999);
	}
	
	void Start () {
		_carSourceAry.Add(car1);
		_carSourceAry.Add(car2);
		_carSourceAry.Add(car3);
		_carSourceAry.Add(car4);
		_carSourceAry.Add(car5);
		_carSourceAry.Add(car6);
		_carSourceAry.Add(car7);
		
		for (int i=0; i<5; i++){
			float rad				= Mathf.PI*2*i/5.0f;
			Vector3 pos				= new Vector3(Mathf.Cos(rad)*8.5f, 0, Mathf.Sin(rad)*8.5f);
     		Quaternion rot			= Quaternion.identity;
			GameObject carInstance	= Instantiate(_carSourceAry[i] as GameObject, pos, rot) as GameObject;
			carInstance.transform.LookAt(new Vector3(Mathf.Cos(rad)*15f, 0, Mathf.Sin(rad)*15f));
			_carAry.Add(carInstance);
		}
	}
	
	// Update is called once per frame
	private Vector3	_lookAt		= new Vector3(0, -8, 0);
	
	List<Touch> touchInsideAry	= new List<Touch>();
	List<Touch> tapInsideAry	= new List<Touch>();
	
	void Update () {
		if (!_isShowing)
			return;
		
		//Debug.Log("render");
		//update touch
		
		touchInsideAry.Clear();
		tapInsideAry.Clear();
		
		foreach (Touch touch in Input.touches) {
	    	if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
				tapInsideAry.Add(touch);
	    	}
			else {
    			touchInsideAry.Add(touch);
	    	}
	    }
		
		if (touchInsideAry.Count == 1) {
	    	Touch soloTouch = touchInsideAry[0];
	    	addCamRotate(new Vector3(-soloTouch.deltaPosition.x*0.2f, -soloTouch.deltaPosition.y*0.2f, 0));
	    }
		
		if (touchInsideAry.Count == 0 && tapInsideAry.Count == 1) {
	    	Touch soloTap = tapInsideAry[0];
			if (soloTap.tapCount == 1) {
				GameObject hitObj = hitTest(soloTap.position);
				if (hitObj != null) {
					//Debug.Log("selectObject:"+ _selectedObj);
					selectedObj	= hitObj;
				}
			}
			else if( soloTap.tapCount == 2) {
				selectedObj = null;
				//Debug.Log("unselectObject");
			}
			//setSelectedObj(hitObj);
		}

		//2 fingers touch to scale
	    if (touchInsideAry.Count == 2) {
	    	Touch firstTouch = touchInsideAry[0];
			Touch secondTouch = touchInsideAry[1];
			float thisDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
			float lastDistance = Vector2.Distance(firstTouch.position - firstTouch.deltaPosition, secondTouch.position-secondTouch.deltaPosition);
			float deltDistance = Mathf.Sqrt(lastDistance / thisDistance);
			setCamRadius(_camRadius * deltDistance);
	    }
	    
		if (_selectedObj != null) {
			_lookAt.x	+= (_selectedObj.transform.position.x - _lookAt.x)/16;
			_lookAt.y	+= (_selectedObj.transform.position.y - _lookAt.y)/16;
			_lookAt.z	+= (_selectedObj.transform.position.z - _lookAt.z)/16;
		}
		else {
			_lookAt.x	-= _lookAt.x/16;
			_lookAt.y	+= (-8-_lookAt.y)/16;
			_lookAt.z	-= _lookAt.z/16;
		}
		//update camera
		_currentAng		+= (_camAng - _currentAng)/8;
		_currentRadius	+= (_camRadius - _currentRadius)/8;
		
		float r			= _currentRadius * Mathf.Cos(_currentAng.y/180*Mathf.PI);
		Vector3	camPos  = new Vector3();
		camPos.x		= r * Mathf.Cos(_currentAng.x/180*Mathf.PI) + _lookAt.x;
		camPos.z		= r * Mathf.Sin(_currentAng.x/180*Mathf.PI) + _lookAt.z;
		camPos.y		= Mathf.Sqrt(_currentRadius*_currentRadius - r*r);
		
		Camera.mainCamera.transform.position	= camPos;
		Camera.mainCamera.transform.LookAt(_lookAt);
	}
	
	private float _maxCamRadius					= 30.0f;
	private float _minCamRadius					= 5.0f;
	private float _minObjSelectedCamRadius		= 3.0f;
	void addCamRotate(Vector3 vec) {
		_camAng	+= vec;
		if (_camAng.y < 20) {
			_camAng.y	= 20;
		}
		else if( _camAng.y > 80) {
			_camAng.y	= 80;
		}
	}
	
	void setCamRadius(float val) {
		if (_selectedObj == null) {
			if (val < _minCamRadius)
				val = _minCamRadius;
		}
		else {
			if (val < _minObjSelectedCamRadius)
				val = _minObjSelectedCamRadius;
		}
		
		if (val > _maxCamRadius)
			val = _maxCamRadius;
		
		_camRadius	= val;
	}
	
		
	GameObject hitTest(Vector2 position) {
		Ray ray = Camera.mainCamera.ScreenPointToRay(position);    
	    RaycastHit hit;
	    if (Physics.Raycast(ray, out hit)) {
	        //Debug.Log("hit object: "+ hit.collider.gameObject.name);
	        return hit.collider.gameObject;
	    }
		return null;
	}
}