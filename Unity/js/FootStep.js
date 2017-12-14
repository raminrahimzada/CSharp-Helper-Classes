/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

var footstepInterval : float = .5;
var footstepVolume : float = .5;
private var distanceMoved : float = 0;
private var lastStep : Vector3;
private var landing : boolean = false;

@HideInInspector
var effectsManager : EffectsManager;
@HideInInspector
var characterMotor : CharacterMotorDB;
@HideInInspector
var source : AudioSource;
@HideInInspector
var soundClip : AudioClip;
@HideInInspector
var playDex : int = 0;
@HideInInspector
var surface : GameObject;

function Awake(){
	effectsManager = GameObject.FindObjectOfType(EffectsManager);
	characterMotor = gameObject.GetComponent(CharacterMotorDB);
	source = gameObject.GetComponent(AudioSource);
}

function Update(){
	if(!PlayerWeapons.playerActive)
		return;
	
	if(!characterMotor.grounded){
		distanceMoved = footstepInterval;
		landing = true;
	}
	distanceMoved += Vector3.Distance(transform.position, lastStep);
	lastStep = transform.position;
	if(characterMotor.walking)  {//|| (landing && characterMotor.grounded))){
		if(characterMotor.prone){
			Crawl();
			landing = false;
		} else {
			Footstep();
			landing = false;
		}
	}
}

function Airborne () {
	if(characterMotor.prone){
		Crawl();
		landing = false;
	} else {
		Footstep();
		landing = false;
	}
}

function Landed () {
	if(characterMotor.prone){
		Crawl();
		landing = false;
	} else {
		Footstep();
		landing = false;
	}
}

function Footstep(){
	if(distanceMoved >= footstepInterval){
		GetClip();
		/*source.clip = soundClip;
		source.volume = footstepVolume;
		source.Play();*/
		if(soundClip != null){
			var audioObj : GameObject = new GameObject("Footstep");
			audioObj.transform.position = transform.position;
			audioObj.transform.parent = transform;
			audioObj.AddComponent(TimedObjectDestructorDB).timeOut = soundClip.length + .1;
			var aO : AudioSource = audioObj.AddComponent(AudioSource);
			aO.clip = soundClip;
			aO.volume = footstepVolume;
			aO.Play();
			aO.loop = false;
			aO.rolloffMode = AudioRolloffMode.Linear;
		}
		distanceMoved = 0;
	}
}

function Crawl(){
	if(distanceMoved >= footstepInterval){
		GetCrawlClip();
		/*source.clip = soundClip;
		source.volume = footstepVolume;
		source.Play();*/
		if(soundClip != null){
			var audioObj : GameObject = new GameObject("Footstep");
			audioObj.transform.position = transform.position;
			audioObj.transform.parent = transform;
			audioObj.AddComponent(TimedObjectDestructorDB).timeOut = soundClip.length + .1;
			var aO : AudioSource = audioObj.AddComponent(AudioSource);
			aO.clip = soundClip;
			aO.volume = footstepVolume;
			aO.Play();
			aO.loop = false;
			aO.rolloffMode = AudioRolloffMode.Linear;
		}
		distanceMoved = 0;
	}
}

//This function, called by Crawl, gets a random clip and sets soundClip to equal that clip
function GetCrawlClip(){
	var hit : RaycastHit;
	if(Physics.Raycast(transform.position, -Vector3.up, hit, 100, ~(1<<PlayerWeapons.playerLayer))){
		if(hit.transform.GetComponent(UseEffects)){
			var effectScript : UseEffects = hit.transform.GetComponent(UseEffects);
			var dex : int = effectScript.setIndex;
			if(effectsManager.setArray[0] != null){
				if(effectsManager.setArray[dex].crawlSounds != null){
					soundClip = effectsManager.setArray[dex].crawlSounds[playDex];
					if(playDex >= effectsManager.setArray[dex].lastCrawlSound - 1){
						playDex = 0;
					}else{
						playDex ++;
					}
				}
			}
		}
	}
}

//This function, called by Footstep, gets a random clip and sets soundClip to equal that clip
function GetClip(){
	var hit : RaycastHit;
	if(Physics.Raycast(transform.position, -Vector3.up, hit, 100, ~(1<<PlayerWeapons.playerLayer))){
		if(hit.transform.GetComponent(UseEffects)){
			var effectScript : UseEffects = hit.transform.GetComponent(UseEffects);
			var dex : int = effectScript.setIndex;
			if(effectsManager.setArray[0] != null){
				if(effectsManager.setArray[dex].footstepSounds != null){
					soundClip = effectsManager.setArray[dex].footstepSounds[playDex];
					if(playDex >= effectsManager.setArray[dex].lastFootstepSound - 1){
						playDex = 0;
					}else{
						playDex ++;
					}
				}
			}
		}
	}
}