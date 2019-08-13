using UnityEngine;
using System.Collections;

public class MapButton : MonoBehaviour {

	public bool isDisabled;

	public GameObject target;
	
	public string argument;
	public string function;
	
	private Bounds rendererBounds;
	
	private MeshRenderer meshRenderer;
	
	private Material thisMaterial;
	
	private float alphaShift = .008f;

	public void Awake(){
		meshRenderer = gameObject.GetComponent<MeshRenderer>();
		thisMaterial = meshRenderer.material;
		if(!isDisabled)thisMaterial.color = new Color(1f, 1f, 1f, 0f);
		rendererBounds = meshRenderer.bounds;
	}
	
	public void Update(){
		if(isDisabled){
			return;
		}
		
		rendererBounds = gameObject.GetComponent<MeshRenderer>().bounds;
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition = new Vector3(mousePosition.x, mousePosition.y, rendererBounds.center.z);
		
		if(rendererBounds.Contains(mousePosition)){
			// Debug.Log("Hovering");
			Hovering();
			Flash();
		} else if(thisMaterial.color.a > 0f){
			thisMaterial.color = new Color(1f, 1f, 1f, 0f);
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
	
	private void Flash(){
		
		// Debug.Log(thisMaterial.color.a);
		
		thisMaterial.color += new Color(0f, 0f, 0f, alphaShift);
		
		if(thisMaterial.color.a < .01f && alphaShift < 0){
			alphaShift *= -1f;
		} else if(thisMaterial.color.a > .6f && alphaShift > 0){
			alphaShift *= -1f;
		}
	}
}


