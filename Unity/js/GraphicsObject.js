var obj : GameObject;
var val : boolean = true;
var instant : boolean = false;
private var cache: boolean;
private var applied : boolean = false;
private var tempInstant : boolean = false;

function Apply () {
	cache = obj.activeSelf;
	if(val){
		obj.SetActive(true);
	} else {
		obj.SetActive(false);
	}
	
	var gos = obj.GetComponentsInChildren(Renderer);
	var go : Renderer;
	
	if(!instant && !tempInstant) {	
		for(go in gos){
			go.enabled=false;
		}
	} else {
		for( go in gos){
			go.enabled=true;
		}
	}
	tempInstant = false;
	
	transform.parent.BroadcastMessage("reapply", SendMessageOptions.DontRequireReceiver);
}
function Remove () {
	obj.SetActive(cache);
	if(instant || tempInstant){
		var gos = obj.GetComponentsInChildren(Renderer);
		var go : Renderer;
		for( go in gos){
			go.enabled=true;
		}
		tempInstant = false;
	}
}

function TempInstant () {
	tempInstant = true;
}