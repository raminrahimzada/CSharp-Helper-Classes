/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

var fireAnim : String = "Fire";
var emptyFireAnim : String = "";
var reloadAnim : String = "Reload";
var emptyReloadAnim : String = "Reload";
var takeOutAnim : String = "TakeOut";
var putAwayAnim : String = "PutAway";
var enterSecondaryAnim : String = "EnterSecondary";
var exitSecondaryAnim : String = "ExitSecondary";
var reloadIn : String = "ReloadIn";
var reloadOut : String = "ReloadOut";

var walkAnimation : String = "Walk";
var secondaryWalkAnim : String = "";
var secondarySprintAnim : String = "";
var walkSpeedModifier : float = 20;
var walkWhenAiming  : boolean = false;
var sprintAnimation : String = "Sprint";
var nullAnim : String = "Null";
var secondaryNullAnim : String = "";
var idleAnim : String = "Idle";
var secondaryIdleAnim : String = "";
var chargeAnim : String = "Charge";
private var stopAnimTime : float = 0;
var aim : boolean = false;
private var CM : CharacterMotorDB;
private var idle : boolean = false;
private var secondary : boolean = false;
private var walkAnim : String = "";
private var sprintAnim : String = "";
private var nullAnimation : String = "";
var hasSecondary : boolean = false;

var secondaryReloadAnim : String = "";
var secondaryReloadEmpty : String = "";
var secondaryFireAnim : String = "";
var secondaryEmptyFireAnim : String = "";

//melee
var animCount : int = 2;
var fireAnims = new String[15];
var reloadAnims = new String[15];
var index : int = -1;
var lastIndex : int = -1;
var melee : boolean = false;
var random : boolean = false;
var lastSwingTime : float;
var resetTime : float;
var gs : GunScript;

private var dir : Vector3;
private var moveWeight : float = 1;
private var nullWeight : float = 1;
private var useStrafe : boolean = true;

function PlayAnim (name : String){
	idle = false;
	if(animation[name] == null || !gs.gunActive){
		return;
	}
	animation.Stop(name);
	animation.Rewind(name);
	animation.CrossFade(name, .2);
	stopAnimTime = Time.time + animation[name].length;
}

function PlayAnim (name : String, time : float){
	idle = false;
	if(animation[name] == null || !gs.gunActive){
		return;
	}
	animation.Stop(name);
	animation.Rewind(name);
	animation[name].speed = (animation[name].clip.length/time);
	animation.CrossFade(name, .2);
	stopAnimTime = Time.time + animation[name].length;
}

function Update () {
	if(gs != null){
		if(!gs.gunActive){
			return;
		}
	}
	if(animation[nullAnim] == null)
		return;
	if(animation[walkAnimation] == null)
		return;
	
	var CM : CharacterMotorDB = PlayerWeapons.CM;
	
	if (!CM.grounded){
		nullWeight = Mathf.Lerp(nullWeight, 1, Time.deltaTime * 5);
		moveWeight = 0;
	} 
	if(Time.time > stopAnimTime+.1){
		moveWeight = Mathf.Lerp(moveWeight, 1, Time.deltaTime * 5);
		nullWeight = Mathf.Lerp(nullWeight, 1, Time.deltaTime * 5);
	} else {
		moveWeight = 0;
		nullWeight = 0;
	}
	
	animation[nullAnim].weight = nullWeight;
		
	var veloc : Vector3 = PlayerWeapons.CM.movement.velocity;
	var trans : Transform = PlayerWeapons.player.transform;		
	dir = Vector3.Lerp(dir, trans.InverseTransformDirection(veloc), Time.deltaTime*6);
	var dirN = dir.normalized;
    
    var forwardWeight : float = dirN.z;
    var rightWeight = dirN.x;
    
    //Weight and speed from direction
    animation[walkAnimation].weight = Mathf.Abs(forwardWeight)*moveWeight;    
    animation[walkAnimation].speed = dir.z/CM.movement.maxForwardSpeed;
    
    var strafeWeight : float = Mathf.Abs(rightWeight)*moveWeight;
    var strafeSpeed : float = dir.x/CM.movement.maxSidewaysSpeed*moveWeight;
    
    //Apply to strafe animation
   /* if(useStrafe){
    	animation[strafeRightAnimation].weight = strafeWeight;
   		animation[strafeRightAnimation].speed = strafeSpeed;
   	} else {*/
   		//Handle if we don't have a strafe animation by applying to walk animation
   		animation[walkAnimation].weight = Mathf.Max(animation[walkAnimation].weight, strafeWeight);
   		if(Mathf.Abs(strafeSpeed) > Mathf.Abs(animation[walkAnimation].speed)){
   			animation[walkAnimation].speed = strafeSpeed;
   		}
  // 	}
}

