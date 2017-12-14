/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
 For additional information contact us info@dastardlybanana.com.
*/
@HideInInspector
var bulletsLeft : int;
@HideInInspector
var clips : int;
@HideInInspector
var display : boolean = true; //used by system
var show : boolean = true; //used by user
@HideInInspector
var clipDisplay : String;
private var gunScripts = new Array();
private var gunScriptSecondary : GunScript;
private var gunScript : GunScript;

function Start(){
	//This is AmmoDisplay getting all of the GunScripts from this weapon, then saving the primary and secondary.
	gunScripts = this.GetComponents(GunScript);
	var g : GunScript;
	for(var i = 0; i < gunScripts.length; i++){
		g = gunScripts[i] as GunScript;
		if(g.isPrimaryWeapon){
			gunScript = g;
		}
	}
	for(var q = 0; q < gunScripts.length; q++){
		g = gunScripts[q] as GunScript;
		if(!g.isPrimaryWeapon){
			if(gunScript.secondaryWeapon == g)
				gunScriptSecondary = g;
		}
	}
}

function reapply(){
	//This is AmmoDisplay getting all of the GunScripts from this weapon, then saving the primary and secondary.
	gunScripts = this.GetComponents(GunScript);
	var g : GunScript;
	for(var i = 0; i < gunScripts.length; i++){
		g = gunScripts[i] as GunScript;
		if(g.isPrimaryWeapon){
			gunScript = g;
		}
	}
	for(var q = 0; q < gunScripts.length; q++){
		g = gunScripts[q] as GunScript;
		if(!g.isPrimaryWeapon){
			if(gunScript.secondaryWeapon == g)
				gunScriptSecondary = g;
		}
	}
}

function OnGUI(){
	if(!(display && show))	
		return;
	//Decide whether or not to show clips depending on if the guns have infinite ammo
	//This will have to be modified if you change the display
	var clipDisplay : String;
	var clipDisplay2 : String;
	
	if(!gunScript.infiniteAmmo){
		clipDisplay ="/"+gunScript.clips;
	} else {
		clipDisplay = "";
	}
	
	if(gunScriptSecondary != null && !gunScriptSecondary.infiniteAmmo){
		clipDisplay2 ="/"+gunScriptSecondary.clips;
	} else {
		clipDisplay2 = "";
	}

	//This is where you'll want to edit to make your own ammo display
		if(gunScriptSecondary != null){
		//If there is a secondary weapon, display it's ammo along with the main weapon's
			GUI.Box(Rect(Screen.width - 110,Screen.height-55,100,20),"Ammo: "+Mathf.Round(gunScript.ammoLeft) + clipDisplay);
			GUI.Box(Rect(Screen.width - 80,Screen.height-30,70,20),"Alt: "+Mathf.Round(gunScriptSecondary.ammoLeft) + clipDisplay2);
		} else {
		//Otherwise just display the main weapon's ammo
			GUI.Box(Rect(Screen.width - 110,Screen.height-30,100,20),"Ammo: "+Mathf.Round(gunScript.ammoLeft) + clipDisplay);
		}
}

function SelectWeapon(){
	display = true;
}

function DeselectWeapon(){
	display = false;
}
