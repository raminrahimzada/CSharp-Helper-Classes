var val : int;
private var cache: int;
private var applied : boolean = false;

function Apply (gscript : GunScript) {
	cache = val-gscript.burstCount;
	gscript.burstCount += cache;
}
function Remove (gscript : GunScript) {
	gscript.burstCount -= cache;
}