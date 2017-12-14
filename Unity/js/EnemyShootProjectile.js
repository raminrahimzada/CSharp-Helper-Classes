#pragma strict
private var nextAttackTime : float;
var pos : Transform;
var projectile : Rigidbody;
var initialSpeed : float = 50;
var fireRate : float = 1;
var actualSpread : float = .2;
var emitter : ParticleEmitter;
var backForce : float = 10;

function Attack () {
	if(Time.time < nextAttackTime)
		return;
	nextAttackTime = Time.time + fireRate;
	//function Fire (penetration : int, damage : float, force : float, tracer : GameObject, direction : Vector3, firePosition : Vector3) {
	FireProjectile();
}

function FireProjectile () {
	var direction : Vector3 = SprayDirection();
	var convert : Quaternion = Quaternion.LookRotation(direction+Vector3(0,.04,0));
	
	var instantiatedProjectile : Rigidbody;
	instantiatedProjectile = Instantiate (projectile, pos.position, convert);
	instantiatedProjectile.velocity = instantiatedProjectile.transform.TransformDirection(Vector3 (0, 0, initialSpeed));
	Physics.IgnoreCollision(instantiatedProjectile.collider, transform.root.collider);
	emitter.Emit();
	transform.root.rigidbody.AddRelativeForce(Vector3(0,0,-backForce), ForceMode.Impulse);
}

function SprayDirection(){
	var vx = (1 - 2 * Random.value) * actualSpread;
	var vy = (1 - 2 * Random.value) * actualSpread;
	var vz = 1.0;
	return transform.TransformDirection(Vector3(vx,vy,vz));
}