using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : Enemy {	
	public new int health = 1;

	private float speed = 6f;
	
	public void Update(){
		if(madeLateralContact){
			TurnAround();
			madeLateralContact = false;
		}
		
		if(facing == -1){
			sprite.flipX = true;
		} else {
			sprite.flipX = false;
		}
		
		Move();
	}
	
	public void Damage(int damageReceived){
		health -= damageReceived;
		
		if(health <= 0){
			Die();
		}
	}
	
	private void Move(){
		animator.SetBool("isMoving", true);
		transform.position += new Vector3(Time.deltaTime * speed * facing, 0f, 0f);
	}
}
