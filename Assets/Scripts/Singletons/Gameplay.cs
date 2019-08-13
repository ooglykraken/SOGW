using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameplay : MonoBehaviour {
	
	// private float fps;
	
	public bool isPaused = false;
	
	public bool testRun;
	public bool displayFPS;
	
	public bool playerExists;
	
	public Texture2D customCursor;
	
	private Player player;
	
	public void Awake(){
		DontDestroyOnLoad(gameObject);
		// Application.targetFrameRate = 60;
		// if(testRun){
			// player = Instantiate(Resources.Load("Player", typeof(Player)) as Player) as Player;
			// Respawn();
			// playerExists = true;
			// testRun = false;
		// }
		
		if(!playerExists){
			player = Instantiate(Resources.Load("Player", typeof(Player)) as Player) as Player;
			playerExists = true;
			// player.health = player.maxHealth;
		}
		
	}
	
	// public float getFPS(){
		// return fps;
	// }
	
	// public void FixedUpdate(){
		// fps = 1.0f / (Time.deltaTime * 1000.0f);
	// }

	public void TogglePause(){
		if(isPaused){
			isPaused = false;
			Time.timeScale = 1;
		} else {
			isPaused = true;
			Time.timeScale = 0;
		}
	}
	
	public void Respawn(){
		player.transform.position = SpawnPoint.Instance().transform.position;
		player.health = 10;
	}
	
	public void PlayerDeath(){
		Player.Instance().ResetPlayer();
		// SceneManager.LoadScene("intro", LoadSceneMode.Single);
		
		Respawn();
		ObjectPool.Instance().EnableAllEnemies();
		Vector3 playerPosition = Player.Instance().transform.position;
		Camera.main.transform.position = new Vector3(playerPosition.x, playerPosition.y, Camera.main.transform.position.z);
	}
	
	// public void LoadHub(){
		// SceneManager.LoadScene("Hub", LoadSceneMode.Single);
		// if(!Gameplay.Instance().testRun && !playerExists){
			
		// }
	// }
	
	public void LoadScene(string sceneName){
		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
		if(Time.timeScale < 1)TogglePause();
	}
	
	public void FinishTutorial(){
		
	}
	
	public void OpenAbilityChart(){
		
	}
	
	private static Gameplay instance;
	
	public static Gameplay Instance(){
		if(instance == null){
			instance = GameObject.FindObjectOfType<Gameplay>();
		}
		
		return instance;
	}
}
