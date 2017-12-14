var val : String;
private var cache: String;
private var applied : boolean = false;


function Apply (gScript : GunScript) {
	cache = gScript.GetComponentInChildren(GunChildAnimation).fireAnim;
	gScript.GetComponentInChildren(GunChildAnimation).fireAnim = val;
}
function Remove (gScript : GunScript) {
	gScript.GetComponentInChildren(GunChildAnimation).fireAnim = cache;
}