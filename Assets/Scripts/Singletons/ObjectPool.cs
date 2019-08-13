using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
	
	public Transform enemiesContainer;
	public Transform hazardsContainer;
	
	public List<GameObject> enemies;
	public List<GameObject> hazards;

	public void Awake(){
		enemies = new List<GameObject>(enemiesContainer.childCount);
		for(int i = 0; i < enemiesContainer.childCount; i++){
			GameObject enemy = enemiesContainer.GetChild(i).gameObject;
			enemies.Add(enemy);
		}
		
		hazards = new List<GameObject>(hazardsContainer.childCount);
		for(int i = 0; i < hazardsContainer.childCount; i++){
			GameObject hazard = enemiesContainer.GetChild(i).gameObject;
			hazards.Add(hazard);
		}
	}
	
	public void EnableAllEnemies(){
		foreach(GameObject g in enemies){
			EnableEnemy(g);
		}
	}
	
	public void DisableAllEnemies(){
		foreach(GameObject g in enemies){
			DisableEnemy(g);
		}
	}
	
	public void DisableEnemy(GameObject enemyGameObject){
		enemyGameObject.transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>().enabled = false;
		enemyGameObject.GetComponent<Rigidbody2D>().simulated = false;
		switch(enemyGameObject.transform.name){
			case "Eye":
				enemyGameObject.GetComponent<Eye>().enabled = false;
				break;
			case "Skull":
				enemyGameObject.GetComponent<Skull>().enabled = false;
				break;
			case "Breaker":
				enemyGameObject.GetComponent<Breaker>().enabled = false;
				break;
			case "Hand":
				enemyGameObject.GetComponent<Hand>().enabled = false;
				break;
			case "Tentacle":
				enemyGameObject.GetComponent<Tentacle>().enabled = false;
				break;
			default:
				break;
		}
	}
	
	public void EnableEnemy(GameObject enemyGameObject){
		enemyGameObject.transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>().enabled = true;
		enemyGameObject.GetComponent<Rigidbody2D>().simulated = true;
		switch(enemyGameObject.transform.name){
			case "Eye":
				enemyGameObject.GetComponent<Eye>().enabled = true;
				break;
			case "Skull":
				enemyGameObject.GetComponent<Skull>().enabled = true;
				break;
			case "Breaker":
				enemyGameObject.GetComponent<Breaker>().enabled = true;
				break;
			case "Hand":
				enemyGameObject.GetComponent<Hand>().enabled = true;
				break;
			case "Tentacle":
				enemyGameObject.GetComponent<Tentacle>().enabled = true;
				break;
			default:
				break;
		}
	}
	
	public void DisableObject(GameObject target){
		target.GetComponent<Rigidbody2D>().simulated = false;
		target.transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>().enabled = false;
	}
	
	public void EnableObject(GameObject target){
		target.GetComponent<Rigidbody2D>().simulated = true;
		target.transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>().enabled = true;
	}

	private static ObjectPool instance;
	
	public static ObjectPool Instance(){
		if(instance == null){
			instance = GameObject.FindObjectOfType<ObjectPool>();
		}
		
		return instance;
	}
}
