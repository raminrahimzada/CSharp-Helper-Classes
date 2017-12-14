var val : boolean;
private var cache: boolean;
private var applied : boolean = false;

function Apply (gScript : GunScript) {
	cache = gScript.autoFire;
	gScript.autoFire = val;
}
function Remove (gScript : GunScript) {
	gScript.autoFire = cache;
}