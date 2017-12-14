
var PointValue : int = 1;
var SparkleFX : GameObject;
var SpoldeFX : GameObject;
private var SpoldeFXclone : GameObject;

function Update() {
	if ( this.gameObject.active && !SparkleFX.active ) {
		SparkleFX.active = true;
	}
}

function OnTriggerEnter (other : Collider) {
    
    if ( other.tag == "creature" ) {
    	
    	SpoldeFXclone = Instantiate(SpoldeFX, transform.position, transform.rotation);
    	
    	var PlayerScore : CrystalGame = other.transform.GetComponent("CrystalGame");
    	PlayerScore.Score += PointValue;
		this.gameObject.active = false;
		SparkleFX.active = this.gameObject.active;
		
    }
}