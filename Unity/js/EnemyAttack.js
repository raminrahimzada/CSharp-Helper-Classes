#pragma strict
private var target : Transform;
private var nextAttackTime : float;
var damage : float;
var attackTime : float;

function Start () {
	target = PlayerWeapons.weaponCam.transform;
}

function Attack () {
	if(Time.time < nextAttackTime)
		return;
	var sendArray : Object[] = new Object[2];
	sendArray[0] = damage;
	sendArray[1] = false;		
	target.SendMessageUpwards("ApplyDamage", sendArray, SendMessageOptions.DontRequireReceiver);
	target.SendMessageUpwards("Direction", transform, SendMessageOptions.DontRequireReceiver);
	nextAttackTime = Time.time + attackTime;
	animation.Play();
}
