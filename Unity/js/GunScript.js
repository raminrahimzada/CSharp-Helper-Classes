#pragma strict
#pragma downcast

/*
 FPS Constructor - Weapons
 Copyright� Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

/////////////////////////// CHANGEABLE BY USER ///////////////////////////
/*These variables can be changed by external scripts when necessary. 
*/

////////// Accuracy Variables //////////

/*Kickback variables: Kickback is the visual motion of the camera when firing.
*/
var kickbackAngle : float; //Vertical kickback per shot (degrees)
var xKickbackFactor : float = .5; //Horizontal kickback per shot (percent of vertical)
var maxKickback : float = 15; //Maximum vertical kickback (degrees)
var kickbackAim : float;
var crouchKickbackMod : float = .6;
var proneKickbackMod : float = .35;
var moveKickbackMod : float = 1.3;

private var curKickback : float;
var recoilDelay : float = .11; //Delay between stopping firing and recoil decreasing

/*Spread variables: Spread (between 0 and 1) determines the accuracy of the weapon.
A spread of 0 means that the weapon will fire exactly towards the crosshair, and
a spread of 1 means that it will fire anywhere within 90 degrees of the crosshair.
*/
var standardSpread = .1; //default spread of this weapon
var maxSpread = .25; //Maximum spread of this weapon
var crouchSpreadModifier = .7; //When crouching, spread is multiplied by this
var proneSpreadModifier = .4; //When prone, spread is multiplied by this
var moveSpreadModifier = 1.5; //When walking, spread is multiplied by this
var standardSpreadRate = .05; //Standard increase in spread per shot
var aimSpreadRate = .01; //Increase in spread per shot when aiming
var aimSpread = .01; //Default spread when aiming
var spDecRate = .05; //Speed at which spread decreases when not firing


////////// Ammo variables //////////
var ammoLeft : float = 0; //Ammo left in the curent clip (ammo before next reload)
var ammoPerClip : int = 40; //Shots per clip
var ammoPerShot : int = 1; //Ammo used per shot
var clips : int = 20; //Number of spare clips (reloads) left
var maxClips : int = 20; //Maximum number of clips
var infiniteAmmo : boolean = false; //Does this gun deplete clips whe reoading?
enum ammoTypes {byClip, byBullet}
var ammoType : ammoTypes = ammoTypes.byClip; //Does this weapon conserve ammo when reloading? (e.g. if you reload after shooting one bullet, does it use a whole clip)


////////// Fire Variables //////////
var fireSound : AudioClip; //Sound that plays when firing
var fireVolume : float = 1;
var firePitch : float = 1;//Pitch of fire sound
var fireRate = 0.05; //Time in seconds between shots
var autoFire : boolean; //Is this weapon automatic (can you hold down the fre button?)
var fireAnim : boolean = false; //Does this weapon's fire animation scale to fit the fire rate (generally used for non-automatic weapons)
var delay : float = 0; //Delay between hitting fire button and actually firing (can be used to sync firing with animation)
var emptySound : AudioClip; //Sound that plays when firing
var emptyVolume : float = 1;
var emptyPitch : float = 1;//Pitch of fire sound

//Burst fire
//note: burst fire doesn't work well with automatic weapons
var burstFire : boolean = false; //does this wepon fire in bursts
var burstCount : int = 1; //shots per burst
var burstTime : float = .5; //time to fire full burst


////////// Reloading variables //////////
var reloadTime = 0.5;
var emptyReloadTime = .4;
var addOneBullet : boolean = false;
var waitforReload = 0.00;

/*Progressive Reload is a different kind of reloading where the reload is broken into stages.
The first stage initializes the animation to get to the second stage. The second stage represents
reloading one shot, and will repeat as many times as necessary to reach a full clip unless
interrupted. Then the third stage returns the weapon to its standrad position. This is useful
for weapons like shotguns that reload by shells.
*/
var progressiveReload : boolean = false; //Does this weapon use progressive reload?
var progressiveReset : boolean = false; //Does this weapon's ammo reset to 0 when starting a reload? 
//(e.g. a revolver where the shells are discarded and replaced)
var reloadInTime = 0.5; //time in seconds for the first stage
var reloadOutTime = 0.5; //time in seconds for the third stage
//the time for the second stage is just reloadTime


////////// Gun-Specific Variables //////////
var range = 100.0; //Range of bullet raycast in meters
var force = 10.0; //Force of bullet
var damage = 5.0; //Damage per bullet
var shotCount : int = 6; //Bullets per shot
var penetrateVal : int = 1; //penetration level of bullet

/* Damage falloff allows raycast weapons to do less damage at long distances
*/
var hasFalloff : boolean = false; //Does this weapon use damage falloff?
var minFalloffDist : float = 10; //Distance at which falloff begins to take effect
var maxFalloffDist : float = 100; //Distance at which falloff stops (minumum damage)
var falloffCoefficient : float = 1; //Coefficient for multiplying damage
var falloffDistanceScale : float = 4; //Scaling value to change speed of falloff


////////// Launcher-Specific Variables //////////
var projectile : Rigidbody; //The object to launch. This can be anything, as long as it has a rigidbody.
var initialSpeed = 20.0; //Initial speed of projectile, applied  forward
var projectileCount : int = 1; //Number of projectiles fired
var launchPosition : GameObject; //GameObject whose position the projectile is fired from (place this at the end of the weapon's barrel, generally)


////////// Tracer related variables //////////
/* Tracers are done using particle emitters.
*/
var tracer : GameObject; //Tracer object. Must have a particle emitter attached
var traceEvery : int = 0; //Activate a tracer evey x shots.
var simulateTime : float = .02; //How long to simulate tracer before it appears
var minDistForTracer : float = 2; //This isn't exposed, but can be tweaked if needed


////////// Sway //////////
var sway : boolean; //Does the weapon sway?
var moveSwayRate : Vector2 = Vector2(2.5, 5); //How fast does the weapon sway when walking? (xy)
var moveSwayAmplitude : Vector2 = Vector2(.04, .01); //How much does the weapon sway when walking? (xy)
var runSwayRate : Vector2 = Vector2(4.5, .9); //How fast does the weapon sway when sprinting? (xy)
var runAmplitude : Vector2 = Vector2(.04, .04); //How much does the weapon sway when sprinting? (xy)
var idleSwayRate : Vector2 = Vector2(2, 1); //How fast does the weapon sway when standing? (xy)
var idleAmplitude : Vector2 = Vector2(.002, .001); //How much does the weapon sway when standing? (xy)


////////// Secondary Weapons //////////
var secondaryWeapon : GunScript; //Gunscript of secondary weapon (additional weapon script on this object)
var secondaryInterrupt : boolean = false; //Can primary and secondary weapon interrupt each other's actions
var secondaryFire : boolean = false; //Is the secondary weapon fired with Mouse2?
var enterSecondaryTime : float = .5; //How long does it take to switch to secondary (animation)?
var exitSecondaryTime : float = .5; //How long does it take to switch from secondary (animation)?


