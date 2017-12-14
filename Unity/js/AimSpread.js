var multiplier : float = 1.5;
private var cache: float;
private var applied : boolean = false;

function Apply (gScript : GunScript) {
	cache = gScript.aimSpread*(multiplier-1);
	gScript.aimSpread += cache;
}
function Remove (gScript : GunScript) {
	gScript.aimSpread -= cache;
}