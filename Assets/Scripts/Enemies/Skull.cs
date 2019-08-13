using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : Enemy {
	
	public new int health = 1;
	
	public new int damage;
	private bool isAscending;
	private float verticalModifier;
	private float speed = 7f;
				
	public void Update(){
		
		if(facing == 1){
			sprite.flipX = false;
		} else {
			sprite.flipX = true;
		}

		if(madeLateralContact || Random.value > .99f){
			gameObject.GetComponent<AudioSource>().Play();
			TurnAround();
			madeLateralContact = false;
		}
		
		Move();
	}
	
	public void Damage(int damageReceived){
		if(isInvulnerable){
			return;
		}
		
		health -= damageReceived;
		
		if(health <= 0){
			Die();
		}
	}
	
	private void Move(){
		if(isAscending){
			verticalModifier += Time.deltaTime;
			isAscending = verticalModifier > 1f ? false : true;
		} else {
			verticalModifier -= Time.deltaTime;
			isAscending = verticalModifier < -1f ? true : false;
		}
		
		thisRigidbody.velocity = new Vector3(speed * facing, verticalModifier * 2f, 0f);
	}
}
