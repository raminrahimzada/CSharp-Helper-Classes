/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
 For additional information contact us info@dastardlybanana.com.
*/

class EffectSet extends System.Object{
	var maxOfEach : int = 15; //You can increase this if desired
	var setID : int = 0;
	var setName : String = "New Set";
	var localMax : int = 20;
	
	var blankGameObject : GameObject;
	
	var bulletDecals : GameObject[] = new GameObject[maxOfEach];
	var bulletDecalsFolded : boolean = false;
	var lastBulletDecal : int = 0;
	
	var dentDecals : GameObject[] = new GameObject[maxOfEach];
	var dentDecalsFolded : boolean = false;
	var lastDentDecal : int = 0;
	
	var hitParticles : GameObject[] = new GameObject[maxOfEach];
	var hitParticlesFolded : boolean = false;
	var lastHitParticle : int = 0;
	
	var footstepSounds : AudioClip[] = new AudioClip[maxOfEach];
	var footstepSoundsFolded : boolean = false;
	var lastFootstepSound : int = 0;
	
	var crawlSounds : AudioClip[] = new AudioClip[maxOfEach];
	var crawlSoundsFolded : boolean = false;
	var lastCrawlSound : int = 0;
	
	var bulletSounds : AudioClip[] = new AudioClip[maxOfEach];
	var bulletSoundsFolded : boolean = false;
	var lastBulletSound : int = 0;
	
	var collisionSounds : AudioClip[] = new AudioClip[maxOfEach];
	var collisionSoundsFolded : boolean = false;
	var lastCollisionSound : int = 0;
}