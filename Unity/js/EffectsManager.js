/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

/*
This script is the manager script for the FPS package's effects system. It has two major r. The first is creating,
storing, and managing all the effects sets which will be used in the scene. The user interacts with the script through a custom
editor script called "EffectsManagerEditor", which can be found in the Editor folder.

The script also plays a role in the actual execution of effects at runtime. When a player shoots a wall which uses a hit effect, this
script instantiates the appropriate decal effect; it is also responsible for playing hit sounds and hit effects. The only effect which
is not applied by this script are footstep sounds. Footsteps are handled in a script called "Footstep", which is attached to the player.
*/

var maxSets : int = 75; //The maximum number of decal sets. This is an arbitrary number, and can be increased if required for some reason.
var maxDecals : int = 75; //The maximum number of decals that can exist in the world. This is editable in the inspector

var setArray : EffectSet[] = new EffectSet[maxSets]; //This is an array which stores all the effects sets we have created.
var setNameArray : String[] = new String[1]; //The names of each set are stored in a separate array from the actual effects sets
var selectedSet : int = 0; //The set we are currently viewing in the inspector
var highestSet : int = 0; //Because we use a builtin array for our setArray, the size of the array can never change. This variable allows 
						  //us to know how many items in our setArray are actually in use, so we don't display any null entries
var audioEvent : GameObject; //When we instantiate an audio event in the script we will store it here.

var decalArray : GameObject[] = new GameObject[maxDecals]; //Every time we instantiate a decal effect in the world we will track it here
var currentDecal : int = 0; //Just like highestSet, this tracks the highest non-null index in the decalArray

var thePlayer : GameObject; //We find the player object in update and store its info here

//These variables are used to make sure that multiple bullet sounds are not all played at once.
var lastSoundTime : float = 0;
var soundCooldown : float = .005;
var canPlay : boolean = true; //If this is false, we can't play a sound yet

static var DELETED : String = "deleted"; //When the user deletes an effects set, we have to move every effects set after it to a new location
										//in the array. A deleted set is renamed "deleted" so the system knows to compact the array.
static var manager : EffectsManager; //static access variable so other scripts can access this one.

function Awake(){
	thePlayer = GameObject.FindWithTag("Player");
	manager = this;
}

function Update(){
	//We want to limit the number of sounds that can be played in a short timeframe, so weapons such as shotguns do not
	//play a hit sound for every single bullet
	if(canPlay == false){
		if(Time.time > lastSoundTime + soundCooldown){
			canPlay = true;
		}
	}
}

//This function is called whenever the user adds a new effects set. The new name has to be added to the name array
function RebuildNameArray(str : String){
	var tempArr = new Array();
	var abort : boolean = false;
	
	if(setNameArray.length == 0){
		setNameArray = new String[1];
	}
		
	if(setNameArray[0] == null){
		setNameArray[0] = str;
	} else {
		for(var i : int = 0; i<setNameArray.length; i++){
			tempArr.Add(setNameArray[i]);
		}
		
		if(abort == false){
			tempArr.Add(str);
			setNameArray = tempArr.ToBuiltin(String);
		}
		
	}
}

//Called when the user creates a set
function CreateSet(){
	setArray[highestSet] = new EffectSet();
	RebuildNameArray("Set " + highestSet);	
	selectedSet = highestSet;
	highestSet ++;
	TrimSetArray();
}

//Whenever the user deletes a set, this is called
function DeleteSet(index : int){
	setArray[index] = null;
	setNameArray[index] = "deleted";
	CompactNameArray();
	CompactSetArray();
	highestSet = setNameArray.length;
}

//This is called inside of DeleteSet. It rebuilds the setNameArray to not include the deleted set.
function CompactNameArray(){
	var tempArr = new Array();
	for(var i : int = 0; i<setNameArray.length; i++){
		if(setNameArray[i] != DELETED){
			tempArr.Add(setNameArray[i]);
		}
	}
	setNameArray = tempArr.ToBuiltin(String);
}

//We shouldn't have any sets at indices greater than 'highestSet.' This function just makes sure of that
function TrimSetArray(){
	for (var i = highestSet ; i < maxSets; i++){
		setArray[i] = null;
	}
}

//This function is called inside of DeleteSet. It reuilds our array of decal sets to not include the deleted set
function CompactSetArray(){
	var tmpSetArray : EffectSet[] = new EffectSet[maxSets];
	var n : int = 0;
	for (var i : int =0; i< maxSets ; i ++){
		if(setArray[i] != null) {
			tmpSetArray[n] = setArray[i];
			n++;
		}
	}
	setArray = tmpSetArray;
}

//Renames a Decal Set
function Rename(str : String){
	var illegal : boolean = false;
	for(var i : int =0; i<setNameArray.length; i++){
		if(setNameArray[i] == str){
			illegal = true;
			Debug.LogWarning("There is already an effects set named " + str + "! Please choose a different name");
		}
	}
	
	if(!illegal){
		setArray[selectedSet].setName = str;
		setNameArray[selectedSet] = str;
	}
	
}

