using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayTrigger : MonoBehaviour {

	public GameObject target;
	
	public string function;
	
	public GameObject argument;
	
	private bool triggeredAlready = false;
	
	public void OnTriggerEnter2D(Collider2D c){
		if(triggeredAlready){
			return;
		}
		
		if(c.transform.tag == "Player"){
			target.SendMessage(function, argument, SendMessageOptions.DontRequireReceiver);
			triggeredAlready = true;
		} else if(c.transform.parent != null && c.transform.parent.tag == "Player"){
			target.SendMessage(function, argument, SendMessageOptions.DontRequireReceiver);
			triggeredAlready = true;
		}
	}
	
	public void OnTriggerExit2D(Collider2D c){
		if(c.transform.tag == "Player"){
			triggeredAlready = false;
		} else if(c.transform.parent != null && c.transform.parent.tag == "Player"){
			triggeredAlready = false;
		}
	}
}
