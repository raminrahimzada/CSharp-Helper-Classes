/*
 FPS Constructor - Weapons
 Copyright� Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

/* AimMode controls weapon positioning, aiming, sprinting */

/////////////////////////// CHANGEABLE BY USER ///////////////////////////

var scopeTexture : Texture; //Currently used scope texture
var overrideSprint : boolean = false;
var sprintDuration = 5; //how long can we sprint for?
var sprintAddStand : float = 1; //how quickly does sprint replenish when idle?
var sprintAddWalk : float = .3; //how quickly does sprint replenish when moving?
var sprintMin : float = 1; //What is the minimum value ofsprint at which we can begin sprinting?
var recoverDelay : float = .7;	//how much time after sprinting does it take to start recovering sprint?
var exhaustedDelay : float = 1; //how much time after sprinting to exhaustion does it take to start recovering sprint?
var crosshairWhenAiming : boolean = false;

//Changes to the following variable will only be reflected when AimPrimary() or AimSecondary() are called
//Calling either while aiming is not suggested

//Zoom info for secondary weapon
var zoomFactor2 : float = 1;
var scoped2 : boolean = false;
var sightsZoom2 : boolean = false;
//Zoom info for primary weapon
var zoomFactor1 : float = 1;
var scoped1 : boolean = false;
var sightsZoom1 : boolean = false;

//Set of positions and rotations used for primary weapon
var aimPosition1 : Vector3;
var aimRotation1 : Vector3;
var hipPosition1 : Vector3;
var hipRotation1 : Vector3;
var sprintPosition1 : Vector3;
var sprintRotation1 : Vector3;

//Set of positions and rotations used for secondary weapon
var aimPosition2 : Vector3;
var aimRotation2 : Vector3;
var hipPosition2 : Vector3;
var hipRotation2 : Vector3;
var sprintPosition2 : Vector3;
var sprintRotation2 : Vector3;

private var aimStartTime : float;

///////////////////////// END CHANGEABLE BY USER /////////////////////////



///////////////////////// Internal Variables /////////////////////////
/*These variables should not be modified directly, weither because it could compromise
the functioning of the package, or because changes will be overwritten or otherwise
ignored.
*/





var st169 : Texture; //scope texture for 16 : 9 aspect ration
var st1610: Texture; // 16 :10
var st43 : Texture; // 4 : 3
var st54 : Texture; // 5 : 4
private var player : GameObject; //Player object
var scoped : boolean = false; //Does this weapon use a scope?
private var scopeTime : float; //Time when we should be in scope
var sightsZoom : boolean = false; //Does this weapon zoom when aiming? (Not scoped)
var inScope : boolean = false; //Are we currently scoped?
var aim : boolean = true; //Does the primary weapon aim?
var secondaryAim : boolean = true; //Does the Secondaey weapon Aim?
var canAim : boolean; //does the weapon currently aim?
var aiming : boolean; //are we currently aiming?

static var sprintingPublic : boolean; //are we currently sprinting?
var sprinting : boolean;
var canSprint : boolean; //can the player sprint currently?

private var deltaAngle : Vector3;
private var selected : boolean = false;
static var sprintNum : float;
var aimRate : float = 3;
var sprintRate : float = .4;
var retRate : float = .4;
private var cmra : GameObject;
private var wcmra : GameObject;

var zoomFactor : float = 1; //how much does this zoom in when aiming? (currently)

//Set of positions and rotations used
var aimPosition : Vector3;
var aimRotation : Vector3;
var hipPosition : Vector3;
var hipRotation : Vector3;
var sprintPosition : Vector3;
var sprintRotation : Vector3;

var rotationSpeed : float = 180;
var controller : CharacterController;
private var zoomed : boolean = false;
static var canSwitchWeaponAim : boolean = true;
static var staticAiming : boolean = false;
var hasSecondary : boolean = true;
var GunScript1 : GunScript;
private var curVect : Vector3;
private var sprintEndTime : float = 0;
private var CM : CharacterMotorDB;
static var exhausted : boolean = false;
private var switching : boolean = false;

private var startPosition : Vector3;
private var startRotation : Vector3;
private var moveProgress : float;
static var staticRate : float;


function Start(){
	if(aimRate <= 0)
		aimRate = .3;
	if(!overrideSprint){
		//Get sprint info form MovementValues
		sprintDuration = MovementValues.singleton.sprintDuration;
		sprintAddStand = MovementValues.singleton.sprintAddStand;
		sprintAddWalk = MovementValues.singleton.sprintAddWalk;
		sprintMin = MovementValues.singleton.sprintMin;
		recoverDelay = MovementValues.singleton.recoverDelay;
		exhaustedDelay = MovementValues.singleton.exhaustedDelay;
	}
	AimPrimary();
	cmra = PlayerWeapons.mainCam;
	wcmra = PlayerWeapons.weaponCam;
	player = PlayerWeapons.player;
	sprintNum = sprintDuration;
	canSprint=true;
	aiming = false;
	sprinting = false;
	controller = player.GetComponent(CharacterController);
	if( zoomFactor == 0){
		zoomFactor = 1;
	}
	AspectCheck();
	CM = GameObject.FindWithTag("Player").GetComponent(CharacterMotorDB);
}

