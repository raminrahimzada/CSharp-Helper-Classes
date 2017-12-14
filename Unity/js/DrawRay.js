/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

var needsSelection : boolean = false;
var negative : boolean = false;

function OnDrawGizmosSelected(){
    if(needsSelection){
  		Gizmos.color = Color.white;
	    var pos : Vector3 = Camera.main.ScreenToWorldPoint(Vector3(Screen.width/2, Screen.height/2,10));
	    Gizmos.DrawWireSphere(pos, .05);
    }
}

function OnDrawGizmos(){
	if(!needsSelection){
		Gizmos.color = Color.white;
		var pos : Vector3 = Camera.main.ScreenToWorldPoint(Vector3(Screen.width/2, Screen.height/2,10));
	    Gizmos.DrawWireSphere(pos, .05);
	}
}