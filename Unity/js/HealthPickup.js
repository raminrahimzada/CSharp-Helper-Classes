#pragma strict
var amount : int;
private var p : PlayerHealth;
var delay : float;
var destroys : boolean = false;
private var nextTime : float = 0;
var limited : boolean;
var limit : int;

//Called via message
//Adds health to player
function Interact () {
	if(Time.time > nextTime && (limit || !limited)){ //if it has been long enough, and we are either not past our limit or not limited
		nextTime = Time.time + delay; //set next time
		p = PlayerWeapons.player.GetComponent(PlayerHealth);
		if(p.health < p.maxHealth){ //if we aren't at full health already
			p.health = Mathf.Clamp(p.health+amount, p.health, p.maxHealth); //add health up to max
			if(audio){
				var audioObj : GameObject = new GameObject("PickupSound");
				audioObj.transform.position = transform.position;
				audioObj.AddComponent(TimedObjectDestructorDB).timeOut = audio.clip.length + .1;;
				var aO : AudioSource = audioObj.AddComponent(AudioSource); //play sound
				aO.clip = audio.clip;
				aO.volume = audio.volume;
				aO.pitch = audio.pitch;
				aO.Play();
				aO.loop = false;
				aO.rolloffMode = AudioRolloffMode.Linear;
			}
			limit--; //decrement limit
		}
	}
	if(limit <= 0 && destroys)
		Destroy(gameObject);
}
