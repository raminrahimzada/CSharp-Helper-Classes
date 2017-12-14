var multiplier : float = 1.5;
private var cache: float;
private var applied : boolean = false;

function Apply (gscript : GunScript) {
	cache = gscript.force*(multiplier-1);
	gscript.force += cache;
}
function Remove (gscript : GunScript) {
	gscript.force -= cache;
}