////////// Charge weapon variables //////////
var minCharge : float = 0; //Minimum charge value at ahich the weapon can fire
var maxCharge : float = 10; // Maximum charge value the weapon can have
var chargeLevel : float = 0; //current charge level of the weapon
var forceFire : boolean = false; //Does this weapon have to fire when it hits max charge?
var chargeLoop : AudioClip; //Sound to play when charging
var chargeAuto : boolean = false; //Does the weapon automatically start charging again after a forced release?


//Specifically for hitscan charge weapons
var chargeCoefficient : float = 1.1; //Damage multiplier as charge increases
var additionalAmmoPerCharge : float = 0; //Ammo change as charge increases (add this per 1 charge level)


//////////Other variables//////////
var idleTime : float = 0; //Time in seconds that the player has been idle
var timeToIdle : float = 7; //Time in seconds of being idle which will cause the idle animation to play
var takeOutTime : float = .6; //Time to take out (switch to) weapon
var putAwayTime : float = .6; //Time to put away (switch from) weapon 

//////////Z KickBack//////////
var useZKickBack : boolean = true; //Does this weapon use z kickback?
var kickBackZ : float = 2; //Rate of z kickback when firing
var zRetRate : float = 1; //rate of return from z when not firing
var maxZ : float = .3; //maximum z kickback

//////////Avoidance//////////
//Avoidance is by default handled globall by the Avoidance Component. This just overrides its values for this weapon.
var overrideAvoidance : boolean = false; //Does this weapon override global object avoidance values
var avoids : boolean = true;
var rot : Vector3;
var pos : Vector3;
var dist : float = 2;
var minDist : float = 1.5;

//Shell Ejection
var shellEjection : boolean = false; //Does this weapon use shell ejection?
var ejectorPosition : GameObject; //If it does, this gameobject provides the position where shells are instantiated
var ejectDelay : float = 0;
var shell : GameObject; //The shell prefab to instantiate


//Custom crosshair variables
var scale : boolean = false; //Does the crosshair scale with accuracy?
var crosshairObj : GameObject; //Crosshair object to use
var crosshairSize : float; //Default scale of the crosshair object

///////////////////////// END CHANGEABLE BY USER /////////////////////////



///////////////////////// Internal Variables /////////////////////////
/*These variables should not be modified directly, weither because it could compromise
the functioning of the package, or because changes will be overwritten.
*/


var gunActive : boolean = false; // Is the weapon currently selected & activated


//Status
private var interruptPutAway : boolean = false;
private var progressiveReloading : boolean = false;
var inDelay : boolean = false;
private var m_LastFrameShot = -1;
static var reloading : boolean = false;
var nextFireTime = 0.0;
static var takingOut : boolean = false;
static var puttingAway : boolean = false;

var secondaryActive : boolean = false;
static var crosshairSpread : float = 0;
private var shotSpread : float;
private var actualSpread : float;
private var spreadRate = .05;
var isPrimaryWeapon : boolean = true;
var aim : boolean = false;
var aim2 : boolean = false;
private var pReloadTime : float = 0;
private var stopReload : boolean = false;
private var startPosition : Vector3;
var gunDisplayed : boolean;
private var totalKickBack : float; //How far have we kicked back?

//Components
var ammo : AmmoDisplay;
var sprint : SprintDisplay;
var wepDis : WeaponDisplay; 
static var mainCam : GameObject;
static var weaponCam : GameObject;
private var primaryWeapon : GunScript;
private var player : GameObject;
var aim1 : AimMode;
var mouseY : MouseLookDBJS;
var mouseX : MouseLookDBJS;
var reloadCancel : boolean = false;


////////// Spray //////////
/* Spray weapons are meant to be a simple solution for something that can now be done better with a 
charge weapon.
*/
private var tempAmmo : float = 1;
var sprayOn : boolean = false;
var sprayObj : GameObject;
var sprayScript : SprayScript;
var deltaTimeCoefficient : float = 1;
var forceFalloffCoefficient : float = .99;
var loopSound : AudioClip;
var releaseSound : AudioClip;
var ammoPerSecond : float;

////////// Charge weapon variables //////////
var chargeWeapon : boolean = false; //Is this weapon a charge weapon?
var chargeReleased : boolean = false;
var chargeLocked : boolean = false; 

//Gun Types
enum gunTypes {hitscan, launcher, melee, spray}
var gunType : gunTypes = gunTypes.hitscan;

//Melee
var hitBox : boolean = false;

//Tracer related variables
private var shotCountTracer : int = 0;

//Ammo Sharing
var sharesAmmo : boolean = false;
var shareLoadedAmmo : boolean = false;
var ammoSetUsed : int = 0;
var managerObject : GameObject;
var ammoManagerScript : AmmoManager;

//Effects
var effectsManager : EffectsManager;
var CM : CharacterMotorDB;

//Inspector only variables
var shotPropertiesFoldout : boolean = false;
var firePropertiesFoldout : boolean  = false;
var accuracyFoldout : boolean = false;
var altFireFoldout : boolean = false;
var ammoReloadFoldout : boolean = false;
var audioVisualFoldout : boolean = false;

//Sway (Internal)
var swayStartTime : float = 0;
var swayRate : Vector2;
var swayAmplitude : Vector2;
var overwriteSway : boolean = false;

private var airborne : boolean = false;

function Awake(){
	//if(gunActive){
		var gos = GetComponentsInChildren(Renderer);
		for( var go : Renderer in gos){
			go.enabled=false;
		}
		gunActive = false;
	//}
	startPosition = transform.localPosition;
	if(gunType == gunTypes.spray){
		if(sprayObj){
			sprayScript = sprayObj.GetComponent(SprayScript);
			sprayScript.isActive = false;
		}else{
			Debug.LogWarning("Spray object is undefined; all spray weapons must have a spray object!");
		}
	}
	crosshairSpread = 0;
	managerObject = GameObject.FindWithTag("Manager");
	ammoManagerScript = managerObject.GetComponent(AmmoManager);
	effectsManager = managerObject.GetComponent(EffectsManager);
	aim1 = this.GetComponentInChildren(AimMode);
	ammo = this.GetComponent(AmmoDisplay);
	sprint = aim1.GetComponent(SprintDisplay);
	wepDis = this.GetComponent(WeaponDisplay);
	ammo.enabled = false;
	sprint.enabled = false;
	wepDis.enabled = false;
}

