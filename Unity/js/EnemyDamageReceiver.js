#pragma strict
#pragma downcast

var hitPoints = 100.0;
var deathEffect : Transform;
var effectDelay = 0.0;
private var gos : GameObject[];
var multiplier : float = 1;
var deadReplacement : Rigidbody;
@HideInInspector
var playerObject : GameObject;
var useHitEffect : boolean = true;

@HideInInspector
var isEnemy : boolean = false;

function ApplyDamage(Arr : Object[]){
	//Info array contains damage and value of fromPlayer boolean (true if the player caused the damage)
	//Find the player if we haven't
	if(Arr[1] == true){
		if(!playerObject){
			playerObject = GameObject.FindWithTag("Player");
		}
		if(useHitEffect){
			playerObject.BroadcastMessage("HitEffect");
		}
	}
	
	// We already have less than 0 hitpoints, maybe we got killed already?
	if (hitPoints <= 0.0)
		return;
	var tempFloat : float;
	tempFloat = Arr[0];
	//float.TryParse(Arr[0], tempFloat);
	hitPoints -= tempFloat*multiplier;
	if (hitPoints <= 0.0) {
		// Start emitting particles
		var emitter : ParticleEmitter = GetComponentInChildren(ParticleEmitter);
		if (emitter)
			emitter.emit = true;

		Invoke("DelayedDetonate", effectDelay);
	}
}

function ApplyDamagePlayer (damage : float){
	//Info array contains damage and value of fromPlayer boolean (true if the player caused the damage)
	//Find the player if we haven't
	if(!playerObject){
		playerObject = GameObject.FindWithTag("Player");
	}
	if(useHitEffect){
		playerObject.BroadcastMessage("HitEffect");
	}
	
	// We already have less than 0 hitpoints, maybe we got killed already?
	if (hitPoints <= 0.0)
		return;
	//float.TryParse(Arr[0], tempFloat);
	hitPoints -= damage*multiplier;
	if (hitPoints <= 0.0) {
		// Start emitting particles
		var emitter : ParticleEmitter = GetComponentInChildren(ParticleEmitter);
		if (emitter)
			emitter.emit = true;

		Invoke("DelayedDetonate", effectDelay);
	}
}

function ApplyDamage (damage : float){
	//Info array contains damage and value of fromPlayer boolean (true if the player caused the damage)
	//Find the player if we haven't
	
	// We already have less than 0 hitpoints, maybe we got killed already?
	if (hitPoints <= 0.0)
		return;

	//float.TryParse(Arr[0], tempFloat);
	hitPoints -= damage*multiplier;
	if (hitPoints <= 0.0) {
		// Start emitting particles
		var emitter : ParticleEmitter = GetComponentInChildren(ParticleEmitter);
		if (emitter)
			emitter.emit = true;

		Invoke("DelayedDetonate", effectDelay);
	}
}

function DelayedDetonate(){
	BroadcastMessage ("Detonate");
}

function Detonate(){
	if(isEnemy)
		EnemyMovement.enemies--;
	// Create the deathEffect
	if (deathEffect)
		Instantiate (deathEffect, transform.position, transform.rotation);

	// If we have a dead replacement then replace ourselves with it!
	if (deadReplacement) {
		var dead : Rigidbody = Instantiate(deadReplacement, transform.position, transform.rotation);

		// For better effect we assign the same velocity to the exploded gameObject
		dead.rigidbody.velocity = rigidbody.velocity;
		dead.angularVelocity = rigidbody.angularVelocity;
	}
	
	// If there is a particle emitter stop emitting and detach so it doesnt get destroyed right away
	var emitter : ParticleEmitter = GetComponentInChildren(ParticleEmitter);
	if (emitter){
		emitter.emit = false;
		emitter.transform.parent = null;
	}
	BroadcastMessage ("Die", SendMessageOptions.DontRequireReceiver);
	Destroy(gameObject);

}