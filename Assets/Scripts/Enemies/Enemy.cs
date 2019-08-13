using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Enemy : MonoBehaviour {
	
	public int health { get; set; }

	public int damage { get; set; }
	
	private int numberOfContactPoints = 10;
	
	public float visibilityDistance {get; set;}
	
	public bool isInvulnerable { get; set; }
	
	public bool isFrozen { get; set; }
	public bool isStunned { get; set; }
	
	public bool isMoving { get; set; }
	public bool isAttacking { get; set; }
	public bool playerInSight {get;set;}
	
	public bool madeContact;
	public bool madeLateralContact;
	public bool madeBottomContact;
	
	public Vector2 lastLateralNormal;
	
	public ContactPoint2D[] contactPoints;
	// public ContactPoint2D[] triggerPoints;
	
	public int facing = 1;
		
	public Animator animator;
	
	public Rigidbody2D thisRigidbody;
	
	public Player player;
	
	public SpriteRenderer sprite;
	public Collider2D thisCollider;
	
	public void Awake(){
		sprite = transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
		thisCollider = transform.Find("Collider").gameObject.GetComponent<Collider2D>();
		thisRigidbody = gameObject.GetComponent<Rigidbody2D>();
		animator = transform.Find("Sprite").gameObject.GetComponent<Animator>();
		
		contactPoints = new ContactPoint2D[numberOfContactPoints];
	}
	
	public void Start(){
		player = Player.Instance();
		damage = 1;
		// transform.name = transform.name.Split("("[0])[0];
	}
		
	public void TurnAround(){
		facing *= -1;
	}
	
	public void OnCollisionEnter2D(Collision2D c){
		if(contactPoints == null){
			contactPoints = new ContactPoint2D[numberOfContactPoints];
		}
		int numberOfContacts = c.GetContacts(contactPoints);
		madeBottomContact = false;
		madeLateralContact = false;
		if(numberOfContacts <= 0){
			madeContact = false;
		} else {
			madeContact = true;
			for(int i = 0; i < numberOfContacts; i++){
				ContactPoint2D contact = contactPoints[i];
				Vector2 contactPoint = contactPoints[i].point;
				
				bool touchedPlayer = false;
				if(c.transform.tag == "Player" && !isFrozen){
					Player.Instance().Damage(damage);
					touchedPlayer = true;
				}
				
				if(c.transform.tag == "Enemy" || c.transform.tag == "Hazard" || touchedPlayer || 
					(c.transform.tag == "Platform" && Mathf.Abs(contact.normal.x) > 0f)){
					lastLateralNormal = contact.normal;
					madeLateralContact = true;
				} 

				if(contactPoint.y < thisCollider.bounds.min.y){
					madeBottomContact = true;
				}
			}
		}
	}
	
	public void OnCollisionExit2D(Collision2D c){
		if(contactPoints == null){
			contactPoints = new ContactPoint2D[numberOfContactPoints];
		}
		int numberOfContacts = c.GetContacts(contactPoints);

		if(numberOfContacts <= 0){
			madeContact = false;
			madeBottomContact = false;
			madeLateralContact = false;
		} else {
			for(int i = 0; i < numberOfContacts; i++){
				ContactPoint2D contactPoint = contactPoints[i];
				if(contactPoint.point.x < thisCollider.bounds.min.x || contactPoint.point.x > thisCollider.bounds.max.x){
					madeLateralContact = false;
				}

				if(contactPoint.point.y < thisCollider.bounds.min.y){
					madeBottomContact = false;
				}				
			}
		}
	}
	
	public void Revive(){
		transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>().enabled = true;
		gameObject.GetComponent<Rigidbody2D>().simulated = true;
		switch(transform.name){
			case "Eye":
				gameObject.GetComponent<Eye>().enabled = true;
				break;
			case "Skull":
				gameObject.GetComponent<Skull>().enabled = true;
				break;
			case "Breaker":
				gameObject.GetComponent<Breaker>().enabled = true;
				break;
			case "Hand":
				gameObject.GetComponent<Hand>().enabled = true;
				break;
			case "Tentacle":
				gameObject.GetComponent<Tentacle>().enabled = true;
				break;
			default:
				break;
		}
	}
	
	public void DropScrap(){
		int chanceToDrop = Random.Range(0,100) + UI.Instance().playerCombo;
		if(chanceToDrop > 50){
			GameObject scrap = Instantiate(Resources.Load("Pickups/Scrap", typeof(GameObject)) as GameObject) as GameObject;
			scrap.transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
		}
	}
	
	public void Die(){
		transform.parent.gameObject.GetComponent<AudioSource>().Play();
		transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>().enabled = false;
		gameObject.GetComponent<Rigidbody2D>().simulated = false;
		
		DropScrap();
		
		switch(transform.name){
			case "Eye":
				gameObject.GetComponent<Eye>().enabled = false;
				break;
			case "Skull":
				gameObject.GetComponent<Skull>().enabled = false;
				break;
			case "Breaker":
				gameObject.GetComponent<Breaker>().enabled = false;
				break;
			case "Hand":
				gameObject.GetComponent<Hand>().enabled = false;
				break;
			case "Tentacle":
				gameObject.GetComponent<Tentacle>().enabled = false;
				break;
			default:
				break;
		}
	}
}