function Start(){
	mainCam = PlayerWeapons.mainCam;
	weaponCam = PlayerWeapons.weaponCam;
	player = PlayerWeapons.player;
	CM = player.GetComponent(CharacterMotorDB);
	mouseY = weaponCam.GetComponent(MouseLookDBJS);
	mouseX = player.GetComponent(MouseLookDBJS);

	if(maxSpread > 1)
		maxSpread = 1;
	

	inDelay = false;
	hitBox = false;
	
	if(sharesAmmo){
		clips = ammoManagerScript.clipsArray[ammoSetUsed];
		maxClips = ammoManagerScript.maxClipsArray[ammoSetUsed];
		infiniteAmmo = ammoManagerScript.infiniteArray[ammoSetUsed];	
	}

	if(!isPrimaryWeapon){
		gunActive = false;
		var wpns = new Array();
		wpns = this.GetComponents(GunScript);
		for(var p : int = 0; p < wpns.length; p++){
			var g : GunScript = wpns[p] as GunScript;
			if(g.isPrimaryWeapon){	
				primaryWeapon = g;
			}
		}
	}
	
	if(!overwriteSway && sway){
		var overSway : CamSway = CamSway.singleton;
		runSwayRate = overSway.runSwayRate;
		moveSwayRate = overSway.moveSwayRate;
		idleSwayRate = overSway.idleSwayRate;
	}
	
	curKickback = kickbackAngle;
	shotSpread = standardSpread;
	spreadRate = standardSpreadRate;
	ammoLeft = ammoPerClip;
	SendMessage("UpdateAmmo", ammoLeft, SendMessageOptions.DontRequireReceiver);
	SendMessage("UpdateClips", clips, SendMessageOptions.DontRequireReceiver);
	swayRate = moveSwayRate;
	swayAmplitude = moveSwayAmplitude;
}

function Aiming(){
	idleTime = 0;
	shotSpread = aimSpread;
	spreadRate = aimSpreadRate;
	curKickback = kickbackAim;
	if(CM.crouching)
		Crouching();
	if(CM.prone)
		Prone();
	if(CM.walking)
		Walking();
	if(!CM.grounded)
		Airborne();		
}

function Crouching () {
	if(aim1.aiming){
		spreadRate = aimSpreadRate*crouchSpreadModifier;
		shotSpread = Mathf.Max(aimSpread*crouchSpreadModifier, shotSpread);
		curKickback = kickbackAim * crouchKickbackMod;
	} else {
		curKickback = kickbackAngle * crouchKickbackMod;
		spreadRate = standardSpreadRate*crouchSpreadModifier;
		shotSpread = Mathf.Max(standardSpread*crouchSpreadModifier, shotSpread);
	}
	
}

function Prone () {
	if(aim1.aiming){
		curKickback = kickbackAim * proneKickbackMod;
		spreadRate = aimSpreadRate*proneSpreadModifier;
		shotSpread = Mathf.Max(aimSpread*proneSpreadModifier, shotSpread);
	} else {
		curKickback = kickbackAngle * proneKickbackMod;
		spreadRate = standardSpreadRate*proneSpreadModifier;
		shotSpread = Mathf.Max(standardSpread*proneSpreadModifier, shotSpread);
	}
}

function Walking () {
	if(aim1.aiming){
		curKickback = kickbackAim * moveKickbackMod;
		spreadRate = aimSpreadRate*moveSpreadModifier;
		shotSpread = Mathf.Max(aimSpread*moveSpreadModifier, shotSpread);
	} else {
		curKickback = kickbackAngle * moveKickbackMod;
		spreadRate = standardSpreadRate*moveSpreadModifier;
		shotSpread = Mathf.Max(standardSpread*moveSpreadModifier, shotSpread);
	}
}

function StopWalking(){
	if(airborne)
		return;
	spreadRate = standardSpreadRate;
	curKickback = kickbackAngle;
	if(shotSpread < standardSpread)
		shotSpread = standardSpread;
	if(aim1.aiming){
		curKickback = kickbackAim;
		spreadRate = aimSpreadRate;
		shotSpread = aimSpread;
	}
}

function Landed() {
	airborne = false;
	spreadRate = standardSpreadRate;
	curKickback = kickbackAngle;
	if(shotSpread < standardSpread)
		shotSpread = standardSpread;
	if(aim1.aiming){
		curKickback = kickbackAim;
		spreadRate = aimSpreadRate;
		shotSpread = aimSpread;
	}
}

function Airborne() {
	airborne = true;
	if(aim1.aiming){
		curKickback = kickbackAim * moveKickbackMod;
		spreadRate = aimSpreadRate*moveSpreadModifier;
		shotSpread = Mathf.Max(aimSpread*moveSpreadModifier, shotSpread);
	} else {
		curKickback = kickbackAngle * moveKickbackMod;
		spreadRate = standardSpreadRate*moveSpreadModifier;
		shotSpread = Mathf.Max(standardSpread*moveSpreadModifier, shotSpread);
	}
}

function StopAiming(){
	idleTime = 0;
	shotSpread = standardSpread;
	spreadRate = standardSpreadRate;
	curKickback = kickbackAngle;
	if(CM.crouching)
		Crouching();
	if(CM.prone)
		Prone();
	if(CM.walking)
		Walking();
	if(!CM.grounded)
		Airborne();
}
function Cooldown () {
	if(!gunActive)
		return;
	ReturnKickBackZ();
	var targ : float;
	if(aim1.aiming){
		targ = aimSpread;
	} else {
		targ = standardSpread;
	}
	if(CM.crouching)
		targ *= crouchSpreadModifier;
	if(CM.prone)
		targ *= proneSpreadModifier;
	if(CM.walking || !CM.grounded)
		targ *= moveSpreadModifier;
	shotSpread = Mathf.Clamp(shotSpread - spDecRate*Time.deltaTime, targ, maxSpread);
}

function Update(){
	if(progressiveReloading){
		if(ammoLeft < ammoPerClip && clips >=1 && !stopReload){
			if(Time.time > pReloadTime){
				BroadcastMessage("ReloadAnim", reloadTime);
				pReloadTime = Time.time + reloadTime;
				ammoLeft++;
				clips--;
				SendMessage("UpdateAmmo", ammoLeft, SendMessageOptions.DontRequireReceiver);
				SendMessage("UpdateClips", clips, SendMessageOptions.DontRequireReceiver);
			}
		} else if(Time.time > pReloadTime) {
			progressiveReloading = false;
			PlayerWeapons.autoFire = autoFire;
			stopReload = false;
			BroadcastMessage("ReloadOut", reloadOutTime);
			reloading=false;
			if(aim){
				aim1.canAim = true;
			}
			aim1.canSprint = true;
			//aim1.canSwitchWeaponAim = true;
			ApplyToSharedAmmo();
		}
	}
	
	if(actualSpread != shotSpread){
		if(actualSpread > shotSpread){
			actualSpread = Mathf.Clamp(actualSpread - Time.deltaTime/4, shotSpread, maxSpread);
		} else {
			actualSpread = Mathf.Clamp(actualSpread + Time.deltaTime/4, 0,  shotSpread);
		}
	}
	
	if(gunActive){
		idleTime += Time.deltaTime;
		if(!PlayerWeapons.autoFire && autoFire){
			SendMessageUpwards("FullAuto");
		}
		if(PlayerWeapons.autoFire && !autoFire){
			SendMessageUpwards("SemiAuto");
		}
		if(!PlayerWeapons.charge && chargeWeapon){
			SendMessageUpwards("Charge");
		}
		if(PlayerWeapons.charge && !chargeWeapon){
			SendMessageUpwards("NoCharge");
		}
	}
}

