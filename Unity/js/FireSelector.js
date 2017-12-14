#pragma strict
var gscript : GunScript;
var state : int = 0;
var sound : AudioClip;
var soundVolume : float = 1;
var anim : String;

function Start () {
	gscript.autoFire = (state == 0);
	gscript.burstFire = (state == 1);
}

function Cycle () {
	if (!gscript.gunActive || AimMode.sprintingPublic || LockCursor.unPaused || gscript.reloading)
		return;
	audio.PlayOneShot(sound, soundVolume);
	if(anim != ""){
		BroadcastMessage("PlayAnim", anim);
	}
	state++;
	if(state == 3)
		state = 0;
	
	gscript.autoFire = (state == 0);
	gscript.burstFire = (state == 1);
}

function Update () {
	if(InputDB.GetButtonDown("Fire2")){
		Cycle();
	}
}