/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

public var delay : float;
public var fadeTime : float;
	
private var fadeSpeed : float;
private var intensity : float;
var startIntensity : float = 6;
private var color : Color;
private var active1 : boolean = false;
	
function Start(){
	if(light == null){
		Destroy(this);
		return;
	}
}
	
function Update(){
	if(!active1)
		return;
	if(delay > 0.0){
		delay -= Time.deltaTime;
	}
	else if(intensity > 0.0){
		intensity -= fadeSpeed * Time.deltaTime;
		light.intensity = intensity;
	}
}

function Activate(){
	light.intensity = startIntensity;
	intensity = light.intensity;
	active1 = true;
	if(fadeTime > 0.0)
	{
		fadeSpeed = intensity / fadeTime;
	}
	else
	{
		fadeSpeed = intensity;
	}
}