function LateUpdate(){

	if(InputDB.GetButtonDown("Fire2") && secondaryWeapon != null && !secondaryFire && !aim1.aiming && !Avoidance.collided){
		if(!secondaryWeapon.gunActive){
			ActivateSecondary();
		} else if(secondaryWeapon.gunActive){
			ActivatePrimary();
		}
	}	
	
	if(gunActive){
		if(idleTime > timeToIdle){
			if(!aim1.aiming && !Avoidance.collided){
				BroadcastMessage("IdleAnim", SendMessageOptions.DontRequireReceiver);
			}
			idleTime = 0;
		}
		shotSpread = Mathf.Clamp (shotSpread, 0 , maxSpread);
		crosshairSpread = actualSpread*180/weaponCam.camera.fieldOfView*Screen.height;
	} else {
		return;
	}
	
	if(CM.walking && !aim1.aiming && sway && !CharacterMotorDB.paused){
		//if(swayStartTime > Time.time)
		//	swayStartTime = Time.time;
		WalkSway();
		idleTime = 0;
	} else {
		//swayStartTime = 999999999999999999;
		ResetPosition();
	}
	
	if(chargeLevel > 0 && chargeWeapon){
		if(audio.clip != chargeLoop || !audio.isPlaying){
			audio.clip = chargeLoop;
			audio.loop = true;
			audio.Play();
		}
	} else {
		if(audio.clip == chargeLoop)
		{
			audio.Stop();
		}
	}
	
	// We shot this frame, enable the muzzle flash
	if (m_LastFrameShot == Time.frameCount) {
				} else {			
		// Play sound
		if (audio){
			audio.loop = false;
		}
	}
}

function FireAlt(){
	if(!isPrimaryWeapon){
		AlignToSharedAmmo();
		gunActive = true;
		Fire();
		gunActive = false;
	}
}

function AlignToSharedAmmo(){
	if(sharesAmmo){
		clips = ammoManagerScript.clipsArray[ammoSetUsed];
		maxClips = ammoManagerScript.maxClipsArray[ammoSetUsed];
		infiniteAmmo = ammoManagerScript.infiniteArray[ammoSetUsed];
		SendMessage("UpdateAmmo", ammoLeft, SendMessageOptions.DontRequireReceiver);
		SendMessage("UpdateClips", clips, SendMessageOptions.DontRequireReceiver);	
	}
}

function ApplyToSharedAmmo(){
	if(sharesAmmo){
		ammoManagerScript.clipsArray[ammoSetUsed] = clips;
		ammoManagerScript.maxClipsArray[ammoSetUsed] = maxClips;
		ammoManagerScript.infiniteArray[ammoSetUsed] = infiniteAmmo;
	}
}

function Fire2(){
	if(isPrimaryWeapon && secondaryWeapon != null && gunActive && secondaryFire){
		ApplyToSharedAmmo();
		secondaryWeapon.FireAlt();
	}
}

function Fire(){
	idleTime = 0;
	if (!gunActive || aim1.sprinting || inDelay || LockCursor.unPaused){
		if(gunTypes.spray && sprayOn){
			if(audio){
				if(audio.clip == loopSound){
					audio.Stop();
				}
			sprayOn = false;
			sprayScript.ToggleActive(false);
			}
		}
		return;
	}
	//Melee attack
	if(gunType == gunTypes.melee && nextFireTime < Time.time){
		BroadcastMessage("FireMelee", delay, SendMessageOptions.DontRequireReceiver);
		nextFireTime = Time.time + fireRate;
		inDelay = true;
		hitBox = true;
		audio.clip = fireSound;
		audio.Play();
		yield new WaitForSeconds(delay);
		inDelay = false;
		if(reloadTime > 0)
			BroadcastMessage("ReloadMelee", reloadTime, SendMessageOptions.DontRequireReceiver);
		hitBox = false;
		return;
	}
		
	//Prog reload cancel
	if(progressiveReloading && ammoLeft > 0){
		stopReload = true;
	}

	var b : int = 1; //variable to control burst fire
		
	//Can we fire?
	if ((ammoLeft < ammoPerShot) || (nextFireTime > Time.time) || !gunActive || reloading || Avoidance.collided){
		if(ammoLeft < ammoPerShot && !((nextFireTime > Time.time) || !gunActive || reloading || Avoidance.collided)){
			if(PlayerWeapons.autoReload && clips > 0){
				Reload();
			} else {
				if(isPrimaryWeapon){
					BroadcastMessage("EmptyFireAnim");
				} else {
					BroadcastMessage("SecondaryEmptyFireAnim");
				}
				nextFireTime = Time.time +.3;
			}
			if(!reloading){
				PlayerWeapons.autoFire = false;
				audio.pitch = emptyPitch;
				audio.volume = emptyVolume;
				audio.clip = emptySound;
				audio.Play();
			}
		}
		if(gunType == gunTypes.spray){
			sprayOn = false;
			sprayScript.ToggleActive(false);
		}
		return;
	}
	
	//KickBack
	KickBackZ();

	if(gunType != gunTypes.spray){
	
		//Handle charging
		if(chargeWeapon){
			if(chargeLevel < maxCharge && !chargeLocked && gunActive){
				if(ammoPerShot + additionalAmmoPerCharge * chargeLevel >= ammoLeft && additionalAmmoPerCharge != 0){
					chargeReleased = true;
					chargeLocked = true;
				}else{
					chargeLevel += Time.deltaTime;
				}
			}else if(forceFire && chargeLocked == false){
				chargeReleased = true;
				if(!chargeAuto){
					chargeLocked = true;
				}
			}
		}
		
		//Handle firing
		if(!chargeWeapon || (chargeWeapon && chargeReleased)){
			if(chargeWeapon){
				chargeReleased = false;
			}
			if(burstFire){
				b = burstCount;
			} else {
				b = 1;
			}
			for(var i=0; i<b; i++){
				if(ammoLeft >= ammoPerShot){
					FireShot();
					if(chargeWeapon){
						ammoLeft -= ammoPerShot + Mathf.Floor(additionalAmmoPerCharge * chargeLevel);
					}else{
						ammoLeft-= ammoPerShot;
					}
					if(fireRate<delay){	
						nextFireTime = Time.time+delay;
					} else {
						nextFireTime = Time.time+fireRate;
					}
					if(secondaryWeapon != null && secondaryFire && !secondaryWeapon.secondaryInterrupt){
						if(fireRate<delay){	
							secondaryWeapon.nextFireTime = Time.time+delay;
						} else {
							secondaryWeapon.nextFireTime = Time.time+fireRate;
						}
					} else if(secondaryFire && !secondaryInterrupt && !isPrimaryWeapon){
						if(fireRate<delay){	
							primaryWeapon.nextFireTime = Time.time+delay;
						} else {
							primaryWeapon.nextFireTime = Time.time+fireRate;
						}

					}
					if(burstFire && i < (b-1)){
						if(burstTime/burstCount<delay){	
							yield new WaitForSeconds(delay);
						} else {
							yield new WaitForSeconds(burstTime/burstCount);
						}
					}
				}		
			}
		}
	}else if(gunType == gunTypes.spray){
		FireSpray();
	}
	ApplyToSharedAmmo();
	
	SendMessage("UpdateAmmo", ammoLeft, SendMessageOptions.DontRequireReceiver);
	SendMessage("UpdateClips", clips, SendMessageOptions.DontRequireReceiver);
	if (ammoLeft <= 0 && PlayerWeapons.autoReload){
		if(fireRate<delay){
			yield new WaitForSeconds(delay);
		} else {
			yield new WaitForSeconds(fireRate);
		}
		Reload();
	}
}

