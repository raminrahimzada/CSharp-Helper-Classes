var multiplier : float = 1.5;
private var cache: float;
private var applied : boolean = false;

function Apply (gScript : GunScript) {
	cache = gScript.aimSpreadRate*(multiplier-1);
	gScript.aimSpreadRate += cache;
}
function Remove (gScript : GunScript) {
	gScript.aimSpreadRate -= cache;
}