using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
	
	
	private float mass = 2f;
	
	private Vector3 gravitationalAcceleration = new Vector3(0f, -1.8f, 0f);
	
	public void Awake(){
		mass = 3f * transform.localScale.x;
	}
	
	public void FixedUpdate(){
		Gravity();
		Inertia();
	}
	
	private void Inertia(){
		gameObject.GetComponent<Rigidbody>().velocity = Vector3.Lerp(gameObject.GetComponent<Rigidbody>().velocity, Vector3.zero, Time.deltaTime * mass);
	}
	
	private void Gravity(){
		gameObject.GetComponent<Rigidbody>().velocity += gravitationalAcceleration * Time.deltaTime * mass;
	}
}