//Kickback function which moves the gun transform backwards when called
function KickBackZ(){
	if(!useZKickBack)
		return;
	var amt : float = Time.deltaTime*kickBackZ;
	amt = Mathf.Min(amt, maxZ - totalKickBack);
	transform.localPosition.z -= amt;
	totalKickBack += amt;
}

//Reset Kickback function which moves the gun transform forwards when called
function ReturnKickBackZ(){
	var amt : float = Time.deltaTime*zRetRate;
	amt = Mathf.Min(amt, totalKickBack);
	transform.localPosition.z += amt;
	totalKickBack -= amt;
}

function FireShot(){
	if(isPrimaryWeapon){
		if(fireAnim && !autoFire && !burstFire){
			BroadcastMessage("SingleFireAnim", fireRate, SendMessageOptions.DontRequireReceiver);
		} else {
			BroadcastMessage("FireAnim", SendMessageOptions.DontRequireReceiver);
		}
	} else {
		if(fireAnim && !autoFire && !burstFire){
			BroadcastMessage("SingleSecFireAnim", fireRate, SendMessageOptions.DontRequireReceiver);
		} else {
			BroadcastMessage("SecondaryFireAnim", SendMessageOptions.DontRequireReceiver);
		}
	}
	if(shellEjection && !aim1.inScope)
		EjectShell();
	if(gunType == gunTypes.hitscan){
		inDelay = true;
		yield new WaitForSeconds(delay);
		inDelay = false;
		for (var i=0; i<shotCount; i++) {
			FireOneBullet();
			Kickback();
		}
	} else if (gunType == gunTypes.launcher){
		inDelay = true;
		yield new WaitForSeconds(delay);
		inDelay = false;
		for (var p=0; p<projectileCount; p++) {
			FireOneProjectile();
		}
	}	
		m_LastFrameShot = Time.frameCount;
		shotSpread = Mathf.Clamp(shotSpread + spreadRate, 0, maxSpread);
		chargeLevel = 0;
		FireEffects ();
}
	
function FireOneProjectile(){
	var direction : Vector3 = SprayDirection();
	var convert : Quaternion = Quaternion.LookRotation(direction);
	/*var layer1 = 1 << PlayerWeapons.playerLayer;
	var layer2 = 1 << 2;
  	var layerMask = layer1 | layer2;
  	layerMask = ~layerMask;*/
	var instantiatedProjectile : Rigidbody;
	var launchPos : Transform;
	if(launchPosition != null && !Physics.Linecast(launchPosition.transform.position, weaponCam.transform.position, ~(PlayerWeapons.PW.RaycastsIgnore.value))){
		launchPos = launchPosition.transform;
	} else {
		launchPos = weaponCam.transform;
	}
	instantiatedProjectile = Instantiate (projectile, launchPos.position, convert);
	instantiatedProjectile.velocity = instantiatedProjectile.transform.TransformDirection(Vector3 (0, 0, initialSpeed));
	instantiatedProjectile.transform.rotation = launchPos.transform.rotation;
	Physics.IgnoreCollision(instantiatedProjectile.collider, transform.root.collider);
	Physics.IgnoreCollision(instantiatedProjectile.collider, player.transform.collider);
	instantiatedProjectile.BroadcastMessage("ChargeLevel", chargeLevel, SendMessageOptions.DontRequireReceiver);
	Kickback();
}

function FireOneBullet(){  
	var penetrate : boolean = true;
	var pVal : int = penetrateVal;
	/*var layer1 = 1 << PlayerWeapons.playerLayer;
	var layer2 = 1 << 2;
  	var layerMask = layer1 | layer2;
  	layerMask = ~layerMask;*/
  	var hits : RaycastHit[];
	//var direction = SprayDirection();
	var ray : Ray = mainCam.camera.ScreenPointToRay(Vector2(Screen.width/2, Screen.height/2));
	ray.direction = SprayDirection(ray.direction);
  	hits = Physics.RaycastAll (ray, range, ~PlayerWeapons.PW.RaycastsIgnore.value);
  	
  	//Tracer
  	shotCountTracer += 1;
  	if(tracer != null && traceEvery <= shotCountTracer && traceEvery != 0){
  		var emitter : ParticleEmitter = tracer.GetComponent(ParticleEmitter);
  		shotCountTracer = 0;
  		/*if(hits.length > 0){
  			if(Vector3.Distance(hits[0].point, transform.position) >= minDistForTracer){
  				tracer.transform.LookAt(hits[0].point);
  				emitter.Emit(); //This code is written twice because if there is a hit it may not happen
  				emitter.GetComponent(ParticleEmitter).Simulate(simulateTime);
  			}
  		}else{*/
		tracer.transform.rotation = Quaternion.LookRotation(ray.direction);//(transform.position + 90 * direction));
		emitter.Emit();
		emitter.Simulate(simulateTime);
  		//}
  	}
	System.Array.Sort(hits, Comparison);  

//	 Did we hit anything?
	for (var i=0;i<hits.length;i++){
        var hit : RaycastHit = hits[i];
        var BP : BulletPenetration = hit.transform.GetComponent(BulletPenetration);
        if(penetrate){
       		if(BP == null){
        		penetrate = false;
       		} else {
       			if(pVal < BP.penetrateValue){
       				penetrate = false;
       			} else {
       				pVal -= BP.penetrateValue;
       			}	
       		}
       		
       		//Apply charge if applicable
       		var chargedDamage : float = damage;
       		if(chargeWeapon){
       			chargedDamage = damage * Mathf.Pow(chargeCoefficient, chargeLevel);
       		}
       		       		
       		//Calculate damage falloff
       		var finalDamage : float = chargedDamage;
       		var hitDist : float;
       		if(hasFalloff){
      	 		hitDist = Vector3.Distance(hit.transform.position, transform.position);
      	 		if(hitDist > maxFalloffDist){
      	 			finalDamage = chargedDamage * Mathf.Pow(falloffCoefficient, (maxFalloffDist - minFalloffDist)/falloffDistanceScale);
       			}else if(hitDist < maxFalloffDist && hitDist > minFalloffDist){
       				finalDamage = chargedDamage * Mathf.Pow(falloffCoefficient, (hitDist - minFalloffDist)/falloffDistanceScale);
       			}
       		}

			// Send a damage message to the hit object
			/*var sendArray : Object[] = new Object[2];
			sendArray[0] = finalDamage;
			sendArray[1] = true;	*/	
			hit.collider.SendMessageUpwards("ApplyDamagePlayer", finalDamage, SendMessageOptions.DontRequireReceiver);
			//hit.collider.SendMessageUpwards("Accuracy", SendMessageOptions.DontRequireReceiver);
			//And send a message to the decal manager, if the target uses decals
			if(hit.transform.gameObject.GetComponent(UseEffects)){
				//The effectsManager needs five bits of information
				var hitRotation : Quaternion = Quaternion.FromToRotation(Vector3.up, hit.normal);
				var hitSet : int = hit.transform.gameObject.GetComponent(UseEffects).setIndex;
				var hitInfo = new Array(hit.point, hitRotation, hit.transform, hit.normal, hitSet);		
				effectsManager.SendMessage("ApplyDecal", hitInfo, SendMessageOptions.DontRequireReceiver);
			}
			
			//Calculate force falloff
			var finalForce : float = force;
			if(hasFalloff){
				if(hitDist > maxFalloffDist){
      	 			finalForce = finalForce * Mathf.Pow(forceFalloffCoefficient, (maxFalloffDist - minFalloffDist)/falloffDistanceScale);
       			}else if(hitDist < maxFalloffDist && hitDist > minFalloffDist){
       				finalForce = finalForce * Mathf.Pow(forceFalloffCoefficient, (hitDist - minFalloffDist)/falloffDistanceScale);
       			}
			}
			
			// Apply a force to the rigidbody we hit
			if (hit.rigidbody)
				hit.rigidbody.AddForceAtPosition(finalForce * ray.direction, hit.point);
		}
	}
}

