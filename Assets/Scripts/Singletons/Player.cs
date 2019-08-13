using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	// Public
	public int facing;
	public int lastFacing;
	public int health = 10;
	public int maxHealth = 10;
	public int scrap = 0;

	public bool hasGun;
	public bool hasJet;
	
	public float fuel;
	public float heat;
	
	public Rigidbody2D playerRigidbody;
	
	public Collider2D playerCollider;
	
	public AudioClip walkLeftClip;
	public AudioClip walkRightClip;
	public AudioClip jumpClip;
	public AudioClip landingClip;
	public AudioClip steamClip;
	public AudioClip failureClip;
	public AudioClip knifeReadyClip;
	// Public
	
	// Private	
	private bool leftGrounded;
	private bool rightGrounded;
	private bool centerGrounded;
	private bool isMoving;
	private bool isGrounded;
	private bool isHoldingDown;
	private bool canMove = true;
	private bool isJumping = false;
	private bool isLongJumping = false;
	private bool isJetting = false;
	private bool useLeftFoot = true;
	private bool isHoldingSpace;
	private bool isInvulnerable;
	
	private int horizontalInput;
	private int verticalInput;
	private int raycastBuffer = 10;
	
	private float knifeCooldown = .8f;
	private float knifeTimer;
	private float gunCooldown = .4f;
	private float gunTimer;
	private float heatCost = 10f;
	private float heatLossRate;
	private float baseHeatLossRate = 2f;
	private float invulnerableCooldown = 1f;
	private float invulnerableTimer;
	private float walkTime = .2f;
	private float walkTimer = .2f;
	private float movementSpeed = 15f;
	private float aerialHorizontalImpetus = 2f;
	private float mass = 20f;
	private float jumpingMass = 19f;
	private float jumpForceModifier = 8f;
	private float baseFuelRegnerationRate = 2f;
	private float fuelRegnerationRate;
	private float jetSpeed = 1500f;
	private float jetFuelCost = .3f;
	private float maxFuel = 100;
	
	private GameObject jet;
	private GameObject gun;
	
	private SpriteRenderer playerSprite;
	private SpriteRenderer gunSprite;
	
	private Animator playerAnimator;
	
	private Vector2 gravitationalAcceleration = new Vector2(0f, -2f);
	private Vector2 lastGroundedPosition;
	
	private CameraMovement cameraInstance;

	private List<GameObject> projectiles;
	
	private Transform slash;
	
	private RaycastHit2D[] forwardHits;
	private RaycastHit2D[] leftHits;
	private RaycastHit2D[] rightHits;
	
	private AudioSource walkLeftSource;
	private AudioSource walkRightSource;
	private AudioSource jumpSource;
	private AudioSource landingSource;
	private AudioSource steamSource;
	private AudioSource failureSource;
	private AudioSource knifeReadySource;
	// Private

	private AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) { 
		AudioSource newAudio = gameObject.AddComponent<AudioSource>();
		newAudio.clip = clip; 
		newAudio.loop = loop;
		newAudio.playOnAwake = playAwake;
		newAudio.volume = vol; 
		return newAudio; 
	}
	
	public void Awake(){
		if(Gameplay.Instance().playerExists){
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
		Gameplay.Instance().playerExists = true;
	
		fuel = 100;
		heat = 0;
		health = maxHealth;
		
		transform.position = SpawnPoint.Instance().transform.position;
		
		walkLeftSource = AddAudio(walkLeftClip, false, false, .3f);
		walkRightSource = AddAudio(walkRightClip, false, false, .3f);
		jumpSource = AddAudio(jumpClip, false, false, .9f);
		landingSource = AddAudio(landingClip, false, false, 1f);
		steamSource = AddAudio(steamClip, false, false, 1f);
		failureSource = AddAudio(failureClip, false, false, 1f);
		knifeReadySource = AddAudio(knifeReadyClip, false, false, .1f);
		
		forwardHits = new RaycastHit2D[raycastBuffer];
		leftHits = new RaycastHit2D[raycastBuffer];
		rightHits = new RaycastHit2D[raycastBuffer];
		
		transform.name = transform.name.Split("("[0])[0];
	}
	
	public void Start(){
		playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
		playerCollider = transform.Find("Collider").gameObject.GetComponent<Collider2D>();
		playerSprite = transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
		
		playerAnimator = transform.Find("Sprite").gameObject.GetComponent<Animator>();
			
		cameraInstance = CameraMovement.Instance();

		slash = transform.Find("Slash");
		
		InitializeProjectiles();
		
		gun = transform.Find("Gun").gameObject;
		gunSprite = transform.Find("Gun").gameObject.GetComponent<SpriteRenderer>();
		
		jet = transform.Find("Jet").gameObject;
						
		lastFacing = 1;
		facing = 1;
	}
	
	private void InitializeProjectiles(){
		projectiles = new List<GameObject>();
		for(int i = 0; i < transform.childCount; i++){
			Transform child = transform.GetChild(i);
			if(child.name == "Bullet")
				projectiles.Add(child.gameObject);
		}
	}
	
	public void Update(){
		if(Input.GetKeyDown("p")){
			Gameplay.Instance().TogglePause();
		}
		
		if(Gameplay.Instance().isPaused){
			return;
		}
		
		if(isInvulnerable){
			invulnerableTimer -= Time.deltaTime;
			
			Blink();
			if(invulnerableTimer <= 0f){
				isInvulnerable = false;
				playerSprite.color = new Vector4(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 1f);
			}
		}

		if(canMove){
			if(Input.GetKey("a")){
				horizontalInput = -1;
			} else if(Input.GetKey("d")){
				horizontalInput = 1;
			} else {
				horizontalInput = 0;
				isMoving = false;
			}
			
			bool readyToJet = false;
			
			if(Input.GetKey("space")){
				readyToJet = true;
			}
			
			verticalInput = 0;
			if(Input.GetKey("s")){
				verticalInput = -1;
			} else if(Input.GetKey("w")){
				verticalInput = 1;
			}
			
			if(horizontalInput != 0){
				isMoving = true;
				fuelRegnerationRate = baseFuelRegnerationRate;
				lastFacing = facing;
				
				if(horizontalInput < 0){
					playerSprite.flipX = true;
					facing = -1;
				} else if(horizontalInput > 0){
					playerSprite.flipX = false;
					facing = 1;
				}
			}	
			
			if(gunTimer > 0f){
				gunTimer -= Time.deltaTime;
				gunSprite.flipX = facing > 0 ? false : true;
			} else if(gunSprite.enabled == true){
				gunSprite.enabled = false;
			}
			
			if(hasGun && Input.GetMouseButton(1)){
				if(heat + heatCost <= 100 && gunTimer <= 0 && knifeTimer <= 0){
					Shoot();
				} else if(heat >= 100){
					steamSource.Play();
				}
			}
			
			if(knifeTimer > 0f){
				knifeTimer -= Time.deltaTime;
				if(knifeTimer <= 0f){
					knifeReadySource.Play();
				}
			}
			
			if(Input.GetMouseButtonDown(0) && knifeTimer <= 0f){
				Melee();
				if(gunTimer > 0f)
					gunTimer = 0;
			} 
			
			if(isGrounded){
				playerAnimator.SetBool("isRunning", isMoving);					
			} else {
				playerAnimator.SetBool("isRunning", false);
			}
			
			if(hasJet && readyToJet && fuel > 0){
				isJetting = true;
			} else if(hasJet && readyToJet && fuel <= 0){
				isJetting = false;
				failureSource.Play();
			} else {
				jet.GetComponent<AudioSource>().Stop();
				isJetting = false;
			}
			
			if(fuel < maxFuel){
				fuel += fuelRegnerationRate * Time.deltaTime;
			} 
			
			playerAnimator.SetBool("isJetting", isJetting);
			
			if(heat > 0){
				heatLossRate = baseHeatLossRate;
				heat -= (heatLossRate * Time.deltaTime);
			}
			
		} else {
			isMoving = false;
		}
	}
	
	public void FixedUpdate(){
		bool groundedLastFrame = isGrounded;
		isGrounded = CheckGrounding();
				
		Bounds playerBounds = playerCollider.bounds;

		if(canMove){	
			if(isGrounded){
				if(!groundedLastFrame){
					landingSource.Play();
				}
				
				lastGroundedPosition = new Vector2(transform.position.x, transform.position.y);
				playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0f);
				isJumping = false;
				
				Inertia();
			}
			
			if(isJumping && verticalInput > 0 && playerRigidbody.velocity.y < .1f){
				isLongJumping = true;
			} else {
				isLongJumping = false;
				if(isGrounded && verticalInput > 0){
					Jump();
				}
			}	
			
			if(isJetting){
				Jet();
			} 
			
			if(!isGrounded){
				if(!isJumping){
					isJumping = true;
				}
				transform.parent = null;
				Gravity();
			}
			if(isMoving){
				Move();
			}
			
		} else {
			if(!isGrounded){
				Gravity();
			} else {
				Inertia();
			}
		}
	}
	
	public void LateUpdate () {
		
		if(cameraInstance == null){
			cameraInstance = CameraMovement.Instance();
			Gameplay.Instance().Respawn();
		}
		
		float verticalOffset = 6f;
		float horizontalOffset = 6f;
		
		if(playerRigidbody.velocity.y < -7f || verticalInput < 0){
			verticalOffset *= -1f;
		}
		
		if(playerRigidbody.velocity.y > 5f && verticalInput > 0){
			verticalOffset = 16f;
		}
		
		float yCameraPosition = transform.position.y + verticalOffset;
		float xCameraPosition = transform.position.x + (horizontalOffset * horizontalInput);
		
		Vector3 targetCameraPosition = new Vector3(xCameraPosition, yCameraPosition, cameraInstance.transform.position.z);
		
		cameraInstance.transform.position = Vector3.Lerp(cameraInstance.transform.position, targetCameraPosition, Time.deltaTime * 1f);
	}
	
	private void Jump(){
		isJumping = true;
		isGrounded = false;
		playerRigidbody.velocity = Vector2.zero;
		playerRigidbody.velocity = Vector2.Lerp(playerRigidbody.velocity, new Vector2(playerRigidbody.velocity.x, (180f * movementSpeed) / (isLongJumping ? jumpingMass : mass)), Time.deltaTime * jumpForceModifier);
		jumpSource.Play();
	}
	
	private void Jet(){
		fuel -= jetFuelCost;
		playerRigidbody.velocity = Vector2.Lerp(playerRigidbody.velocity, new Vector2(playerRigidbody.velocity.x, 1.2f * jetSpeed / mass), Time.deltaTime);
		if(!jet.GetComponent<AudioSource>().isPlaying){
			jet.GetComponent<AudioSource>().Play();
		}
		
	}
	
	private Bullet FirstInactiveBullet(){
		foreach(GameObject g in projectiles){
			if(!g.GetComponent<Rigidbody2D>().simulated){
				g.GetComponent<Rigidbody2D>().simulated = true;
				g.GetComponent<SpriteRenderer>().enabled = true;
				return g.GetComponent<Bullet>();
			} 
		}
		Bullet bullet = Instantiate(Resources.Load("Projectile/Bullet", typeof(Bullet)) as Bullet) as Bullet;				
		projectiles.Add(bullet.gameObject);
		return bullet;
	}
	
	private void Shoot(){
		Bullet bullet = FirstInactiveBullet();
		bullet.transform.position = transform.position + (Vector3.right * facing * 1.5f);
		bullet.Fire();
		gunSprite.enabled = true;
		gunSprite.flipX = facing > 0 ? false : true;
		gun.GetComponent<AudioSource>().Play();
		gunTimer = gunCooldown;
		heat += heatCost;
		if(heat > 100){
			heat = 100;
		}
	}
	
	
	private void Blink(){
		if(playerSprite.color.a < 1f){
			playerSprite.color = new Vector4(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 1f);
		} else {
			playerSprite.color = new Vector4(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, .1f);
		}
	}
	
	public void OnTriggerEnter2D(Collider2D c){
		switch(c.transform.name){
			case "Ability_Jet":
				hasJet = true;
				Destroy(c.gameObject);
				break;
			case "Ability_Gun":
				hasGun = true;
				Destroy(c.gameObject);
				break;
			case "Health":
				health += 3;
				Destroy(c.gameObject);
				break;
			case "Scrap(Clone)":
				scrap++;
				Destroy(c.gameObject);
				break;	
			default:
				break;
		}
	}
	
	public bool CheckGrounding(){
		if(leftHits == null || rightHits == null){	
			leftHits = new RaycastHit2D[raycastBuffer];
			rightHits = new RaycastHit2D[raycastBuffer];
		}
		float botLeft = playerCollider.bounds.min.x;
		float botRight = playerCollider.bounds.max.x;
		float bot = playerCollider.bounds.min.y;
		// float margin = playerCollider.bounds.extents.x;
		
		ContactFilter2D filter = new ContactFilter2D();
		filter.useTriggers = false;
		//filter.useLayerMask = true;
		
		int left = Physics2D.Raycast(new Vector2(botLeft + .1f, bot + .01f), -Vector2.up, filter, leftHits, .1f);
		int right = Physics2D.Raycast(new Vector2(botRight - .1f, bot + .01f), -Vector2.up, filter, rightHits, .1f);
		
		for(int i = 0; i < left; i++){
			RaycastHit2D hit = leftHits[i];
			if(hit.collider != null && hit.collider != playerCollider){
				return true;
			}
		}
		
		for(int i = 0; i < right; i++){
			RaycastHit2D hit = rightHits[i];
			if(hit.collider != null && hit.collider != playerCollider){
				return true;
			}
		}
		
		return false;
	}
	
	public bool CheckFront(){
		if(forwardHits == null){	
			forwardHits = new RaycastHit2D[raycastBuffer];
		}
		Vector2 faceVector = new Vector2(facing, 0f);
		
		ContactFilter2D filter = new ContactFilter2D();
		filter.useTriggers = false;
	
		int hitCount = Physics2D.CapsuleCast(new Vector2(playerCollider.bounds.center.x + (facing * playerCollider.bounds.extents.x), playerCollider.bounds.center.y), new Vector2(.1f, playerCollider.bounds.size.y), CapsuleDirection2D.Vertical, 0f, faceVector, filter, forwardHits, .1f);
				
		for(int i = 0; i < hitCount; i++){
			RaycastHit2D hit = forwardHits[i];
			if(hit.collider != null && hit.collider != playerCollider){
				return true;
			}
		}
		return false;
	}
	
	private void Melee(){
		bool isComboing = false;
		slash.localPosition = new Vector3(.4f * facing, 0f, 0f);
		slash.gameObject.GetComponent<SpriteRenderer>().flipX = facing > 0f ? false : true;
		slash.gameObject.GetComponent<AudioSource>().Play();
		if(isComboing){
		} else {
			slash.gameObject.GetComponent<Slash>().Activate();
		}

		ResetKnifeCooldown();	
	}
	
	public void ResetKnifeCooldown(){
		knifeTimer = knifeCooldown;
	}
	
	public void Combo(){
		knifeTimer = 0;
		knifeReadySource.Play();
	}
	
	private void Move(){
		Vector3 directionalSpeed = Vector2.zero;
		
		if(!isGrounded){
			directionalSpeed = new Vector2((horizontalInput * movementSpeed) / aerialHorizontalImpetus, playerRigidbody.velocity.y);
		} else {
			
			walkTimer -= Time.deltaTime;
			
			if(walkTimer < 0f && useLeftFoot){
				walkLeftSource.Play();
				walkTimer = walkTime;
				useLeftFoot = false;
			} else if(walkTimer <= 0f && !useLeftFoot){
				walkRightSource.Play();
				walkTimer = walkTime;
				useLeftFoot = true;
			}
			directionalSpeed = new Vector2((horizontalInput * movementSpeed), playerRigidbody.velocity.y);
		}
		
		if(CheckFront())
			directionalSpeed = new Vector2(0f, playerRigidbody.velocity.y);
		
		playerRigidbody.velocity = Vector2.Lerp(playerRigidbody.velocity, directionalSpeed, Time.deltaTime * mass);
	}

	public void Respawn(){
		Debug.Log("player reset");
		health -= 1;

		playerRigidbody.velocity = Vector2.zero;
		transform.position = lastGroundedPosition;
	
		if(health <= 0){
			Die();
		}
	}
	
	public void Die(){
		health = maxHealth;
		slash.gameObject.GetComponent<Slash>().Disable();
		Gameplay.Instance().PlayerDeath();
	}
	
	private void ActivateInvulnerability(){
		invulnerableTimer = invulnerableCooldown;
		horizontalInput = 0;
		isInvulnerable = true;
	}
	
	public void Damage(int damageReceived){
		if(invulnerableTimer > 0f || isInvulnerable)
			return;
		
		health -= damageReceived;
		ActivateInvulnerability();
		UI.Instance().playerCombo = 0;
		if(health <= 0){
			playerRigidbody.velocity = Vector2.zero;
			Die();
		}
	}
	
	public void Knockback(){
		playerRigidbody.velocity += new Vector2(facing * 100f, 20f) * Time.deltaTime * mass;
	}
	
	public void ResetPlayer(){
		invulnerableTimer = 0f;
		isInvulnerable = false;
		health = maxHealth;
		heat = 0;
		fuel = maxFuel;
		isGrounded = false;
		slash.gameObject.GetComponent<Slash>().Disable();
	}
	
	private void Inertia(){
		playerRigidbody.velocity = Vector2.Lerp(playerRigidbody.velocity, Vector2.zero, Time.deltaTime * mass);
	}
	
	private void Gravity(){
		float playerMass = isLongJumping ? jumpingMass : mass;
		playerRigidbody.velocity += gravitationalAcceleration * Time.deltaTime * playerMass;
	}
	
	private static Player instance;
	
	public static Player Instance(){
		if(instance == null){
			instance = GameObject.FindObjectOfType<Player>();
		}
		
		return instance;
	}
}