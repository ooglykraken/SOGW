using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	public GameObject target;
	
	public string argument;
	public string function;
	
	private Bounds colliderBounds;

	public void Awake(){
		colliderBounds = gameObject.GetComponent<Collider>().bounds;
	}
	
	public void Update(){
		colliderBounds = gameObject.GetComponent<Collider>().bounds;
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition = new Vector3(mousePosition.x, mousePosition.y, colliderBounds.center.z);
		
		if(colliderBounds.Contains(mousePosition)){
			Hovering();
		}

	}
	
	public void Hovering(){
		if(Input.GetMouseButtonDown(0)){
			if (target) {
				if (argument.Length > 0)
					target.SendMessage(function, argument, SendMessageOptions.DontRequireReceiver);
				else
					target.SendMessage(function, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}


