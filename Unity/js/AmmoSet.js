var val : int;
private var cache: int;
private var applied : boolean = false;

function Apply (gscript : GunScript) {
	gscript.ApplyToSharedAmmo();
	cache = gscript.ammoSetUsed;
	gscript.ammoSetUsed = val;
	gscript.AlignToSharedAmmo();
}
function Remove (gscript : GunScript) {
	gscript.ApplyToSharedAmmo();
	gscript.ammoSetUsed = cache;
	gscript.AlignToSharedAmmo();
}