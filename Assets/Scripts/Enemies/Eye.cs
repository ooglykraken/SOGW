using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : Enemy {
	
	public new int health = 1;
	
	public new int damage;
			
	private bool shootAtPlayer;
	
	private new float visibilityDistance = 20f;
	
	private float attackCooldown = 2f;
	public float attackTimer;
		
	// private Vector3 shotPoint = Vector3.zero;
	
	public bool projectilesInitialized = false;
	public List<GameObject> projectiles = new List<GameObject>();

	public void Update(){
		if(!projectilesInitialized){
			InitializeProjectiles();
			projectilesInitialized = true;
		}
		if(isFrozen && sprite.color != Color.cyan){
			sprite.color = Color.cyan;
		} else if(!isFrozen && sprite.color != Color.white){
				sprite.color = Color.white;
		}
		
		if(!isFrozen){
			if(Vector3.Distance(transform.position, player.transform.position) < visibilityDistance){
				shootAtPlayer = true;
			} else {
				shootAtPlayer = false;
			}
				
			if(shootAtPlayer){
				if(attackTimer > 0){
					attackTimer -= Time.deltaTime;
					sprite.color += new Color(.1f, 0f, 0f, 0f);
				}
				
				if(attackTimer <= 0){
					sprite.color = Color.white;
					Attack();
				}
			}
		}
	}

	public void InitializeProjectiles(){
		projectiles = new List<GameObject>();
		for(int i = 0; i < transform.childCount; i++){
			Transform child = transform.GetChild(i);
			if(child.name == "Eyeshot")
				projectiles.Add(child.gameObject);
		}
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
	
	private Eyeshot FirstInactiveEyeshot(){
		foreach(GameObject g in projectiles){
			if(!g.GetComponent<Rigidbody2D>().simulated){
				g.GetComponent<Rigidbody2D>().simulated = true;
				g.GetComponent<SpriteRenderer>().enabled = true;
				return g.GetComponent<Eyeshot>();
			} 
		}
		Eyeshot eyeshot = Instantiate(Resources.Load("Projectile/Eyeshot", typeof(Eyeshot)) as Eyeshot) as Eyeshot;				
		projectiles.Add(eyeshot.gameObject);
		return eyeshot;
	}
	
	private void Attack(){
		Vector3 targetPosition = Player.Instance().transform.position;
		attackTimer = attackCooldown;
		
		Eyeshot eyeshot = FirstInactiveEyeshot();
		
		eyeshot.target = targetPosition;
		eyeshot.Fire();
		gameObject.GetComponent<AudioSource>().Play();
	}
	
	// public new void Die(){
		// while(transform.Find("Eyeshot") != null){
			// transform.Find("Eyeshot").parent = Gameplay.Instance().gameObject.transform;
		// }
		// transform.parent.gameObject.GetComponent<AudioSource>().Play();
		// Destroy(gameObject);
	// }
}