function AspectCheck(){
	if(cmra.camera.aspect == 1.6 && st1610 != null){
		scopeTexture = st1610;
	} else if(Mathf.Round(cmra.camera.aspect) == 2 && st169 != null){
		scopeTexture = st169;
	} else if(cmra.camera.aspect == 1.25 && st54 != null){
		scopeTexture = st54;
	} else if(Mathf.Round(cmra.camera.aspect) == 1 && st43 != null){
		scopeTexture = st43;
	}
}

function Update(){	
	if(!GunScript1.gunActive){
		if(transform.localPosition != hipPosition)
			transform.localPosition = hipPosition;
		if(transform.localEulerAngles != hipRotation)
			transform.localEulerAngles = hipRotation;
			sprinting = false;
		return;
	}
	
	staticAiming = aiming;
	
    //Replenish Sprint time
	var tempSprintTime : float;
	if(controller.velocity.magnitude == 0){
		tempSprintTime = sprintEndTime;
	}
	if(sprintNum < sprintDuration  && !sprinting && Time.time > tempSprintTime){
		if(controller.velocity.magnitude == 0){
			sprintNum = Mathf.Clamp(sprintNum + sprintAddStand*Time.deltaTime, 0, sprintDuration);
		}else{
			sprintNum = Mathf.Clamp(sprintNum + sprintAddWalk*Time.deltaTime, 0, sprintDuration);
		}
	}	
	if(sprintNum > sprintMin){
		exhausted = false;
	}
	
	//Turn on scope if it is time
	if ((inScope && !aiming) || (zoomed && !aiming)){
		inScope = false;
		zoomed = false;
		var gos = GetComponentsInChildren(Renderer);
		for( var go : Renderer in gos){
			if (go.name != "muzzle_flash")
				go.enabled=true;
		}
	}
	//Reset Camera
	if(!aiming && cmra.camera.fieldOfView != PlayerWeapons.fieldOfView){
		if(sightsZoom1 && !scoped){
			cmra.camera.fieldOfView = Mathf.Lerp(cmra.camera.fieldOfView, PlayerWeapons.fieldOfView, Mathf.SmoothStep(0,1,1 - (aimStartTime-Time.time)/aimRate));
			wcmra.camera.fieldOfView = Mathf.Lerp(wcmra.camera.fieldOfView, PlayerWeapons.fieldOfView, Mathf.SmoothStep(0,1,1 - (aimStartTime-Time.time)/aimRate));
		} else {
			cmra.camera.fieldOfView = PlayerWeapons.fieldOfView;
			wcmra.camera.fieldOfView = PlayerWeapons.fieldOfView;
		}
	}

	staticRate = aimRate;
    //aiming
	if (InputDB.GetButton("Aim") && canAim && PlayerWeapons.canAim && selected && !sprinting /*&& !GunScript1.sprint*/ && Avoidance.canAim){	
				
		if (!aiming){
			aimStartTime = Time.time + aimRate;
			scopeTime = Time.time + aimRate;
			aiming = true;
			canSwitchWeaponAim = false;
			startPosition = transform.localPosition;
			startRotation = transform.localEulerAngles;
			curVect= aimPosition-transform.localPosition;

			player.BroadcastMessage("Aiming", zoomFactor, SendMessageOptions.DontRequireReceiver);
		}
	
		//Align to position
		GunToRotation(aimRotation, aimRate);
		if (aiming){
			transform.localPosition = Vector3.Slerp(startPosition, aimPosition, Mathf.SmoothStep(0,1,1 - (aimStartTime-Time.time)/aimRate));
		}
		
		//Turn on scope if it's time
		if (scoped && selected && Time.time >= scopeTime && !inScope){
			inScope = true;
			var go = GetComponentsInChildren(Renderer);
			for( var g : Renderer in go){
				if (g.gameObject.name != "Sparks")
					g.enabled=false;
			}
			cmra.camera.fieldOfView = PlayerWeapons.fieldOfView/zoomFactor;
		}
		
		//Otherwise if sights zoom then zoom in camera
		if (sightsZoom && selected && !zoomed && !scoped){
			cmra.camera.fieldOfView = Mathf.Lerp(cmra.camera.fieldOfView, PlayerWeapons.fieldOfView/zoomFactor, Mathf.SmoothStep(0,1,1 - (aimStartTime-Time.time)/aimRate));
			wcmra.camera.fieldOfView = Mathf.Lerp(wcmra.camera.fieldOfView, PlayerWeapons.fieldOfView/zoomFactor, Mathf.SmoothStep(0,1,1 - (aimStartTime-Time.time)/aimRate));
			
			if(cmra.camera.fieldOfView == PlayerWeapons.fieldOfView/zoomFactor){
				zoomed = true;
			}
		}

    //sprinting
	}else if(InputDB.GetButton("Sprint")&& !InputDB.GetButton("Aim") && canSprint && PlayerWeapons.canSprint && selected && !aiming && CM.grounded && !exhausted  && (controller.velocity.magnitude > CM.movement.minSprintSpeed || (/*CM.prone || */CM.crouching))){

		sprintNum = Mathf.Clamp(sprintNum - Time.deltaTime, 0, sprintDuration);
		aiming = false;
		if (!sprinting){
			aimStartTime = Time.time + sprintRate;
			curVect= sprintPosition-transform.localPosition;
			sprinting = true;			
			player.BroadcastMessage("Sprinting", SendMessageOptions.DontRequireReceiver);
			canSwitchWeaponAim = false;	
			startPosition = transform.localPosition;
			startRotation = transform.localEulerAngles;
		}
		
		//Align to position
		transform.localPosition = Vector3.Slerp(startPosition, sprintPosition, Mathf.SmoothStep(0,1,1 - (aimStartTime-Time.time)/sprintRate));
		GunToRotation(sprintRotation, sprintRate);
		
		//Check if we're out of sprint
		if(sprintNum <= 0){
			exhausted = true;
			sprintEndTime = Time.time + recoverDelay;
	}

    //returning to normal		
	} else {
					
		if ((aiming || sprinting || switching)){
			if(sprinting){
				sprintEndTime = Time.time + recoverDelay;
				player.BroadcastMessage("StopSprinting", SendMessageOptions.DontRequireReceiver);
			}
			switching = false;
			aimStartTime = Time.time + retRate;
			startPosition = transform.localPosition;
			startRotation = transform.localEulerAngles;
			sprinting = false;
			canSwitchWeaponAim = true;
			curVect= hipPosition-transform.localPosition;

			SendMessageUpwards("NormalSpeed", SendMessageOptions.DontRequireReceiver);
			if(aiming){
				aiming = false;
				player.BroadcastMessage("StopAiming", SendMessageOptions.DontRequireReceiver);
			}
		}
		
		//Align to position
		transform.localPosition = Vector3.Slerp(startPosition, hipPosition, Mathf.SmoothStep(0,1,1 - (aimStartTime-Time.time)/retRate));
		GunToRotation(hipRotation, retRate);
	}
	staticAiming = aiming;
	sprintingPublic = sprinting;	
}

