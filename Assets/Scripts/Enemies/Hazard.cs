using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour {
	
	private int damage = 1;

	public Player player;
	
	public bool playerInContact;
	
	public void Start(){
		player = Player.Instance();
	}

	public void Update(){
		if(playerInContact){
			player.Damage(damage);
		}
	}
	
	public void OnTriggerEnter2D(Collider2D c){
		if(c.transform.parent && c.transform.parent.tag == "Player" && c.transform.tag != "Projectile"){
			playerInContact = true;
		}
	}
	
	public void OnTriggerExit2D(Collider2D c){
		if(c.transform.parent && c.transform.parent.tag == "Player" && c.transform.tag != "Projectile"){
			playerInContact = false;
		}
	}
}
