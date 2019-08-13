using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour {

	private int damage = 1;

	private float lifetime;
	private float slashTime = .2f;
	
	public void OnTriggerEnter2D(Collider2D c){		
		bool resetCooldown = false;
		
		
		
		if(c.transform.tag == "Projectile"){
			UI.Instance().IncrementCombo();
			c.gameObject.GetComponent<Rigidbody2D>().simulated = false;
			c.transform.GetComponent<SpriteRenderer>().enabled = false;
			resetCooldown = true;
		}
		
		if(c.transform.parent && c.transform.parent.tag == "Enemy" && !c.isTrigger){
			UI.Instance().IncrementCombo();
			switch(c.transform.parent.name)
			{
				case "Tentacle":
					c.transform.parent.gameObject.GetComponent<Tentacle>().Damage(damage);
					break;
				case "Eye":
					c.transform.parent.gameObject.GetComponent<Eye>().Damage(damage);
					break;
				case "Skull":
					c.transform.parent.gameObject.GetComponent<Skull>().Damage(damage);
					break;
				case "Hand":
					c.transform.parent.gameObject.GetComponent<Hand>().Damage(damage);
					break;
				case "Breaker":
					c.transform.parent.gameObject.GetComponent<Breaker>().Damage(damage);
					c.transform.parent.gameObject.GetComponent<Breaker>().MadeContact();
					break;
				default:
					// Destroy(c.gameObject);
					break;
			}
			resetCooldown = true;
		}
		
		if(resetCooldown){
			transform.parent.GetComponent<Player>().Combo();
		}
	}
	
	public void Update(){		
		if(lifetime > 0f){
			lifetime -= Time.deltaTime;
		} else {
			Disable();
		}
	}
	
	public void Activate(){
		lifetime = slashTime;
		gameObject.GetComponent<SpriteRenderer>().enabled = true;
		gameObject.GetComponent<Collider2D>().enabled = true;
	}
	
	public void Disable(){
		gameObject.GetComponent<Collider2D>().enabled = false;
		gameObject.GetComponent<SpriteRenderer>().enabled = false;
	}
}
