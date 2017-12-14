var smokeDestroyTime : float = 6;
var destroySpeed : float = .05;

private var destroyEnabled = false;

function Start () {
	yield WaitForSeconds(smokeDestroyTime);
	destroyEnabled = true;
}

function Update () {
	if(destroyEnabled == true){
		var render = GetComponent(ParticleRenderer);
		var col : Color = render.materials[1].GetColor("_TintColor");
		col.a -= destroySpeed*Time.deltaTime;
		render.materials[1].SetColor("_TintColor",col);
	}
}