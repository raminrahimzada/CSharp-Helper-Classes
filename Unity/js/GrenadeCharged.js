/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

var delay = 1.0;
var timeOut = 1.0;
var detachChildren = false;
var explosion : Transform;
var explosionCharged : Transform;
var explodeAfterBounce : boolean = false;
private var hasCollided : boolean = false;
private var explodeTime : float;
var playerThings : GameObject[];
var chargeVal : float;

function Start (){
	explodeTime = Time.time+timeOut;
	playerThings = GameObject.FindGameObjectsWithTag("Player");
	for(var i : int = 0; i < playerThings.length; i++){
		if(playerThings[i].collider != null){
			Physics.IgnoreCollision(this.collider, playerThings[i].collider);
		}
	}
}

function OnCollisionEnter (collision : Collision){
	if(hasCollided || !explodeAfterBounce)
		DestroyNow();
	yield new WaitForSeconds(delay);
	hasCollided = true;
}

function ChargeLevel(charge : float){
	if(charge >= chargeVal){
		this.GetComponent(Rigidbody).useGravity = false;
		explosion = explosionCharged;
	}
}

function DestroyNow(){
	if (detachChildren) {
		transform.DetachChildren ();
	}
	DestroyObject (gameObject);
	if (explosion){
		Instantiate (explosion, transform.position, transform.rotation);
	}
}

function Update(){
   var direction = transform.TransformDirection(Vector3.forward);
   var hit : RaycastHit;
   if(Time.time > explodeTime){
   	DestroyNow();
   }
}
