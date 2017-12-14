var multiplier : float = 1.5;
private var cache: float;
private var applied : boolean = false;

function Apply (gScript : GunScript) {
	cache = gScript.damage*(multiplier-1);
	gScript.damage += cache;
}
function Remove (gScript : GunScript) {
	gScript.damage -= cache;
}