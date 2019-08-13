using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverButton : MonoBehaviour {

	public GameObject downTarget;
	
	public string downArgument;
	public string downFunction;

	private Bounds colliderBounds;
	
	private Transform backgroundTransform;
	
	private Vector3 visiblePosition = new Vector3(0f, 0f, -.2f);
	private Vector3 hiddenPosition = new Vector3(0f, 0f, .1f);
	
	public void Awake(){
		colliderBounds = gameObject.GetComponent<Collider>().bounds;
		backgroundTransform = transform.Find("Background");
	}
	
	public void Update(){
		colliderBounds = gameObject.GetComponent<Collider>().bounds;
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition = new Vector3(mousePosition.x, mousePosition.y, colliderBounds.center.z);

		if(colliderBounds.Contains(mousePosition)){
			if(backgroundTransform.localPosition != visiblePosition){
				backgroundTransform.localPosition = visiblePosition;
			}
			Hovering();
		} else {
			if(backgroundTransform.localPosition != hiddenPosition){
				backgroundTransform.localPosition = hiddenPosition;
			}
		}
	}
	
	public void Hovering(){
		if(Input.GetMouseButtonDown(0)){
			if (downTarget) {
				if (downFunction.Length > 0) {
					if (downArgument.Length > 0)
						downTarget.SendMessage(downFunction, downArgument, SendMessageOptions.DontRequireReceiver);
					else
						downTarget.SendMessage(downFunction, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
}
