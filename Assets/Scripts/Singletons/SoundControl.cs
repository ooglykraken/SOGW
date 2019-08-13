using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour {

	private static SoundControl instance;
	
	public static SoundControl Instance(){
		if(instance == null){
			instance = GameObject.FindObjectOfType<SoundControl>();
			
		}
		
		return instance;
	}
}
