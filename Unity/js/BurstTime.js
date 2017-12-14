var multiplier : float = 1.5;
private var cache: float;
private var applied : boolean = false;

function Apply (gScript : GunScript) {
	cache = gScript.burstTime*(multiplier-1);
	gScript.burstTime += cache;
}
function Remove (gScript : GunScript) {
	gScript.burstTime -= cache;
	cache = 0;
}