var gscript : GunScript;
var emitters : ParticleEmitter[];
var emitting : boolean = false;
var specialEmitter : ParticleEmitter;
var minSpecial : float;

function LateUpdate () {
	this.GetComponentInChildren(Light).range = gscript.chargeLevel*10;
	if(gscript.chargeLevel > 0 || emitting){
		gscript.gameObject.GetComponent(AudioSource).pitch = gscript.chargeLevel;
		if(!emitting){
			emitCharge(true);
		} else if (emitting) {
			emitCharge(false);
		}
	} else {
		gscript.gameObject.GetComponent(AudioSource).pitch = gscript.firePitch;
		specialEmitter.emit = false;
	}
}

function emitCharge (s : boolean) {
	for(var i : int = 0; i < emitters.length; i++){
		emitters[i].emit = s;
	}
	if(gscript.chargeLevel > minSpecial){
		specialEmitter.emit = true;
	} else {
		specialEmitter.emit = false;
	}

	emitting = s;
}

