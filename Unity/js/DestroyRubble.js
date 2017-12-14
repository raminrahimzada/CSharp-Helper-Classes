var maxTime : float = 3;
var particleEmitters : ParticleEmitter[];
var time : float;

function Start () {
	yield WaitForSeconds(time);
	for(i=0;i<particleEmitters.length;i++){
		
		
		particleEmitters[i].emit = false;
		yield WaitForSeconds(maxTime);
		Destroy(gameObject);
		
	}
	
}
