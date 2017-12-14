var val : String;
private var cache: String;
private var applied : boolean = false;


function Apply (gScript : GunScript) {
	cache = gScript.GetComponentInChildren(GunChildAnimation).emptyReloadAnim;
	gScript.GetComponentInChildren(GunChildAnimation).emptyReloadAnim = val;
}
function Remove (gScript : GunScript) {
	gScript.GetComponentInChildren(GunChildAnimation).emptyReloadAnim = cache;
}