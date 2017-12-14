/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

var speed = 6.0;
var aimSpeed = 2.0;
var sprintSpeed = 10.0;
private var canSprint : boolean = true;

var sprintJumpSpeed = 8.0;
var normSpeed = 6.0;
var crouchSpeed = 6.0;
var crouchDeltaHeight = 1.0;
var jumpSpeed = 8.0;
var normJumpSpeed = 8.0;
var gravity = 20.0;
private var mainCamera : GameObject;
private var weaponCamera : GameObject;
static var crouching : boolean = false;
private var stopCrouching : boolean = false;
private var standardCamHeight : float;
private var crouchingCamHeight : float;

private var moveDirection = Vector3.zero;
static var grounded : boolean = false;

static var walking : boolean = false;
static var notWalkingTime : float = 0;
private var stopWalkingTime : float;

function Start(){
	speed = normSpeed;
	mainCamera = gameObject.FindWithTag("MainCamera");
	weaponCamera = gameObject.FindWithTag("WeaponCamera");
	crouching = false;
	standardCamHeight = weaponCamera.transform.localPosition.y;
	crouchingCamHeight = standardCamHeight - crouchDeltaHeight;
}

function Update(){
	if(!walking){
		notWalkingTime = Time.time - stopWalkingTime;
	} else {
		notWalkingTime = 0;
	}
	if(weaponCamera.transform.localPosition.y > standardCamHeight){
		weaponCamera.transform.localPosition.y = standardCamHeight;
	} else if(weaponCamera.transform.localPosition.y < crouchingCamHeight){
		weaponCamera.transform.localPosition.y = crouchingCamHeight;
	}

	if (grounded){	
		if (InputDB.GetButtonDown ("Crouch")){
			if(crouching){
				stopCrouching = true;
				NormalSpeed();
				return;
			}

			if(!crouching)
				Crouch();
		}
	}
	
	if(crouching){
		if(weaponCamera.transform.localPosition.y > crouchingCamHeight){
				weaponCamera.transform.localPosition.y = Mathf.Clamp (weaponCamera.transform.localPosition.y - crouchDeltaHeight*Time.deltaTime*8, crouchingCamHeight, standardCamHeight);
		}
	} else {
		if(weaponCamera.transform.localPosition.y < standardCamHeight){
				weaponCamera.transform.localPosition.y = Mathf.Clamp(weaponCamera.transform.localPosition.y + standardCamHeight*Time.deltaTime*8,0, standardCamHeight);
		}
	}

}

function FixedUpdate(){
	if (grounded && PlayerWeapons.canMove){
		// We are grounded, so recalculate movedirection directly from axes
		moveDirection = new Vector3(InputDB.GetAxis("Horizontal"), 0, InputDB.GetAxis("Vertical"));
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= speed;
		
		if (InputDB.GetButton("Jump")) {
			moveDirection.y = jumpSpeed;
			if(crouching){
				stopCrouching = true;
				NormalSpeed();
			}
		}

	}

	// Apply gravity
	moveDirection.y -= gravity * Time.deltaTime;
	
	// Move the controller
	var controller : CharacterController = GetComponent(CharacterController);
	var flags = controller.Move(moveDirection * Time.deltaTime);	
	grounded = (flags & CollisionFlags.CollidedBelow) != 0;
	
	if((Mathf.Abs(moveDirection.x) > 0) && grounded || (Mathf.Abs(moveDirection.z) > 0 && grounded)){
		if(!walking){
			walking = true;
			BroadcastMessage("Walking", SendMessageOptions.DontRequireReceiver);
		}
	} else if(walking){
		walking = false;
		stopWalkingTime=Time.time;
		BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
	}
}

@script RequireComponent(CharacterController)

function Aiming(){
	speed = aimSpeed;
}

function Crouch(){
	speed = crouchSpeed;
	this.GetComponent(CharacterController).height -= crouchDeltaHeight;
	this.GetComponent(CharacterController).center -= Vector3(0,crouchDeltaHeight/2, 0);
	crouching = true;
}

function NormalSpeed(){
	if(stopCrouching){
		crouching = false;
		this.GetComponent(CharacterController).height += crouchDeltaHeight;
		BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
		this.GetComponent(CharacterController).center += Vector3(0,crouchDeltaHeight/2, 0);

		stopCrouching = false;		
	} else if(crouching){
		speed = crouchSpeed;
		return;
	}
		speed = normSpeed;
		jumpSpeed = normJumpSpeed;
}

function Sprinting(){
	if(crouching){
		crouching = false;
		this.GetComponent(BoxCollider).size += Vector3(0,crouchDeltaHeight, 0);
		this.GetComponent(BoxCollider).center += Vector3(0,crouchDeltaHeight, 0); 

	}
	if(canSprint){
		speed = sprintSpeed;
		jumpSpeed = sprintJumpSpeed;
	}
}