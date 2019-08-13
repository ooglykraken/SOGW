using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour {
	
	public void OnTriggerEnter2D(Collider2D c){
		if(c.transform.parent.tag == "Player"){
			gameObject.GetComponent<AudioSource>().Play();
			Player.Instance().Respawn();
		}
	}
	
	private static Floor instance;
	
	public static Floor Instance(){
		
		if(instance == null){
			instance = GameObject.FindObjectOfType<Floor>().GetComponent<Floor>();
		}
		
		return instance;
	}
}
