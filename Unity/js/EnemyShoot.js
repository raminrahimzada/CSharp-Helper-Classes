#pragma strict
private var target : Transform;
private var nextAttackTime : float;
var damage : float;
var force : float;
var fireRate : float;
var fire : Fire;
var tracer : GameObject;
var shootPos : Transform;
var actualSpread : float;

function Start () {
	target = PlayerWeapons.weaponCam.transform;
}

function Attack () {
	if(Time.time < nextAttackTime)
		return;
	nextAttackTime = Time.time + fireRate;
	//function Fire (penetration : int, damage : float, force : float, tracer : GameObject, direction : Vector3, firePosition : Vector3) {
	fire.Fire(0, damage, force, tracer, SprayDirection(), shootPos.position);
}

function SprayDirection(){
	var vx = (1 - 2 * Random.value) * actualSpread;
	var vy = (1 - 2 * Random.value) * actualSpread;
	var vz = 1.0;
	return transform.TransformDirection(Vector3(vx,vy,vz));
}