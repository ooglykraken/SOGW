using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {
	
	public GameObject healthBar;
	public GameObject fuelBar;
	public GameObject heatSink;
	public GameObject MapOverview;
	public GameObject debug;
	public GameObject comboCounter;
	public GameObject scrap;
	
	private Transform fuelBarMeter;
	private Transform heatSinkMeter;
	
	private TextMesh comboCounterText;
	private TextMesh debugFPS;
	private TextMesh debugMs;
	
	private float fuelStartingScale;
	private float heatStartingScale = 15;
	
	public int playerCombo = 0;
	private int comboCooldown = 540;
	public int comboCountdown = 0;
	
	private float maxComboSize = .017f;
	private float normalComboSize = 0.01f;
	
	private Player player;
	
	public bool allowDebug;
	
	public bool mapIsOpen = false;
	public bool shipMenuIsOpen = false;
	public bool shopIsOpen = false;
	
	public void Start(){
		player = Player.Instance();
		
		fuelBarMeter = fuelBar.transform.Find("Bar");
		heatSinkMeter = heatSink.transform.Find("Bar");
		
		comboCounterText = comboCounter.transform.Find("Counter").gameObject.GetComponent<TextMesh>();
		debugFPS = debug.transform.Find("FPS").gameObject.GetComponent<TextMesh>();
		debugMs = debug.transform.Find("Millis").gameObject.GetComponent<TextMesh>();
		
		fuelStartingScale = fuelBar.transform.Find("Bar").localScale.y;
		// heatStartingScale = heatSink.transform.Find("Bar").localScale.x;
		// heatSink.transform.Find("Bar").localScale = new Vector3(0f, heatSink.transform.Find("Bar").localScale.y, heatSink.transform.Find("Bar").localScale.z);
	}
	
	public void Update(){
		if(allowDebug){
			if(!debug.activeSelf){
				debug.SetActive(true);
			}
			UpdateDebug();
		} else {
			if(debug.activeSelf){
				debug.SetActive(false);
			}
		}
		 
		if(mapIsOpen || player == null){
			return;
		}
		
		if(player.hasJet){
			if(!fuelBar.activeSelf)
				fuelBar.SetActive(true);
			UpdateFuel();
		}else{ 
			fuelBar.SetActive(false);
		}
		
		if(player.hasGun){
			if(!heatSink.activeSelf)
				heatSink.SetActive(true);
			if(player.heat > 0)
				UpdateHeatSink();
		}else{ 
			heatSink.SetActive(false);
		}
		
		if(comboCountdown > 0){
			comboCountdown--;
		} else if(comboCountdown <= 0 && playerCombo > 0){
			playerCombo = 0;
		}
		
		UpdateHealth();
		UpdateComboCounter();
		
		scrap.transform.Find("Counter").gameObject.GetComponent<TextMesh>().text = "x " + Player.Instance().scrap;
	}
	
	public void IncrementCombo(){
		comboCounter.transform.Find("Counter").localScale = new Vector3(maxComboSize, maxComboSize, .1f);
		playerCombo++;
		comboCountdown = comboCooldown;
	}
	
	public void UpdateHealth(){
		for(int i = healthBar.transform.childCount - 1; i >= 0; i--){
			
			if(i >= player.health){
				healthBar.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().enabled = false;
			} else {
				healthBar.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().enabled = true;
			}
		}
	}
	
	public void UpdateFuel(){
		float ratio = player.fuel/100f;
		
		fuelBarMeter.localScale = new Vector3(fuelBar.transform.Find("Bar").localScale.x, ratio * fuelStartingScale, fuelBar.transform.Find("Bar").localScale.z);
		fuelBarMeter.localPosition = new Vector3(fuelBar.transform.Find("Bar").localPosition.x, -((1f - ratio) * fuelStartingScale)/2, fuelBar.transform.Find("Bar").localPosition.z);
		
	}
	
	public void UpdateHeatSink(){
		float ratio = player.heat/100f;
				
		heatSinkMeter.localScale = new Vector3(ratio * heatStartingScale, heatSink.transform.Find("Bar").localScale.y, heatSink.transform.Find("Bar").localScale.z);
		heatSinkMeter.localPosition = new Vector3(-heatStartingScale/2 + (ratio * heatStartingScale)/2f, heatSink.transform.Find("Bar").localPosition.y, heatSink.transform.Find("Bar").localPosition.z);
	}
	
	public void UpdateComboCounter(){		
		comboCounterText.text = "x " + playerCombo;
		float currentComboTextSize = normalComboSize + (((float) comboCountdown / (float) comboCooldown) * (maxComboSize - normalComboSize));
		comboCounter.transform.Find("Counter").localScale = new Vector3(currentComboTextSize, currentComboTextSize, .1f);
	}
	
	public void UpdateDebug(){
		debug.transform.Find("FPS").gameObject.GetComponent<TextMesh>().text = "FPS: " + Mathf.Round(1.0f / Time.deltaTime);
		debug.transform.Find("Millis").gameObject.GetComponent<TextMesh>().text = Mathf.Round(Time.deltaTime * 1000.0f) + " ms";
	}
	
	public void OpenShipMenu(){
		if(Time.timeScale > 0)Gameplay.Instance().TogglePause();
		shipMenuIsOpen = true;
		transform.Find("HealthBar").gameObject.SetActive(false);
		fuelBar.SetActive(false);
		transform.Find("ShipMenu").gameObject.SetActive(true);
	}
	
	public void CloseShipMenu(){
		if(Time.timeScale < 1)Gameplay.Instance().TogglePause();
		shipMenuIsOpen = false;
		transform.Find("HealthBar").gameObject.SetActive(true);
		fuelBar.SetActive(true);
		transform.Find("ShipMenu").gameObject.SetActive(false);
	}
	
	public void OpenShop(){
		shopIsOpen = false;
		transform.Find("ShopMenu").gameObject.SetActive(true);
		transform.Find("ShipMenu").gameObject.SetActive(false);
	}
	
	public void CloseShop(){
		shopIsOpen = false;
		transform.Find("ShopMenu").gameObject.SetActive(false);
		transform.Find("ShipMenu").gameObject.SetActive(true);
	}
		
	public void OpenAbilityChart(){
		
	}
	
	// public void OpenDialogueBox(int startIndex, int endIndex){
		// dialogueBox.SetActive(true);
		// dialogueBox.transform.Find("Text").gameObject.GetComponent<TextController>().SetDialogue(startIndex, endIndex);
		// Gameplay.Instance().TogglePause();
	// }
	
	// public void CloseDialogueBox(){
		// dialogueBox.SetActive(false);
		// Gameplay.Instance().TogglePause();
	// }
	
	public void OpenMap(){
		if(Time.timeScale > 0)Gameplay.Instance().TogglePause();
		mapIsOpen = true;
		transform.Find("HealthBar").gameObject.SetActive(false);
		transform.Find("ShipMenu").gameObject.SetActive(false);
		transform.Find("Overview").gameObject.SetActive(true);
	}
	
	public void CloseMap(){
		if(Time.timeScale < 1)Gameplay.Instance().TogglePause();
		mapIsOpen = false;
		transform.Find("HealthBar").gameObject.SetActive(true);
		if(shipMenuIsOpen){
			transform.Find("ShipMenu").gameObject.SetActive(true);
			if(Time.timeScale > 0)Gameplay.Instance().TogglePause();
		}
		transform.Find("Overview").gameObject.SetActive(false);
	}

	private static UI instance;
	
	public static UI Instance(){
		if(instance == null){
			instance = GameObject.FindObjectOfType<UI>();
			
		}
		
		return instance;
	}
	
}
