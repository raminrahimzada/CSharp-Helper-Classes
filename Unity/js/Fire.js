/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

private var effectsManager : EffectsManager;
private var shotCountTracer : int;
var traceEvery : int = 1;

function Start () {
	effectsManager = GameObject.FindWithTag("Manager").GetComponent(EffectsManager);
}

function Fire (penetration : int, damage : float, force : float, tracer : GameObject, direction : Vector3, firePosition : Vector3) {
//must pass in penetation level, damage, force, tracer object (optional), direction to fire in, and position top fire from.

	var penetrate : boolean = true;
	var pVal : int = penetration;
	var layer2 = 1 << 2; 
	var layerMask = layer2;
  	layerMask = ~layerMask;
  	var hits : RaycastHit[];
  	hits = Physics.RaycastAll (firePosition, direction, 100, layerMask);
	shotCountTracer += 1;
  	if(tracer != null && traceEvery <= shotCountTracer){
  		shotCountTracer = 0;
 		if(hits.length > 0){
  			tracer.transform.LookAt(hits[0].point);
  		}else{
  			tracer.transform.LookAt((transform.position + 90 * direction));
  		}
  		tracer.GetComponent(ParticleEmitter).Emit();
  		tracer.GetComponent(ParticleEmitter).Simulate(.02);
  	}
	System.Array.Sort(hits, Comparison);  	
//	 Did we hit anything?
	for (var i=0;i<hits.length;i++){
        var hit : RaycastHit = hits[i];
        var BP : BulletPenetration = hit.transform.GetComponent(BulletPenetration);
        if(penetrate){
       		if(BP == null){
        		penetrate = false;
       		} else {
       			if(pVal < BP.penetrateValue){
       				penetrate = false;
       			} else {
       				pVal -= BP.penetrateValue;
       			}	
       		}
       		//DAmage Array
       		var sendArray : Object[] = new Object[2];
			sendArray[0] = damage;
			sendArray[1] = false;
			// Send a damage message to the hit object			
			hit.collider.SendMessageUpwards("ApplyDamage", sendArray, SendMessageOptions.DontRequireReceiver);
			hit.collider.SendMessageUpwards("Direction", transform, SendMessageOptions.DontRequireReceiver);
			//And send a message to the decal manager, if the target uses decals
			if(hit.transform.gameObject.GetComponent(UseEffects)){
				//The effectsManager needs five bits of information
				var hitRotation : Quaternion = Quaternion.FromToRotation(Vector3.up, hit.normal);
				var hitSet : int = hit.transform.gameObject.GetComponent(UseEffects).setIndex;
				var hitInfo = new Array(hit.point, hitRotation, hit.transform, hit.normal, hitSet);		
				effectsManager.SendMessage("ApplyDecal", hitInfo, SendMessageOptions.DontRequireReceiver);
			}
			// Apply a force to the rigidbody we hit
			if (hit.rigidbody)
				hit.rigidbody.AddForceAtPosition(force * direction, hit.point);
		}
	}
	BroadcastMessage("MuzzleFlash", true, SendMessageOptions.DontRequireReceiver);
		var audioObj : GameObject = new GameObject("GunShot");
	audioObj.transform.position = transform.position;
	audioObj.transform.parent = transform;
	audioObj.AddComponent(TimedObjectDestructorDB).timeOut = audio.clip.length + .1;
	var aO : AudioSource = audioObj.AddComponent(AudioSource);
	aO.clip = audio.clip;
	aO.volume = audio.volume;
	aO.pitch = audio.pitch;
	aO.Play();
	aO.loop = false;
	aO.rolloffMode = AudioRolloffMode.Linear;
}

function Comparison(x : RaycastHit, y : RaycastHit) : int { 
   var xDistance = x.distance; 
   var yDistance = y.distance; 
   return xDistance - yDistance; 
}