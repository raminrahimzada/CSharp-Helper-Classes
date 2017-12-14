#pragma strict
var moveSwayRate : Vector2;
var moveSwayAmplitude : Vector2;
var runSwayRate : Vector2;
var runSwayAmplitude : Vector2;
var swayStartTime : float = 0;
var idleSwayRate : Vector2;
var idleAmplitude : Vector2;

private var val : Vector3;
private var lastVal : Vector3;
private var swayRate : Vector2;
private var swayAmplitude : Vector2;
static var singleton : CamSway;
var curJostle : Vector3;
var lastJostle : Vector3;
static var jostleAmt : Vector3;

function Awake () {
	singleton = this;
}

function WalkSway(){	
	if(swayStartTime > Time.time)
			swayStartTime = Time.time;
	var CM : CharacterMotorDB = PlayerWeapons.CM;
	var speed : int = CM.GetComponent(CharacterController).velocity.magnitude;
	
	//Jostle
	lastJostle = curJostle;
	curJostle = Vector3.Lerp(curJostle, jostleAmt, Time.deltaTime*16);
	jostleAmt = Vector3.Lerp(jostleAmt, Vector3.zero, Time.deltaTime*4);
	transform.localPosition += (curJostle - lastJostle)*15;
	
	if(speed < .2){
		ResetPosition();
		return;
	}
	//sine function for motion
	var t : float = Time.time - swayStartTime;
	var curVect : Vector3;
	swayAmplitude = moveSwayAmplitude;
	if(CM.crouching){
		swayRate = moveSwayRate*CM.movement.maxCrouchSpeed/CM.movement.defaultForwardSpeed;
	} else if (CM.prone) {
		swayRate = moveSwayRate*CM.movement.maxProneSpeed/CM.movement.defaultForwardSpeed;
	} else if (AimMode.sprintingPublic) {
		swayRate = runSwayRate;
		swayAmplitude = runSwayAmplitude;
	} else {
		swayRate = moveSwayRate;
	}
	curVect.x = swayAmplitude.x*Mathf.Sin(swayRate.x*t);//*Mathf.Sin(swayRate.x*speed/14*t);
	curVect.y = Mathf.Abs(swayAmplitude.y*Mathf.Sin(swayRate.y*t));
	
	curVect.x -= swayAmplitude.x/2;
	curVect.y -= swayAmplitude.y/2;

	//Move
	lastVal = val;
	val.x = Mathf.Lerp(val.x, curVect.x, Time.deltaTime*swayRate.x);
	transform.localEulerAngles.z = Mathf.LerpAngle(transform.localEulerAngles.z, -curVect.x*.5, Time.deltaTime*swayRate.x);
	
	val.y = Mathf.Lerp(val.y, curVect.y, Time.deltaTime*swayRate.y);
	transform.localEulerAngles.x = Mathf.LerpAngle(transform.localEulerAngles.x, -curVect.y*.5, Time.deltaTime*swayRate.y);
	//transform.localPosition.x = Vector3.Lerp(transform.localPosition.x, curVect.x, Time.deltaTime*swayRate.x);
	
	transform.localPosition.x += val.x - lastVal.x;
	transform.localPosition.y += val.y - lastVal.y;
}

function ResetPosition(){
	swayStartTime = 9999999999999;
	if(transform.localPosition == Vector3(0,0,0)){
		return;
	}
	
	//Move
	lastVal = val;
	
	val.x = Mathf.Lerp(val.x, 0, Time.deltaTime*idleSwayRate.x);	
	transform.localEulerAngles.z = Mathf.LerpAngle(transform.localEulerAngles.z, 0, Time.deltaTime*idleSwayRate.x);
	
	val.y = Mathf.Lerp(val.y, 0, Time.deltaTime*idleSwayRate.y);
	transform.localEulerAngles.x = Mathf.LerpAngle(transform.localEulerAngles.x, 0, Time.deltaTime*idleSwayRate.y);
	
	transform.localPosition.x += val.x - lastVal.x;
	transform.localPosition.y += val.y - lastVal.y;
	
}

function Update () {
	if(!AimMode.staticAiming && PlayerWeapons.CM.grounded && !CharacterMotorDB.paused){// && CM.walking){
 		WalkSway();
 	} else {
 		ResetPosition();
 	}
}