function FireSpray(){
	if(!sprayOn){
		sprayOn = true;
		sprayScript.ToggleActive(true);
		audio.clip = fireSound;
		audio.Play();
	}
	if(audio.clip == loopSound && audio.isPlaying && AimMode.sprintingPublic){
		audio.Stop();
	}else if(audio && !audio.isPlaying && !AimMode.sprintingPublic){
		audio.clip = loopSound;
		audio.loop = true;
		audio.Play();
	}
	if(tempAmmo <= 0){
		tempAmmo = 1;
		ammoLeft -= ammoPerShot;
	}else{
		tempAmmo -= Time.deltaTime * deltaTimeCoefficient;
	}
}

function ReleaseFire(key : int){
	if(audio){
		if(audio.isPlaying && audio.clip == chargeLoop){
			audio.Stop();
		}
	}
	if(sprayOn){
		sprayScript.ToggleActive(false);
		sprayOn = false;
		if(audio){
			audio.clip = releaseSound;
			audio.loop = false;
			audio.Play();
		}
	}
	if(chargeWeapon){
		if(chargeLocked){
			chargeLocked = false;
			chargeLevel = 0;
		}else if(chargeLevel > minCharge){
			chargeReleased = true;
			Fire();
		}else{
			chargeLevel = 0;
		}
	}
}

function Comparison(x : RaycastHit, y : RaycastHit) : int { 
   return Mathf.Sign(x.distance - y.distance); 
}

function SprayDirection(){
	var vx = (1 - 2 * Random.value) * actualSpread;
	var vy = (1 - 2 * Random.value) * actualSpread;
	var vz = 1.0;
	return weaponCam.transform.TransformDirection(Vector3(vx,vy,vz));
}

function SprayDirection(dir : Vector3){
	var vx = (1 - 2 * Random.value) * actualSpread;
	var vy = (1 - 2 * Random.value) * actualSpread;
	var vz = (1 - 2 * Random.value) * actualSpread;
	return dir + Vector3(vx, vy, vz);
}

function Reload(){
	if(ammoLeft >= ammoPerClip || clips <= 0 || !gunActive || Avoidance.collided){
		return;
	}
	reloadCancel = false;
	idleTime = 0;
	aim1.canSprint = PlayerWeapons.PW.reloadWhileSprinting;
	if(progressiveReload){
		ProgReload();
		return;
	}
	
	if(reloading)
		return;
		
	//aim1.canSwitchWeaponAim = false;
	if(aim1.canAim){
		aim1.canAim = false;
		aim = true;
	}
	if(gunType == gunTypes.spray){
		if(audio){
			if(audio.clip == loopSound && audio.isPlaying){
				audio.Stop();
			}
		}
	}
	reloading=true;
	if(secondaryWeapon != null){
		secondaryWeapon.reloading = true;
	} else if(!isPrimaryWeapon){
		primaryWeapon.reloading = true;
	}
	var tempEmpty : boolean;
	yield WaitForSeconds(waitforReload);
	if(reloadCancel){
		return;
	}
	
	if(isPrimaryWeapon){					
		BroadcastMessage("ReloadAnimEarly", SendMessageOptions.DontRequireReceiver);
		if(ammoLeft >= ammoPerShot){
			tempEmpty = false;
			BroadcastMessage("ReloadAnim", reloadTime, SendMessageOptions.DontRequireReceiver);
		} else {
			tempEmpty = true;
			BroadcastMessage("ReloadEmpty", emptyReloadTime, SendMessageOptions.DontRequireReceiver);
		}
	} else {
		BroadcastMessage("SecondaryReloadAnimEarly", SendMessageOptions.DontRequireReceiver);
		if(ammoLeft >= ammoPerShot){
			tempEmpty = false;
			BroadcastMessage("SecondaryReloadAnim", reloadTime, SendMessageOptions.DontRequireReceiver);
		} else {
			tempEmpty = true;
			BroadcastMessage("SecondaryReloadEmpty", emptyReloadTime, SendMessageOptions.DontRequireReceiver);
		}
	}	

	// Wait for reload time first - then add more bullets!
	if(ammoLeft > ammoPerShot){
		yield WaitForSeconds(reloadTime);
	} else {
		yield WaitForSeconds(emptyReloadTime);
	}
	if(reloadCancel){
		return;
	}
	reloading = false;
	if(secondaryWeapon != null){
		secondaryWeapon.reloading = false;
	} else if(!isPrimaryWeapon){
		primaryWeapon.reloading = false;
	}
	// We have a clip left reload
	if(ammoType == ammoTypes.byClip){
		if (clips > 0) {
			if(!infiniteAmmo)
				clips--;
			ammoLeft = ammoPerClip;
		}
	} else if (ammoType == ammoTypes.byBullet){
		if(clips > 0){
				if(clips > ammoPerClip){
					if(!infiniteAmmo)
						clips -= ammoPerClip - ammoLeft;
	
					ammoLeft = ammoPerClip;
			 	} else {
					var ammoVal : float = Mathf.Clamp(ammoPerClip, clips, ammoLeft+clips);
					if(!infiniteAmmo)
						clips -= (ammoVal - ammoLeft);
						
					ammoLeft = ammoVal;
				}
			}	
		}
		if(!tempEmpty && addOneBullet){
			if(ammoType == ammoTypes.byBullet && clips > 0){
				ammoLeft+=1;
				clips -=1;
			}
		}
	if(aim)
		aim1.canAim = true;
	aim1.canSprint = true;
	//aim1.canSwitchWeaponAim = true;
	SendMessage("UpdateAmmo", ammoLeft, SendMessageOptions.DontRequireReceiver);
	SendMessage("UpdateClips", clips, SendMessageOptions.DontRequireReceiver);
	ApplyToSharedAmmo();
	PlayerWeapons.autoFire = autoFire;
} 

