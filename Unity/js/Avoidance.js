#pragma strict
/*var avoidPos : Vector3;
var avoidRot : Vector3;
private var standardPos : Vector3;
private var standardRot : Vector3;*/
var avoidRotation : Vector3;
var avoidPosition : Vector3;
var avoidStartDistance : float = 4;
var avoidMaxDistance : float = 1.3;
private var rot : Vector3;
private var pos : Vector3;
private var dist : float = 2;
private var minDist : float = 1.5;

var layerMask : LayerMask = ~(1<<PlayerWeapons.playerLayer + 1<<PlayerWeapons.ignorePlayerLayer);
private var targetRot : Vector3;
private var targetPos : Vector3;
static var collided : boolean = false;
static var canAim : boolean = true;
static var singleton : Avoidance;
var avoid : boolean = true;
private var startAvoid : boolean;

var stopTime : float = 0;

function Awake(){
	singleton = this;
	rot = avoidRotation;
	pos = avoidPosition;
	dist = avoidStartDistance;
	minDist = avoidMaxDistance;
	startAvoid = avoid;
}

//Sets values to given.
static function SetValues(rotation : Vector3, position : Vector3, startDist : float, maxDist : float, avoids : boolean){
	if(!singleton) return;
	singleton.rot = rotation;
	singleton.pos = position;
	singleton.dist = startDist;
	singleton.minDist = maxDist;
	singleton.avoid = avoids;
}

//Reverts to default values
static function SetValues(){
	if(!singleton) return;
	singleton.rot = singleton.avoidRotation;
	singleton.pos = singleton.avoidPosition;
	singleton.dist = singleton.avoidStartDistance;
	singleton.minDist = singleton.avoidMaxDistance;
	singleton.avoid = singleton.startAvoid;
}

function Update () {
	if(!avoid){
		collided = false;
		return;
	}
	var hit : RaycastHit;
	var ray : Ray = Camera.main.ScreenPointToRay(Vector3(Screen.width/2, Screen.height/2,0));
	var ray2 : Ray = Camera.main.ScreenPointToRay(Vector3(Screen.width/2+Screen.width/65, Screen.height/2,0));
	var ray3 : Ray = Camera.main.ScreenPointToRay(Vector3(Screen.width/2-Screen.width/65, Screen.height/2,0));
	
	if(Physics.Raycast(ray, hit, dist, layerMask) && !GunScript.reloading && !GunScript.takingOut && !GunScript.puttingAway){
		Collide(hit);
	} else if (stopTime < 0) {
		stopTime = Time.time + .06;
	}/* else if(Physics.Raycast(ray2, hit, dist, layerMask) && collided && !GunScript.reloading && !GunScript.takingOut && !GunScript.puttingAway){
		Collide(hit);
	} else if(Physics.Raycast(ray3, hit, dist, layerMask) && collided && !GunScript.reloading && !GunScript.takingOut && !GunScript.puttingAway){
		Collide(hit);
	} else {
	*/
	if(Time.time > stopTime && stopTime > 0){
		targetRot = Vector3(0,0,0);
		targetPos = Vector3(0,0,0);
		canAim = true;
		if(transform.localPosition.magnitude < .3)
			collided = false;
	}
	var rate : float = Time.deltaTime*9;
	
	if(transform.localPosition != targetPos)
		transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, rate);
		
	if(transform.localEulerAngles != targetRot){
		transform.localEulerAngles.x = Mathf.LerpAngle(transform.localEulerAngles.x, targetRot.x, rate);
		transform.localEulerAngles.y = Mathf.LerpAngle(transform.localEulerAngles.y, targetRot.y, rate);
		transform.localEulerAngles.z = Mathf.LerpAngle(transform.localEulerAngles.z, targetRot.z, rate);
	}
}

function Collide (hit : RaycastHit) {
	stopTime = -1;
	var val : float;

	val = ((dist-minDist)-(hit.distance-minDist))/(dist-minDist);
	val = Mathf.Min(val, 1);
	targetRot = rot*val;
	targetPos = pos*val;
	collided = true;
	canAim = false;
}