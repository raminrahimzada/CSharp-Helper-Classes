//FPS Constructor - Weapons
//Copyright© Dastardly Banana Productions 2010
//This script, and all others contained within the Dastardly Banana Weapons Package, may not be shared or redistributed. They can be used in games, either commerical or non-commercial, as long as Dastardly Banana Productions is attributed in the credits.
//Permissions beyond the scope of this license may be available at mailto://info@dastardlybanana.com.

//Custom editor 
enum crosshairTypes {Friend, Foe, Other}
var crosshairType : crosshairTypes;

private var weaponCam : GameObject;

function Start(){
	weaponCam = GameObject.FindWithTag("WeaponCamera");
}