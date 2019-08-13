using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

	private static SpawnPoint instance;
	
	public static SpawnPoint Instance(){
		if(instance == null){
			instance = GameObject.FindObjectOfType<SpawnPoint>();
		}
		
		return instance;
	}
}
