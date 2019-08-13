using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : Enemy {
	
	public new int health = 1;
	
	// public new float visibilityDistance = 12f;
	// private float meleeDistance = 4f;
	
	//private float playerDistance = 100f;
	
	private float moveTimer;
	private float moveCooldown = 1f;
	
	public new int damage;
	
	private float mass = 1f;
	
	private float speed = .002f;
				
	private float gravitationalAcceleration = .0018f;	
		
	private Rigidbody2D tentacleRigidbody;
	
	public void Update(){
		moveTimer -= Time.deltaTime;
	}
	
	public void FixedUpdate(){
			
		if(moveTimer <= 0f || madeBottomContact){
			Move();
		}
		
		Gravity();
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
		gameObject.GetComponent<AudioSource>().Play();
		
		moveTimer = moveCooldown + Random.Range(0,.6f) - .3f;
		animator.SetBool("isMoving", true);
		
		if(tentacleRigidbody == null){
			tentacleRigidbody = transform.GetComponent<Rigidbody2D>();
		}
		tentacleRigidbody.AddForce(transform.up * speed, ForceMode2D.Impulse);
		
		madeBottomContact = false;
		madeLateralContact = false;
	}

	private void Gravity(){
		tentacleRigidbody.AddForce(-transform.up * gravitationalAcceleration * mass, ForceMode2D.Force);
	}
}
