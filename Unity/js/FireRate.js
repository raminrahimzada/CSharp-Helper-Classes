var multiplier : float = 1.5;
private var cache: float;
private var applied : boolean = false;


function Apply (gScript : GunScript) {
	cache = gScript.fireRate*(multiplier-1);
	gScript.fireRate += cache;
}
function Remove (gScript : GunScript) {
	gScript.fireRate -= cache;
}