function StopReloading () { 
	reloading = false;
	if(secondaryWeapon != null){
		secondaryWeapon.reloading = false;
	} else if(!isPrimaryWeapon){
		primaryWeapon.reloading = false;
	}  
	progressiveReloading = false;
	aim1.canSprint = true; 
	PlayerWeapons.autoFire = autoFire;
	if(aim)
		aim1.canAim = true;
}

function ProgReload(){
	if(reloading)
		return;
	//aim1.canSwitchWeaponAim = false;
	if(aim1.canAim){
		aim1.canAim = false;
		aim = true;
	}
	
	BroadcastMessage("ReloadIn", reloadInTime);
	yield WaitForSeconds(reloadInTime);
	if(reloadCancel)
		return;
	if(progressiveReset){
		clips += ammoLeft;
		ammoLeft = 0;
	}
	
	progressiveReloading = true;
	reloading=true;
	if(secondaryWeapon != null && secondaryFire && !secondaryWeapon.secondaryInterrupt){
		secondaryWeapon.reloading = true;
	} else if(secondaryFire && !secondaryInterrupt && !isPrimaryWeapon){
		primaryWeapon.reloading = false;
	}
}

function GetBulletsLeft(){
	return ammoLeft;
}

function SelectWeapon(){	
	AlignToSharedAmmo();
	idleTime = 0;	
	if(!isPrimaryWeapon || puttingAway){
		return;
	}
	if(!mainCam)
		mainCam = PlayerWeapons.mainCam;
	SetCrosshair();

	if(overrideAvoidance){
		Avoidance.SetValues(rot, pos, dist, minDist, avoids);
	} else {
		Avoidance.SetValues();
	}
	
	if(secondaryWeapon != null){
		secondaryWeapon.gunActive = false;
		secondaryWeapon.secondaryActive = false;
		BroadcastMessage("AimPrimary", SendMessageOptions.DontRequireReceiver);
	}
	if(!takingOut && !gunActive){
		var gos = GetComponentsInChildren(Renderer);
		for( var go : Renderer in gos){
			go.enabled=true;
		}
	
		wepDis.enabled = true;
		aim1.canSwitchWeaponAim = false;
		BroadcastMessage("TakeOutAnim", takeOutTime, SendMessageOptions.DontRequireReceiver);
		mainCam.SendMessage("TakeOutAnim", takeOutTime, SendMessageOptions.DontRequireReceiver);
		takingOut = true;
		interruptPutAway = true;
		yield WaitForSeconds(takeOutTime);
		if(puttingAway){
			return;
		}
		//	return;
		SmartCrosshair.crosshair = true;
		gunActive = true;
		takingOut = false;
		aim1.canSwitchWeaponAim = true;
		ammo.enabled = true;
		sprint.enabled = true;
		wepDis.Select();
		SendMessage("UpdateAmmo", ammoLeft, SendMessageOptions.DontRequireReceiver);
		SendMessage("UpdateClips", clips, SendMessageOptions.DontRequireReceiver);
		NormalSpeed();
		
		if(gos.length > 0)
			if(gos[0].renderer.enabled == false)
				for( var go : Renderer in gos){
					go.enabled=true;
			}
			
		if(PlayerWeapons.autoReload && ammoLeft <= 0 && gunType != gunTypes.melee){
			Reload();
		}
	}
}

function DeselectWeapon(){
	if(audio){
		if(audio.clip == loopSound && audio.isPlaying){
			audio.Stop();
		}
	}
	chargeLevel = 0;
	reloadCancel = true;
	reloading = false;
	if(!gunActive)
		return; 
	StopReloading();
	interruptPutAway = false;
	puttingAway = true;
	takingOut = false;
	ammo.enabled = false;
	sprint.enabled = false;
	wepDis.enabled = false;
	aim1.canSwitchWeaponAim = false;
	BroadcastMessage("PutAwayAnim", putAwayTime, SendMessageOptions.DontRequireReceiver);
	mainCam.SendMessage("PutAwayAnim", putAwayTime, SendMessageOptions.DontRequireReceiver);
	gunActive=false;
	SmartCrosshair.crosshair = false;
	yield WaitForSeconds(putAwayTime);
	puttingAway = false;
	
	/*if(takingOut || interruptPutAway){
		return;		
	}*/
	
	SendMessageUpwards("ActivateWeapon");
	var gos = GetComponentsInChildren(Renderer);
	for( var go : Renderer in gos){
		go.enabled=false;
	}
}
function DeselectInstant(){
	if(audio){
		if(audio.clip == loopSound && audio.isPlaying){
			audio.Stop();
		}
	}
	chargeLevel = 0;
	if(!gunActive)
		return;
	takingOut = false;
	ammo.enabled = false;
	sprint.enabled = false;
	wepDis.enabled = false;
	gunActive=false;
	SmartCrosshair.crosshair = false;
	//SendMessageUpwards("ActivateWeapon");
	var gos = GetComponentsInChildren(Renderer);
	for( var go : Renderer in gos){
		go.enabled=false;
	}
	BroadcastMessage("DeselectWeapon");
}


function EditorSelect(){
	gunActive = true;
	var gos = GetComponentsInChildren(Renderer);
	for( var go : Renderer in gos){
		go.enabled=true;
	}
}

function EditorDeselect(){
	gunActive = false;
	var gos = GetComponentsInChildren(Renderer);
	for( var go : Renderer in gos){
		go.enabled=false;
	}
}

function WalkSway(){	
	var speed : int = CM.GetComponent(CharacterController).velocity.magnitude;
	if(speed < .2){
		ResetPosition();
		return;
	}
	if(!sway || !gunActive)
		return;
		
	//sine function for motion
	var t : float = Time.time - CamSway.singleton.swayStartTime;
	var curVect : Vector3;
	/*if(CM.crouching){
		swayRate = moveSwayRate*CM.movement.maxCrouchSpeed/CM.movement.defaultForwardSpeed;
	} else if (CM.prone) {
		swayRate = moveSwayRate*CM.movement.maxProneSpeed/CM.movement.defaultForwardSpeed;
	} else if (AimMode.sprintingPublic) {
		swayRate = runSwayRate;
	} else {
		swayRate = moveSwayRate;
	}*/
	curVect.x = swayAmplitude.x*Mathf.Sin(swayRate.x*t +(idleSwayRate.x/2))*Mathf.Sin(swayRate.x*t+(idleSwayRate.x/2));
	curVect.y = Mathf.Abs(swayAmplitude.y*Mathf.Sin(swayRate.y*t+(idleSwayRate.y/2)));
	
	curVect.x -= swayAmplitude.x/2;
	curVect.y -= swayAmplitude.y/2;
		
	//offset from start position
	curVect += startPosition;
	
	var s : float = Vector3(PlayerWeapons.CM.movement.velocity.x, 0, PlayerWeapons.CM.movement.velocity.z).magnitude/14;
	
	//move towards target
	transform.localPosition.x = Mathf.Lerp(transform.localPosition.x, curVect.x, Time.deltaTime*swayRate.x*s);
	transform.localEulerAngles.z = Mathf.LerpAngle(transform.localEulerAngles.z, -curVect.x, Time.deltaTime*s);
	
	transform.localPosition.y = Mathf.Lerp(transform.localPosition.y, curVect.y, Time.deltaTime*swayRate.y*s);
	transform.localEulerAngles.x = Mathf.LerpAngle(transform.localEulerAngles.x, -curVect.y, Time.deltaTime*swayRate.y*s);
}

