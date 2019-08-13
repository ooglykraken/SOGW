using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breaker : Enemy {
	
	private new int health = 15;
	private float speed = 12f;
	private float modifiedSpeed = 20f;
	private bool isActive = false;
	public GameObject door;
	
	public void FixedUpdate(){
		if(isActive){
			Move();

			if(madeContact){
				Vector2 oldVelocity = thisRigidbody.velocity;
				thisRigidbody.velocity = Vector2.Reflect(oldVelocity, lastLateralNormal).normalized * speed;
				Vector2 newDirection = Vector2.Reflect(transform.right, lastLateralNormal);
				transform.right = newDirection;
				
				madeContact = false;
			}
		}
	}
	
	public void Enable(){
		isActive = true;
	}
	
	public void Damage(int damageReceived){
		health -= damageReceived;
		Debug.Log("taking damage");
		if(health <= 0){
			Die();
			if(door){
				ObjectPool.Instance().DisableObject(door);
			}
		}
	}
	
	public void MadeContact(){
		thisRigidbody.velocity *= -1f;
	}
	
	private void Move(){
		thisRigidbody.velocity = transform.right * modifiedSpeed;
	}
}
