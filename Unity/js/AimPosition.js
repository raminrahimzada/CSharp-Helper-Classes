@HideInInspector var gscript : AimMode;
var val : Vector3;
private var cache: Vector3;
private var applied : boolean = false;

function Start () {
	gscript = this.transform.parent.GetComponent(GunScript).GetComponentInChildren(AimMode);
}
function Apply () {
	cache = gscript.aimPosition1;
	gscript.aimPosition1 = val;
	gscript.aimPosition = val;

}
function Remove () {
	gscript.aimPosition1 = cache;
	gscript.aimPosition = cache;
}