#pragma strict
var multiplier : float = 1;
private var curAmp : float;
private var lastVal : Vector3;
private var timeVal : float;
private var amplitude : float;
private var r : float;

static var shakers : CameraShake[];
static var tempShakers = new Array();
static var builtin : boolean = false;

function Awake(){
	tempShakers.Add(this);
}

function Start () {
	if(!builtin){
		shakers = tempShakers.ToBuiltin(CameraShake) as CameraShake[];
		builtin = true;
	}
}


static function ShakeCam (a : float, rT : float, time : float){
	for(var s : CameraShake in shakers){
		s.Shake(a, rT, time);
	}
}

function Shake (a : float, rT : float,  time : float) {
	amplitude = a*multiplier;
	curAmp = amplitude;
	timeVal = time;
	r = rT;
}

function LateUpdate () {
	//if(InputDB.GetButtonDown("Interact")){
	//	ShakeCam(.14, 10, .4);
	//}
	if(curAmp > 0){
		var amt : Vector3 = Random.insideUnitSphere * curAmp;
		transform.localPosition -= lastVal;
		transform.localEulerAngles -= lastVal*r;
		transform.localEulerAngles += amt*r;
		transform.localPosition += amt;
		lastVal = amt;
		curAmp -= Time.deltaTime*amplitude/timeVal;
	}
}