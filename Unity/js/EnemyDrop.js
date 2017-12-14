#pragma strict
var drops : Transform;
var min : int = 0;
var max : int = 5;
var force : float = 5;

function Die () {
	var amt : int = Random.Range (min, max);
	var i : int = 0;
	var t : Transform;
	var dir : Vector3;
	while(i < amt){
		t = Instantiate(drops,transform.position+Vector3(0,2,0), transform.rotation);
		
		dir = Random.insideUnitSphere*force;
		t.rigidbody.AddForce(dir, ForceMode.Impulse);
		t.rigidbody.AddTorque(dir, ForceMode.Impulse);
		i++;
	}
}