function ResetPosition(){
	if(((transform.localPosition == startPosition) && !sway) || ! gunActive){
		return;
	}
	var rate : float = .15*Time.deltaTime;
	
	if(sway && !aim1.aiming){
		//sine function for idle motion
		var curVect : Vector3;
		curVect.x = idleAmplitude.x*Mathf.Sin(idleSwayRate.x*Time.time);
		curVect.y = idleAmplitude.y*Mathf.Sin(idleSwayRate.y*Time.time);
		curVect.x -= idleAmplitude.x/2;
		curVect.y -= idleAmplitude.y/2;
	
		//offset from start position
		curVect += startPosition;
	} else {
		curVect = startPosition;
	}
	//move towards target
	transform.localPosition.x = Mathf.Lerp(transform.localPosition.x, curVect.x, Time.deltaTime*swayRate.x);
	transform.localEulerAngles.z = Mathf.LerpAngle(transform.localEulerAngles.z, curVect.x, Time.deltaTime*swayRate.x);
	
	transform.localPosition.y = Mathf.Lerp(transform.localPosition.y, curVect.y, Time.deltaTime*swayRate.y);
	transform.localEulerAngles.x = Mathf.LerpAngle(transform.localEulerAngles.x, curVect.y, Time.deltaTime*swayRate.y);
}

function Sprinting(){
	if(!gunActive)
		return;
	idleTime = 0;
	PlayerWeapons.sprinting = true;
	swayRate = runSwayRate;
	swayAmplitude = runAmplitude;
	
	//Only affects charge weapons
	if(chargeWeapon)
	{
		chargeLocked = true;
		chargeLevel = 0;
	}
}

function NormalSpeed(){
	if(airborne)
		return;
	PlayerWeapons.sprinting = false;
	if(secondaryWeapon != null){
		if(isPrimaryWeapon && secondaryWeapon.secondaryActive)
			return;
	}
	if(!isPrimaryWeapon && !secondaryActive)
		return;
	swayRate = moveSwayRate;
	if(CM.crouching){
		swayRate = moveSwayRate*CM.movement.maxCrouchSpeed/CM.movement.defaultForwardSpeed;
	} else if (CM.prone) {
		swayRate = moveSwayRate*CM.movement.maxProneSpeed/CM.movement.defaultForwardSpeed;
	}
	swayAmplitude = moveSwayAmplitude;
	//gunActive = true;
	
	//Only affects charge weapons
	if(chargeWeapon)
	{
		chargeLocked = false;
		chargeLevel = 0;
	}
}

function Kickback(){
	mouseY.offsetY = curKickback;
	mouseY.maxKickback = maxKickback;
	mouseX.offsetX = curKickback*xKickbackFactor;//*Random.value;
	mouseX.maxKickback = maxKickback;
	if(mouseY.offsetY < mouseY.maxKickback)
		mouseY.resetDelay = recoilDelay;
	if(Mathf.Abs(mouseX.offsetX) < mouseX.maxKickback)
		mouseX.resetDelay = recoilDelay;
}

function ActivateSecondary(){
	if(secondaryWeapon == null || secondaryFire || reloading)
		return;
	AlignToSharedAmmo();
	if(gunActive){
		SmartCrosshair.crosshair = false;
		gunActive = false;
		BroadcastMessage("EnterSecondary", enterSecondaryTime);
		yield WaitForSeconds(enterSecondaryTime);
		SmartCrosshair.crosshair = true;
		secondaryWeapon.gunActive = true;
		secondaryActive = true;
		secondaryWeapon.SetCrosshair();
		SendMessage("UpdateAmmo", secondaryWeapon.ammoLeft, SendMessageOptions.DontRequireReceiver);
		SendMessage("UpdateClips", secondaryWeapon.clips, SendMessageOptions.DontRequireReceiver);
		BroadcastMessage("AimSecondary", SendMessageOptions.DontRequireReceiver);
	}
}

function SetCrosshair(){
	if(crosshairObj != null){
		weaponCam.GetComponent(SmartCrosshair).SetCrosshair();
		SmartCrosshair.cObj = crosshairObj;
		SmartCrosshair.cSize = crosshairSize;
		SmartCrosshair.scl = scale;
		SmartCrosshair.sclRef = maxSpread;
		SmartCrosshair.ownTexture = true;
	}else{
		SendMessageUpwards("DefaultCrosshair");
	}
}

function ActivatePrimary(){
	AlignToSharedAmmo();
	if(reloading)
		return;
	if(!gunActive){
		secondaryWeapon.gunActive = false;
		secondaryActive = false;
		SmartCrosshair.crosshair = false;
		BroadcastMessage("ExitSecondary", exitSecondaryTime);
		yield WaitForSeconds(exitSecondaryTime);
		SmartCrosshair.crosshair = true;
		gunActive = true;
		SetCrosshair();
		SendMessage("UpdateAmmo", ammoLeft, SendMessageOptions.DontRequireReceiver);
		SendMessage("UpdateClips", clips, SendMessageOptions.DontRequireReceiver);
		BroadcastMessage("AimPrimary", SendMessageOptions.DontRequireReceiver);
	}
}

function EjectShell(){
	yield new WaitForSeconds (ejectDelay);
	var instantiatedProjectile1 = Instantiate (shell, ejectorPosition.transform.position, ejectorPosition.transform.rotation);
}

function FireEffects(){
	var scoped : boolean = transform.Find("AimObject").GetComponent(AimMode).inScope;
	if(!scoped)
		BroadcastMessage("MuzzleFlash", isPrimaryWeapon, SendMessageOptions.DontRequireReceiver);
		
	if(fireSound == null)
		return;
	//Play Audio
	var audioObj : GameObject = new GameObject("GunShot");
	audioObj.transform.position = transform.position;
	audioObj.transform.parent = transform;
	audioObj.AddComponent(TimedObjectDestructorDB).timeOut = fireSound.length + .1;;
	var aO : AudioSource = audioObj.AddComponent(AudioSource);
	aO.clip = fireSound;
	aO.volume = fireVolume;
	aO.pitch = firePitch;
	aO.Play();
	aO.loop = false;
	aO.rolloffMode = AudioRolloffMode.Linear;
}

//Returns primary gunscript on this weapon
function GetPrimaryGunScript() : GunScript {
	if(isPrimaryWeapon){
		return this;
	} else {
		return primaryWeapon;
	}
}