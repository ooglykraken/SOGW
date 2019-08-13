using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyeshot : MonoBehaviour {
	private float movementSpeed = 1000F;
	private float lifespan = 6;
	private int damage = 1;
	public Vector3 target;
	public Vector3 startPoint = Vector3.zero;
	
	public void Fire(){
		Physics2D.IgnoreCollision(transform.parent.Find("Collider").gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
		transform.localScale = new Vector3(.5f, .5f, .5f);
		transform.localPosition = startPoint;
	}
	
	public void FixedUpdate(){
		if(gameObject.GetComponent<Rigidbody2D>().simulated){
			if(lifespan >= 0){
				lifespan -= Time.deltaTime;
				ProjectileMovement();
			} else {
				Die();
			}
		}
	}
	
	public void OnTriggerEnter2D(Collider2D c){
		if(c.transform.tag == "Enemy"){
			return;
		}
		
		if(c.transform.parent != null && c.transform.parent.tag == "Player" && c.transform.tag != "Projectile"){
			Player.Instance().Damage(damage);
			Die();
		}

		Die();
	}
	
	public void ProjectileMovement(){
		gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.Normalize(target - transform.parent.position) * (movementSpeed * Time.deltaTime);
	}

	public void Die(){
		gameObject.GetComponent<Rigidbody2D>().simulated = false;
		gameObject.GetComponent<SpriteRenderer>().enabled = false;
	}
}