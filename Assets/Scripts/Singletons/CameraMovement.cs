using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
	
	public Camera thisCamera;
	
	public void Awake(){		
		thisCamera = gameObject.GetComponent<Camera>();
	}
	
	private static CameraMovement instance;
	
	public static CameraMovement Instance(){
		if(instance == null){
			instance = Camera.main.gameObject.GetComponent<CameraMovement>();
		}
		
		return instance;
	}
	
}