/*function LateUpdate(){
	if(gs)
		if(!gs.gunActive)
			return;
	if(animation[walkAnim] != null){
		var temp : boolean = animation[walkAnim].enabled;
	} else {
		temp = false;
	}
	
	if(animation[sprintAnim] != null){
		var temp2 : boolean = animation[sprintAnim].enabled;
	} else {
		temp2 = false;
	}

	/*if(!animation.IsPlaying(nullAnim))
		animation.CrossFade(nullAnim, .4);
}*/

function ReloadAnim (reloadTime : float){
	idle = false;
	if(animation[reloadAnim] == null){
		return;
	}
	//animation.Stop(reloadAnim);
	animation.Rewind(reloadAnim);
	animation[reloadAnim].speed = (animation[reloadAnim].clip.length/reloadTime);
	animation.Play(reloadAnim);
	stopAnimTime = Time.time + reloadTime;
}

function ReloadEmpty(reloadTime : float){
	idle = false;
	if(animation[emptyReloadAnim] == null){
		return;
	}
	animation.Rewind(emptyReloadAnim);
	animation[emptyReloadAnim].speed = (animation[emptyReloadAnim].clip.length/reloadTime);
	animation.Play(emptyReloadAnim);
	stopAnimTime = Time.time + reloadTime;
}

function FireAnim(){
	idle = false;
	if(animation[fireAnim] == null){
		return;
	}
	animation.Rewind(fireAnim);
	animation.CrossFade(fireAnim, .05);
	stopAnimTime = Time.time + animation[fireAnim].clip.length;
}

function SecondaryReloadEmpty(reloadTime : float){
	idle = false;
	if(animation[secondaryReloadEmpty] == null){
		return;
	}
	animation[secondaryReloadEmpty].speed = (animation[secondaryReloadEmpty].clip.length/reloadTime);
	animation.Rewind(secondaryReloadEmpty);
	animation.CrossFade(secondaryReloadEmpty, .2);
	stopAnimTime = Time.time + reloadTime;
}

function SecondaryReloadAnim(reloadTime : float){
	idle = false;
	if(animation[secondaryReloadAnim] == null){
		return;
	}
	animation[secondaryReloadAnim].speed = (animation[secondaryReloadAnim].clip.length/reloadTime);
	animation.Rewind(secondaryReloadAnim);
	animation.CrossFade(secondaryReloadAnim, .2);
	stopAnimTime = Time.time + reloadTime;
}

function SecondaryFireAnim(){
	idle = false;
	if(animation[secondaryFireAnim] == null){
		return;
	}
	animation.Rewind(secondaryFireAnim);
	animation.CrossFade(secondaryFireAnim, .2);
	stopAnimTime = Time.time + animation[secondaryFireAnim].clip.length;
}

