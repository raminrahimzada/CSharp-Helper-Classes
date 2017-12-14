/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

var deathAnim : AnimationClip;
private var dead : boolean = false;
var deadTexture : Texture;
var menuDelay : float;
private var menuTime : float;
var menuSpeed : float;

function Death(){
	Destroy(this.GetComponent(GunChildAnimation));
	animation.clip = deathAnim;
	animation.Play();
	dead = true;
	menuTime = Time.time + menuDelay;
}

function OnGUI(){
	if(!dead){
		return;
	}
	var temp = Mathf.Clamp(Time.time-menuTime, 0, 1/menuSpeed);
	GUI.color = Color(1,1,1, temp*menuSpeed);
	if(deadTexture != null)
		GUI.DrawTexture(Rect (0, 0, Screen.width, Screen.height), deadTexture);
	if(GUI.Button(Rect(Screen.width/2 -100, Screen.height/2-20, 200, 40), "Try Again?") && temp >= .8/menuSpeed){
		PlayerWeapons.player.BroadcastMessage("UnFreeze");
		Application.LoadLevel(0);
	}
}
