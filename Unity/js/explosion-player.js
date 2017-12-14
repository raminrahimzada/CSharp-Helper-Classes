/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

var explosionRadius = 5.0;
var explosionPower = 10.0;
var explosionDamage = 100.0;
var explosionTimeout = 2.0;
var vFactor : float = 3;
var shakeFactor : float = 1.5;
var minHitShake : float = .07;
var minShake : float = .01;

var highestParent : int = 0;
var parentArray : GameObject[];

function AlreadyHit(GO : GameObject) : boolean{ //if this function returns true, we have already hit another child of this object's highest parent
	var toCompare : GameObject = FindTopParent(GO);
	var toReturn : boolean = false;
	for(var i : int = 0; i<highestParent; i++){
		if(parentArray[i] == toCompare){
			toReturn = true;
			break;
		}
	}
	if(toReturn == false){
		parentArray[highestParent] = toCompare;
		highestParent ++;
	}
	return toReturn;
}
//Finds the top parent, *OR* the first parent with EnemyDamageReceiver
//If the top parent has no EnemyDamageReceiver, it returns the object passed in instead, as if there was no parent
function FindTopParent(GO : GameObject) : GameObject{
	var tempTransform : Transform;
	var returnObj : GameObject;
	var keepLooping : boolean = true;
	if(GO.transform.parent != null){
		tempTransform = GO.transform;
		while(keepLooping){
			if(tempTransform.parent != null){
				tempTransform = tempTransform.parent;
				if(tempTransform.GetComponent(EnemyDamageReceiver)){
					keepLooping = false;
				}
			}else{
				keepLooping = false;
			}
		}
		if(tempTransform.GetComponent(EnemyDamageReceiver)){
			returnObj = tempTransform.gameObject;
		}else{
			returnObj = GO;
		}
	}else{
		returnObj = GO;
	}
	return returnObj;
}

function Start(){
	parentArray = new GameObject[128]; //Arbitrary array size; can be increased
	highestParent = 0;
	var sendArray : Object[] = new Object[2];
	var shook : boolean = false;
	
	var explosionPosition = transform.position;

	// Apply damage to close by objects first
	var colliders : Collider[] = Physics.OverlapSphere (explosionPosition, explosionRadius);
	for (var hit in colliders){
		if(AlreadyHit(hit.gameObject) == false){
			// Calculate distance from the explosion position to the closest point on the collider
			var closestPoint = hit.ClosestPointOnBounds(explosionPosition);
			var distance = Vector3.Distance(closestPoint, explosionPosition);

			// The hit points we apply fall decrease with distance from the explosion point
			var hitPoints = 1.0 - Mathf.Clamp01(distance / explosionRadius);
			if(hit.tag == "Player" && !shook){	
				shook = true;
				CameraShake.ShakeCam(Mathf.Max(hitPoints*shakeFactor, minShake), 10, Mathf.Max(hitPoints*shakeFactor, .3));
			}
			
			hitPoints *= explosionDamage;

			// Tell the rigidbody or any other script attached to the hit object how much damage is to be applied!
			if(hit.gameObject.layer != 2){
				sendArray[0] = hitPoints;
				sendArray[1] = true;

				hit.SendMessageUpwards("ApplyDamage", sendArray, SendMessageOptions.DontRequireReceiver);
				hit.SendMessageUpwards("Direction", transform, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	if(!shook){	
		shook = true;
		CameraShake.ShakeCam(minShake, 10, minShake);
	}
	// Apply explosion forces to all rigidbodies
	// This needs to be in two steps for ragdolls to work correctly.
	// (Enemies are first turned into ragdolls with ApplyDamage then we apply forces to all the spawned body parts)
	colliders = Physics.OverlapSphere (explosionPosition, explosionRadius);
	for (var hit in colliders) {
		if (hit.rigidbody && hit.gameObject.layer != PlayerWeapons.playerLayer)
			hit.rigidbody.AddExplosionForce(explosionPower, explosionPosition, explosionRadius, vFactor);
	}	
	// stop emitting particles
	if (particleEmitter) {
        particleEmitter.emit = true;
		yield WaitForSeconds(0.5);
		particleEmitter.emit = false;
    }
    // destroy the explosion after a while
	Destroy (gameObject, explosionTimeout);
}