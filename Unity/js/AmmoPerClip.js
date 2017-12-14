var val : int;
private var cache: int;
private var applied : boolean = false;

function Apply (gscript : GunScript) {
	cache = val-gscript.ammoPerClip;
	gscript.ammoPerClip += cache;
	if(gscript.ammoLeft > gscript.ammoPerClip)
		gscript.ammoLeft = gscript.ammoPerClip;
}
function Remove (gscript : GunScript) {
	gscript.ammoPerClip -= cache;
	if(gscript.ammoLeft > gscript.ammoPerClip)
		gscript.ammoLeft = gscript.ammoPerClip;
}

///when decreasing clip size add to clip reserve