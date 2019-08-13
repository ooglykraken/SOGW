using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {
	
	public MeshRenderer shipQuery;
	
	public bool playerInRange; 
	
	// Use this for initialization
	void Start () {
		if(!shipQuery)
			shipQuery = transform.Find("ShipQuery").gameObject.GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if(playerInRange)
			if(Input.GetKey("space"))
				UI.Instance().OpenShipMenu();
			
		if(UI.Instance().shipMenuIsOpen)
			if(shipQuery.enabled)
				shipQuery.enabled = false;
	}
	
	public void OnTriggerEnter(Collider c){		
		if(c.transform.parent && c.transform.parent.tag == "Player"){
			playerInRange = true;
			OpenShipQuery();
		}
	}
	
	public void OnTriggerExit(Collider c){
		if(c.transform.parent && c.transform.parent.tag == "Player"){
			playerInRange = false;
			DisableShipQuery();
		}
	}
	
	public void OpenShipQuery(){
		shipQuery.enabled = true;
	}
	
	public void DisableShipQuery(){
		shipQuery.enabled = false;
	}
}
