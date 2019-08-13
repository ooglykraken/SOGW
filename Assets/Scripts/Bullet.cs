using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	
	private float movementSpeed = 40f;
	private float lifespan = .55f;
	private float lifeTimer;
	private int damage = 1;
	public Vector2 target;
	private Player player;	
	
	
	public void Awake(){		
		player = Player.Instance();
		transform.parent = player.transform;
		lifeTimer = lifespan;
	}
	
	public void Update(){
		if(gameObject.GetComponent<Rigidbody2D>().simulated){
			if(lifeTimer >= 0){
				lifeTimer -= Time.deltaTime;
			} else {
				Die();
			}
		}
	}
	
	public void OnCollisionEnter2D(Collision2D c){		
		if(c.transform.tag == "Projectile"){
			UI.Instance().IncrementCombo();
			Destroy(c.gameObject);
		}
		if(c.transform.tag == "Enemy"){
			UI.Instance().IncrementCombo();
			switch(c.transform.name)
				{
					case "Tentacle":
						c.transform.gameObject.GetComponent<Tentacle>().Damage(damage);
						break;
					case "Eye":
						c.transform.gameObject.GetComponent<Eye>().Damage(damage);
						break;
					case "Skull":
						c.transform.gameObject.GetComponent<Skull>().Damage(damage);
						break;
					case "Hand":
						c.transform.gameObject.GetComponent<Hand>().Damage(damage);
						break;
					case "Breaker":
						c.transform.gameObject.GetComponent<Breaker>().Damage(damage);
						break;
					default:
						// Destroy(c.gameObject);
						break;
				}
		}
		Die();
	}
	
	public void Fire(){
		lifeTimer = lifespan;
		Physics2D.IgnoreCollision(player.playerCollider, gameObject.GetComponent<Collider2D>());
		Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		target = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);
		Vector2 myPos = new Vector2(player.transform.Find("Gun").position.x,player.transform.Find("Gun").position.y);
		Vector2 direction = target - myPos;
		Vector2 normalizedDirection = direction;
		normalizedDirection.Normalize();
		
		float angle = Vector3.Angle(new Vector3(target.x, target.y, transform.position.z), new Vector3(myPos.x, myPos.y, transform.position.z));
		
		transform.eulerAngles = new Vector3(0f, 0f, angle);
		
		Vector2 newDirection = normalizedDirection * movementSpeed;

		gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(newDirection.x, newDirection.y, 0f);
	}
	
	public void Die(){
		gameObject.GetComponent<SpriteRenderer>().enabled = false;
		gameObject.GetComponent<Rigidbody2D>().simulated = false;
	}
}