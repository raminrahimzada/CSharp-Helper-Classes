#pragma strict
var cubes : Transform[];
var amts : int[];

function SpawnCS (pos : Transform, w : Waypoint, t : float) {
	var spawned : Transform;
	for(var j : int = 0; j < cubes.length; j++){
		for(var q : int = 0; q < amts[j]; q++){
			spawned = Instantiate(cubes[j], pos.position+Vector3(0,4,0)*q, pos.rotation);
			EnemyMovement.enemies++;
			spawned.GetComponent(EnemyMovement).waypoint = w.transform;
			yield new WaitForSeconds(t);
		}
	}
}