var val : boolean;
private var cache: boolean;
private var applied : boolean = false;


function Apply (gScript : GunScript) {
	cache = gScript.burstFire;
	gScript.burstFire = val;
}
function Remove (gScript : GunScript) {
	gScript.burstFire = cache;
}