function TakeOutAnim(takeOutTime : float){
	idle = false;
	if(takeOutTime <= 0)
		return;
	if(animation[takeOutAnim] == null){
		return;
	}
	animation.Stop(putAwayAnim);
	animation.Stop(takeOutAnim);
	animation.Rewind(takeOutAnim);
	animation[takeOutAnim].speed = (animation[takeOutAnim].clip.length/takeOutTime);
	animation.Play(takeOutAnim);
	stopAnimTime = Time.time + takeOutTime;
}

function PutAwayAnim(putAwayTime : float){
	idle = false;
	secondary = false;
	nullAnimation = nullAnim;
	if(putAwayTime <= 0)
		return;
	if(animation[putAwayAnim] == null){
		return;
	}
	animation.Stop(putAwayAnim);
	animation.Rewind(putAwayAnim);
	animation[putAwayAnim].speed = (animation[putAwayAnim].clip.length/putAwayTime);
	animation.CrossFade(putAwayAnim, .1
	);
	stopAnimTime = Time.time + putAwayTime;
}

function SingleFireAnim(fireRate : float){
	idle = false;
	if(animation[fireAnim] == null){
		return;
	}
	animation.Stop(fireAnim);
	animation[fireAnim].speed = (animation[fireAnim].clip.length/fireRate);
	animation.Rewind(fireAnim);
	animation.CrossFade(fireAnim, .05);
	stopAnimTime = Time.time + fireRate;
}

function EmptyFireAnim () {
	idle = false;
	if(animation[emptyFireAnim] == null){
		return;
	}
	animation.Stop(emptyFireAnim);
	animation.Rewind(emptyFireAnim);
	animation.CrossFade(emptyFireAnim, .05);
	stopAnimTime = Time.time + animation[emptyFireAnim].length;
}

function SecondaryEmptyFireAnim () {
	idle = false;
	if(animation[secondaryEmptyFireAnim] == null){
		return;
	}
	animation.Stop(secondaryEmptyFireAnim);
	animation.Rewind(secondaryEmptyFireAnim);
	animation.CrossFade(secondaryEmptyFireAnim, .05);
	stopAnimTime = Time.time + animation[secondaryEmptyFireAnim].length;
}

function EnterSecondary(t : float){
	if(animation[secondaryNullAnim] != null){
		nullAnimation = secondaryNullAnim;
	}
	idle = false;
	secondary = true;
	if(animation[enterSecondaryAnim] == null){
		return;
	}
	animation.Stop(enterSecondaryAnim);
	animation[enterSecondaryAnim].speed = (animation[enterSecondaryAnim].clip.length/t);
	animation.Rewind(enterSecondaryAnim);
	animation.CrossFade(enterSecondaryAnim, .2);
	stopAnimTime = Time.time + t;
}

function ExitSecondary(t : float){
	nullAnimation = nullAnim;
	idle = false;
	secondary = false;
	if(animation[exitSecondaryAnim] == null){
		return;
	}
	animation.Stop(exitSecondaryAnim);
	animation[exitSecondaryAnim].speed = (animation[exitSecondaryAnim].clip.length/t);
	animation.Rewind(exitSecondaryAnim);
	animation.CrossFade(exitSecondaryAnim, .2);
	stopAnimTime = Time.time + t;
}

function SingleSecFireAnim(fireRate : float){
	idle = false;
	if(animation[secondaryFireAnim] == null){
		return;
	}
	animation.Stop(secondaryFireAnim);
	animation[secondaryFireAnim].speed = (animation[secondaryFireAnim].clip.length/fireRate);
	animation.Rewind(secondaryFireAnim);
	animation.CrossFade(secondaryFireAnim, .05);
	stopAnimTime = Time.time + fireRate;
}

function ReloadIn(reloadTime : float){
	idle = false;
	if(animation[reloadIn] == null){
		return;
	}
	animation[reloadIn].speed = (animation[reloadIn].clip.length/reloadTime);
	animation.Rewind(reloadIn);
	animation.Play(reloadIn);
	stopAnimTime = Time.time + reloadTime;
}