function DeselectWeapon(){
	selected = false;
	inScope = false;
	aiming = false;
}

function SelectWeapon(){
	selected = true;
	aiming = false;
	SmartCrosshair.displayWhenAiming = crosshairWhenAiming;
}

function AimPrimary () {
	aimPosition = aimPosition1;
	aimRotation = aimRotation1;
	hipPosition = hipPosition1;
	hipRotation = hipRotation1;
	sprintPosition = sprintPosition1;
	sprintRotation = sprintRotation1;
	curVect= hipPosition-transform.localPosition;
	GetGunScript(0);
	zoomFactor = zoomFactor1;
	scoped = scoped1;
	sightsZoom = sightsZoom1;
	canAim = aim;
	switching = true;
}

function AimSecondary(){
	aimPosition = aimPosition2;
	aimRotation = aimRotation2;
	hipPosition = hipPosition2;
	hipRotation = hipRotation2;
	sprintPosition = sprintPosition2;
	sprintRotation = sprintRotation2;
	curVect= hipPosition-transform.localPosition;
	GetGunScript(1);
	zoomFactor = zoomFactor2;
	scoped = scoped2;
	sightsZoom = sightsZoom2;
	canAim = secondaryAim;
	switching = true;
}

function GetGunScript(n : int){
	var GunScripts = transform.parent.GetComponents(GunScript);
	for (var gs : GunScript in GunScripts){
		if(n == 0 && gs.isPrimaryWeapon){
			GunScript1 = gs;
		} else if(n == 1 && !gs.isPrimaryWeapon){
			GunScript1 = gs;
		}
	}
}

function GunToRotation(v3 : Vector3, rate : float){
	transform.localEulerAngles.x = Mathf.LerpAngle(startRotation.x , v3.x, Mathf.SmoothStep(0,1,1 - (aimStartTime-Time.time)/rate));
	transform.localEulerAngles.y = Mathf.LerpAngle(startRotation.y , v3.y, Mathf.SmoothStep(0,1,1 - (aimStartTime-Time.time)/rate));
	transform.localEulerAngles.z = Mathf.LerpAngle(startRotation.z , v3.z, Mathf.SmoothStep(0,1,1 - (aimStartTime-Time.time)/rate));
}