//Applies hit effects like sparks. Called from either ApplyDecal or ApplyDent
function ApplyHitEffect(info : Array){
	//The info array has hit.point, hitRotation, hit.transform, hit.normal, hitSet
	if(setArray[info[4]].hitParticles[0] != null){
		var tempVector : Vector3 = info[0];		
		var tempQuat1 : Quaternion = info[1];		
		var tempTrans2 : Transform = info[2];
		var tempVector3 : Vector3 = info[3];		
		var tempInt4 : int = info[4];

		var toApply : int = Random.Range(0,setArray[tempInt4].lastHitParticle);
		var clone : GameObject = Instantiate(setArray[tempInt4].hitParticles[toApply], tempVector, tempQuat1);
		clone.transform.localPosition += .01*tempVector3;
		clone.transform.LookAt(thePlayer.transform.position);
	}
}

//This function is called whenever a weapon is fired (so in 'GunScript' for players and 'Fire' for non-players).
//It is responsible for applying any required decals, hit effects, and hit sounds.
function ApplyDecal(info : Array){
	//The info array has hit.point, hitRotation, hit.transform, hit.normal, hitSet
	if(setArray[0] != null){
		if(setArray[info[4]].bulletDecals[0] != null){
			var tempVector : Vector3 = info[0];		
			var tempQuat1 : Quaternion = info[1];		
			var tempTrans2 : Transform = info[2];
			var tempVector3 : Vector3 = info[3];		
			var tempInt4 : int = info[4];

			var toApply : int = Random.Range(0, setArray[tempInt4].lastBulletDecal);
			var clone : GameObject = Instantiate(setArray[tempInt4].bulletDecals[toApply], tempVector, tempQuat1);
			clone.transform.localPosition += .05*tempVector3;
			clone.transform.parent = tempTrans2;
			if(currentDecal >= maxDecals){
				currentDecal = 0;
			}
			if(decalArray[currentDecal] != null){
				Destroy(decalArray[currentDecal]);
			}
			decalArray[currentDecal] = clone;
			currentDecal ++;
		}
		ApplyHitEffect(info);
		BulletSound(info);
	}else{
		Debug.LogWarning("EffectsManager: You have at least one object with the UseDecals script attached, but no decal sets. Please create a decal set");
	}
}

//Some weapons may apply 'dents' instead of 'decals' - for example, hitting a sheet of metal with a pipe would not make a round bullet hole appear.
//This functions identically to ApplyDecal, only it is used in cases where dents are applied instead of the default decal.
function ApplyDent(info : Array){
	//The info array has hit.point, hitRotation, hit.transform, hit.normal, hitSet

	if(setArray[0] != null){
		if(setArray[info[4]].dentDecals[0] != null){
			var tempVector : Vector3 = info[0];		
			var tempQuat1 : Quaternion = info[1];		
			var tempTrans2 : Transform = info[2];
			var tempVector3 : Vector3 = info[3];		
			var tempInt4 : int = info[4];

			var toApply : int = Random.Range(0, setArray[tempInt4].lastBulletDecal);
			var clone : GameObject = Instantiate(setArray[tempInt4].bulletDecals[toApply], tempVector, tempQuat1);
			clone.transform.localPosition += .05*tempVector3;
			clone.transform.parent = tempTrans2;
			if(currentDecal >= maxDecals){
				currentDecal = 0;
			}			if(decalArray[currentDecal] != null){
				Destroy(decalArray[currentDecal]);
			}
			decalArray[currentDecal] = clone;
			currentDecal ++;
		}
		ApplyHitEffect(info);
		CollisionSound(info);
	}else{
		Debug.LogWarning("EffectsManager: You have at least one object with the UseDecals script attached, but no decal sets. Please create a decal set");
	}
}

//Called by BulletSound and CollisionSound; actually does the legwork of creating an audio event and playing the correct sound
function CreateAudioEvent(clipToPlay : AudioClip, info : Array){
	audioEvent = new GameObject("Audio Event");
	var t : Transform = info[2] as Transform;
	audioEvent.transform.position = t.position;
	audioEvent.AddComponent(AudioSource);
	
	var source : AudioSource = audioEvent.GetComponent(AudioSource);
	//source.rolloffMode = AudioRolloffMode.Linear;
	source.clip = clipToPlay;
	source.Play();
	
	audioEvent.AddComponent(TimedObjectDestructorDB);
	audioEvent.GetComponent(TimedObjectDestructorDB).timeOut = clipToPlay.length + .5;
	canPlay = false;
	lastSoundTime = Time.time;
}

//Called from either ApplyDecal or ApplyDent.
function BulletSound(info : Array){
	//The info array has hit.point, hitRotation, hit.transform, hit.normal, hitSet
	if(setArray[info[4]].bulletSounds[0] != null && canPlay){
		var toPlay : int = Random.Range(0, setArray[info[4]].lastBulletSound);
		CreateAudioEvent(setArray[info[4]].bulletSounds[toPlay], info);
	}
}

//Called from either ApplyDecal or ApplyDent; same as BulletSound except it plays the set's collision sound instead of its bullet sound
function CollisionSound(info : Array){
	//The info array has hit.point, hitRotation, hit.transform, hit.normal, hitSet
	if(setArray[info[4]].collisionSounds[0] != null && canPlay){
		var toPlay : int = Random.Range(0, setArray[info[4]].lastCollisionSound);
		CreateAudioEvent(setArray[info[4]].collisionSounds[toPlay], info);
	}
}