function ReloadOut(reloadTime : float){
	idle = false;
	if(animation[reloadOut] == null){
		return;
	}
	animation[reloadOut].speed = (animation[reloadOut].clip.length/reloadTime);
	animation.Rewind(reloadOut);
	animation.Play(reloadOut);
	stopAnimTime = Time.time + reloadTime;
}

function IdleAnim(){
	if(animation[idleAnim] == null || idle || Time.time < stopAnimTime){
		return;
	}
	if(!PlayerWeapons.doesIdle){
		idle = true;
		return;
	}
	idle = true;
	if(secondary){
		animation.Stop(secondaryIdleAnim);
		animation.Rewind(secondaryIdleAnim);
		animation.CrossFade(secondaryIdleAnim, .2);
		stopAnimTime = Time.time + animation[secondaryIdleAnim].clip.length;
		return;
	}
	animation.Stop(idleAnim);
	animation.Rewind(idleAnim);
	animation.CrossFade(idleAnim, .2);
	stopAnimTime = Time.time + animation[idleAnim].clip.length;
	yield new WaitForSeconds(animation[idleAnim].clip.length);
	idle = false;
}

function Start(){
	idle = false;
	CM = PlayerWeapons.CM;
	stopAnimTime = 10;
	aim = false;
	nullAnimation = nullAnim;
	
	/*for (s : AnimationState in animation) {
    	s.layer = 1;
	}*/
	
	if(animation[nullAnim] != null){
		animation[nullAnim].layer = -2;
		animation[nullAnim].enabled = true;
	}
	if(animation[walkAnimation] != null){
		animation[walkAnimation].layer = -1;
		animation[walkAnimation].enabled = true;
	}
		
/*	if(animation[strafeRightAnimation] != null){
		animation[strafeRightAnimation].layer = -1;
		animation[strafeRightAnimation].enabled = true;
	} else {
		useStrafe = false;
	}*/
		
	if(animation[sprintAnim] != null){
		animation[sprintAnim].layer = -1;
	}
		
	animation.SyncLayer(-1);
	
	stopAnimTime = -1;
}

function Aiming(){
	idle = false;
	aim = true;
	var temp : boolean =  false;
	var temp2 : boolean = false;
	if(animation[walkAnim] != null && !walkWhenAiming){
		animation.Stop(walkAnim);
	}
	if(animation[sprintAnim] != null){
		 animation.Stop(sprintAnim);
	}
	if(animation[nullAnim] != null)
		animation.CrossFade(nullAnimation, .2);
}

function StopAiming () {
	aim = false;
}

function FireMelee (fireRate : float) {
	var temp : String;
	if(random){
		lastIndex = index;
		index = Mathf.Round(Random.Range(0, animCount-1));
		if(index == lastIndex){
			if(index == animCount-1){
				index = Mathf.Clamp(index-1,0,animCount-1);
			} else {
				index += 1;
			}
		}
	} else {
		if(Time.time > lastSwingTime+resetTime){
			index = 0;
		} else {
			index += 1;
		}
		if (index == animCount){
			index = 0;
		}
		lastSwingTime = Time.time;
	}
	temp = fireAnims[index];
	
		idle = false;
	if(temp == "" || animation[temp] == null){
		return;
	}
	//animation.Stop(temp);
	animation[temp].speed = (animation[temp].clip.length/fireRate);
	//animation.Rewind(temp);
	animation.CrossFade(temp, .05);
	stopAnimTime = Time.time + fireRate;
}

function ReloadMelee (fireRate : float) {
	var temp : String;
	temp = reloadAnims[index];
	idle = false;
	if(animation[temp] == null){
		return;
	}
	animation.Stop(fireAnims[index]);
	animation[temp].speed = (animation[temp].clip.length/fireRate);
	//animation.Rewind(temp);
	animation.CrossFadeQueued(temp, .05);
	stopAnimTime = Time.time + fireRate;

}
