/*
 FPS Constructor - Weapons
 Copyright� Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

@CustomEditor (GunScript)
class GunScriptEditor extends Editor {

var displayGun : boolean = false;
var gunDisplayed : boolean = false;
var gunTipo = gunTypes.hitscan;

	function OnInspectorGUI() {
		EditorGUIUtility.LookLikeInspector();
		//EditorGUILayout.BeginVertical();
	
		target.gunType = EditorGUILayout.EnumPopup(GUIContent("  Gun Type: ", "The basic type of weapon - choose 'hitscan' for a basic bullet-based weapon"), target.gunType);
		gunTipo = target.gunType;
		
	
		if(gunTipo == gunTypes.spray){
			target.sprayObj = EditorGUILayout.ObjectField(GUIContent("  Spray Object: ", "Spray weapons need an attached object with a particle collider and the script SprayScript"), target.sprayObj, GameObject, true);			
		}else if(gunTipo == gunTypes.launcher){
			target.launchPosition = EditorGUILayout.ObjectField(GUIContent("  Launch Position: ","The projectile will be instantiated at the position of the object in this field"), target.launchPosition, GameObject, true);
		}
			
		EditorGUILayout.Separator();
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button(new GUIContent("Open All", "Open all the foldout menus"), "miniButton")){
			target.shotPropertiesFoldout = true;
			target.firePropertiesFoldout = true;
			target.accuracyFoldout = true;
			target.altFireFoldout = true;
			target.ammoReloadFoldout = true;
			target.audioVisualFoldout = true;
		}
		
		if(GUILayout.Button(new GUIContent("Close All", "Close all the foldout menus"), "miniButton")){
			target.shotPropertiesFoldout = false;
			target.firePropertiesFoldout = false;
			target.accuracyFoldout = false;
			target.altFireFoldout = false;
			target.ammoReloadFoldout = false;
			target.audioVisualFoldout = false;
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
			
		//Shot properties
		if(gunTipo != gunTypes.melee){
			EditorGUILayout.BeginVertical("toolbar");
			target.shotPropertiesFoldout = EditorGUILayout.Foldout(target.shotPropertiesFoldout, "Shot Properties (damage etc.):");
			EditorGUILayout.EndVertical();
		}else{
			target.shotPropertiesFoldout = false;
		}
		if(target.shotPropertiesFoldout){
			EditorGUILayout.BeginVertical("textField");
				EditorGUILayout.Separator();
				if(gunTipo == gunTypes.launcher){
					target.projectile = EditorGUILayout.ObjectField(GUIContent("  Projectile: ", "This is the actual GameObject to be instantiated from the launcher"), target.projectile,  Rigidbody, false);
					target.initialSpeed = EditorGUILayout.FloatField(GUIContent("  Initial Speed: ", "The initial speed each projectile will have when fired"), target.initialSpeed);
				}
				if(gunTipo == gunTypes.hitscan || gunTipo == gunTypes.spray){
					target.range = EditorGUILayout.FloatField(GUIContent("  Range: ", "The maximum range this gun can hit at"), target.range);
					target.force = EditorGUILayout.FloatField(GUIContent("  Force: ", "The force which is applied to the object we hit"), target.force);
					if(gunTipo == gunTypes.spray){
						target.damage = EditorGUILayout.FloatField(GUIContent("  Damage Per Second:  ", "The damage done by the weapon over each second of continous fire"), target.damage);
					}else{
						target.penetrateVal = EditorGUILayout.FloatField(GUIContent("  Penetration Level: ","If your scene is set up to use bullet penetration, this determines the piercing ability of this weapon"), target.penetrateVal);
						target.damage = EditorGUILayout.FloatField(GUIContent("  Damage: ","The damage done per bullet by this weapon"), target.damage);
					}
					if(target.chargeWeapon){
						target.chargeCoefficient = EditorGUILayout.FloatField(GUIContent("  Charge Coefficient: ", "Multiply this weapon's damage by this number to the power of the current charge level (A value of 1.1 would lead to a 10% increase in damage after charging for one second, for example"), target.chargeCoefficient);
					}
					if(gunTipo == gunTypes.spray){
						target.hasFalloff = true;
					}else{
						target.hasFalloff = EditorGUILayout.Toggle(GUIContent("  Use Damage Falloff: ", "Does this weapon's damage change with distance?"), target.hasFalloff);
					}
					if(target.hasFalloff){
						var tempDamageDisplay : float = (target.damage * Mathf.Pow(target.falloffCoefficient, (target.maxFalloffDist - target.minFalloffDist)/target.falloffDistanceScale));
						EditorGUILayout.LabelField("  Damage at max falloff: ", "" + tempDamageDisplay);
						var tempForceDisplay : float = (target.force * Mathf.Pow(target.forceFalloffCoefficient, (target.maxFalloffDist - target.minFalloffDist)/target.falloffDistanceScale));
						EditorGUILayout.LabelField("  Force at max falloff: ", "" + tempForceDisplay);
						target.minFalloffDist = EditorGUILayout.FloatField(GUIContent("  Min. Falloff Distance: ", "Falloff is not applied to hits closer than this distance"), target.minFalloffDist);	
						target.maxFalloffDist = EditorGUILayout.FloatField(GUIContent("  Max. Falloff Distance: ", "Falloff is not applied to hits past this distance"), target.maxFalloffDist);		
						target.falloffCoefficient = EditorGUILayout.FloatField(GUIContent("  Damage Coefficient: ", "The weapon's damage is multiplied by this for every unit of distance"), target.falloffCoefficient);
						target.forceFalloffCoefficient = EditorGUILayout.FloatField(GUIContent("  Force Coefficient: ", "The force applied is multiplied by this every unit of distance"), target.forceFalloffCoefficient);	
						target.falloffDistanceScale = EditorGUILayout.FloatField(GUIContent("  Falloff Distance Scale: ", "This defines how many unity meters make up one 'unit' of distance in falloff calculations"), target.falloffDistanceScale);
					}
					if(gunTipo == gunTypes.melee){
						target.damage = 0;
						target.force = 0;
					}
					EditorGUILayout.Separator();
				EditorGUILayout.EndVertical();
			}
		}
			
		//Fire Properties
		if(gunTipo != gunTypes.spray){
			EditorGUILayout.BeginVertical("toolbar");
			target.firePropertiesFoldout = EditorGUILayout.Foldout(target.firePropertiesFoldout, "Fire Properties (fire rate etc.):");
			EditorGUILayout.EndVertical();
		}else{
			target.firePropertiesFoldout = false;
		}
		if(target.firePropertiesFoldout){
			EditorGUILayout.BeginVertical("textField");
			EditorGUILayout.Separator();
			if(gunTipo == gunTypes.melee){
				target.shotCount = 0;
				target.fireRate = EditorGUILayout.FloatField(GUIContent("  Attack Rate:  ", "Attacks per second"), target.fireRate);
				target.delay = EditorGUILayout.FloatField(GUIContent("  Damage Delay:  ", "How long into the attack does the hitbox activate, in seconds. The hit box is active during this time"), target.delay);
				target.reloadTime = EditorGUILayout.FloatField(GUIContent("  Recovery Time:  ", "The time, in seconds, after each attack before it is possible to attack again"), target.reloadTime);
			}

			if(gunTipo == gunTypes.hitscan || gunTipo == gunTypes.launcher){
				if(gunTipo == gunTypes.hitscan){
					target.shotCount = EditorGUILayout.IntField(GUIContent("  Shot Count: ","The number of shots fired per pull of the trigger"), target.shotCount);
				}else{
					target.projectileCount = EditorGUILayout.FloatField(GUIContent("  Projectile Count: ", "The number of projectiles fired with every pull of the trigger"), target.projectileCount);
				}
				target.fireRate = EditorGUILayout.FloatField(GUIContent("  Fire Rate: ","The time in seconds after firing before the weapon can be fired again"), target.fireRate);
				target.chargeWeapon = EditorGUILayout.Toggle(GUIContent("  Charge Weapon: ","Charge weapons are weapons which can be 'charged up' by holding down fire. Charge is measured in an internal variable called 'chargeLevel'"), target.chargeWeapon);
				if(target.chargeWeapon){
					target.autoFire = false;
					target.minCharge = EditorGUILayout.FloatField(GUIContent("  Minimum Charge: ", "The weapon cannot fire unless it has charged up to at least this value"), target.minCharge);
					target.maxCharge = EditorGUILayout.FloatField(GUIContent("  Maximum Charge: ", "The weapon cannot charge up beyond this value"), target.maxCharge);
					target.forceFire = EditorGUILayout.Toggle(GUIContent("  Must Fire When Charged: ", "If this is checked, the weapon will be automatically discharged the moment it is fully charged"), target.forceFire);
					if(target.forceFire){
						target.chargeAuto = EditorGUILayout.Toggle(GUIContent("  Charge after force release: ", "When the weapon dischrages because it reaches maximum charge, and chargeAuto is enabled, the weapon will begin charging again immediately if able. If unchecked, the player will have to release the mouse and press it to start charging again"), target.chargeAuto);
					}
				}
				if(!target.autoFire){
					target.burstFire = EditorGUILayout.Toggle(GUIContent("  Burst Fire: ","Does this weapon fire in bursts?"), target.burstFire);
				}
				if(target.burstFire){	
					target.burstCount = EditorGUILayout.IntField(GUIContent("  Burst Count: ","How many shots fired per burst?"), target.burstCount);
					target.burstTime = EditorGUILayout.FloatField(GUIContent("  Burst Time: ","How long does it take to fire the full burst?"), target.burstTime);
				}
				if(!target.burstFire && !target.chargeWeapon){
					target.autoFire = EditorGUILayout.Toggle(GUIContent("  Full Auto: ","Is this weapon fully automatic?"), target.autoFire);
				}
			}
			EditorGUILayout.Separator();
			EditorGUILayout.EndVertical();
		}
			
		//Accuracy
		if(gunTipo != gunTypes.melee){
			EditorGUILayout.BeginVertical("toolbar");
			target.accuracyFoldout = EditorGUILayout.Foldout(target.accuracyFoldout, "Accuracy Properties:");
			EditorGUILayout.EndVertical();
		}else{
			target.accuracyFoldout = false;
		}
		
		if(target.accuracyFoldout){
			EditorGUILayout.BeginVertical("textField");
				EditorGUILayout.Separator();
				if(gunTipo == gunTypes.hitscan || gunTipo == gunTypes.launcher){
					target.standardSpread = EditorGUILayout.FloatField(GUIContent("  Standard Spread: ","The weapon's spread when firing from the hip and standing still. A spread of 1 means the shot could go 90 degrees in any direction; .5 would be 45 degrees"), target.standardSpread);
					target.standardSpreadRate = EditorGUILayout.FloatField(GUIContent("  Spread Rate: ","The rate at which the weapon's spread increases while firing from the hip. This value is added to the spread after every shot"), target.standardSpreadRate);
					target.spDecRate = EditorGUILayout.FloatField(GUIContent("  Spread Decrease Rate: ","The rate at which the spread returns to normal."), target.spDecRate);
					target.maxSpread = EditorGUILayout.FloatField(GUIContent("  Maximum Spread: ","The weapon's spread will not naturally exceed this value."), target.maxSpread);
					EditorGUILayout.Separator();
					target.aimSpread = EditorGUILayout.FloatField(GUIContent("  Aim Spread: ","The weapon's spread when firing in aim mode.  A spread of 1 means the shot could go 90 degrees in any direction; .5 would be 45 degrees"), target.aimSpread);
					target.aimSpreadRate = EditorGUILayout.FloatField(GUIContent("  Aim Spread Rate: ","The rate at which the weapon's spread increases while firing in aim mode. This value is added to the spread after every shot"), target.aimSpreadRate);
					target.crouchSpreadModifier = EditorGUILayout.FloatField(GUIContent("  Crouch Spread Modifier: ","How the weapon's spread is modified while crouching. The spread while crouching is multiplied by this number"), target.crouchSpreadModifier);
					target.proneSpreadModifier = EditorGUILayout.FloatField(GUIContent("  Prone Spread Modifier: ","How the weapon's spread is modified while prone. The spread while prone is multiplied by this number"), target.proneSpreadModifier);
					target.moveSpreadModifier = EditorGUILayout.FloatField(GUIContent("  Move Spread Modifier: ","How the weapon's spread is modified while moving. The spread while moving is multiplied by this number"), target.moveSpreadModifier);
					EditorGUILayout.Separator();
				}
				if(gunTipo != gunTypes.melee){
					target.kickbackAngle = EditorGUILayout.FloatField(GUIContent("  Vertical Recoil (Angle): ","The maximum vertical angle per shot which the user's view will be incremented by"), target.kickbackAngle);
					target.xKickbackFactor = EditorGUILayout.FloatField(GUIContent("  Horizontal Recoil (Factor): ", "Factor relative to vertical recoil which horizontal recoil will use"), target.xKickbackFactor);
					target.maxKickback = EditorGUILayout.FloatField(GUIContent("  Maximum Recoil: ", "The maximum TOTAL angle on the x axis the weapon can recoil"), target.maxKickback);
					target.recoilDelay = EditorGUILayout.FloatField(GUIContent("  Recoil Delay: ", "The time before the weapon returns to its normal position from recoil"), target.recoilDelay);
					
					target.kickbackAim = EditorGUILayout.FloatField(GUIContent("  Aim Recoil (Angle): ","Aim recoil in degrees"), target.kickbackAim);
					target.crouchKickbackMod = EditorGUILayout.FloatField(GUIContent("  Crouch Recoil (Multi): ","Crouch recoil multiplier"), target.crouchKickbackMod);
					target.proneKickbackMod = EditorGUILayout.FloatField(GUIContent("  Prone Recoil (Multi): ","Prone recoil multiplier"), target.proneKickbackMod);
					target.moveKickbackMod = EditorGUILayout.FloatField(GUIContent("  Move Recoil (Multi): ","Move recoil multiplier"), target.moveKickbackMod);
				}
				EditorGUILayout.Separator();
				EditorGUILayout.EndVertical();
			}

			
		//Alt-Fire
		EditorGUILayout.BeginVertical("toolbar");
		target.altFireFoldout = EditorGUILayout.Foldout(target.altFireFoldout, "Alt-Fire Properties:");
		EditorGUILayout.EndVertical();
		
		if(target.altFireFoldout){
			EditorGUILayout.BeginVertical("textField");
				EditorGUILayout.Separator();
				target.isPrimaryWeapon = EditorGUILayout.Toggle(GUIContent("  Primary Weapon:  ", "Is this a primary weapon? Uncheck only if this gunscript is for an alt-fire"), target.isPrimaryWeapon);
				if(target.isPrimaryWeapon){
					target.secondaryWeapon = EditorGUILayout.ObjectField(GUIContent("  Secondary Weapon: ", "Optional alt-fire for weapon"), target.secondaryWeapon,  GunScript, true);
					if(target.secondaryWeapon != null){
						target.secondaryInterrupt = EditorGUILayout.Toggle(GUIContent("  Secondary Interrupt: ","Can you interrupt the firing animation to switch to alt-fire mode?"), target.secondaryInterrupt);
						target.secondaryFire = EditorGUILayout.Toggle(GUIContent("  Instant Fire: ","Does this alt-fire fire immediately? If unchecked, it will have to be switched to"), target.secondaryFire);
						if(target.secondaryFire == false){
							target.enterSecondaryTime = EditorGUILayout.FloatField(GUIContent("  Enter Secondary Time: ", "The time in seconds to transition to alt-fire mode"), target.enterSecondaryTime);
							target.exitSecondaryTime = EditorGUILayout.FloatField(GUIContent("  Exit Secondary Time: ", "The time in seconds to transition out of alt-fire mode"), target.exitSecondaryTime);
						}
					}
						
				}
				EditorGUILayout.Separator();
			EditorGUILayout.EndVertical();
		}
		
		//Ammo + Reload
		if(gunTipo != gunTypes.melee){
			EditorGUILayout.BeginVertical("toolbar");
			target.ammoReloadFoldout = EditorGUILayout.Foldout(target.ammoReloadFoldout, "Ammo + Reloading:");
			EditorGUILayout.EndVertical();
		}else{
			target.ammoReloadFoldout = false;
		}
		
		if(target.ammoReloadFoldout){
			EditorGUILayout.BeginVertical("textField");
				EditorGUILayout.Separator();
				if(gunTipo == gunTypes.hitscan || gunTipo == gunTypes.launcher){
					if(target.chargeWeapon){
						target.additionalAmmoPerCharge = EditorGUILayout.FloatField(GUIContent("  Ammo Cost Per Charge: ", "For each charge level, increase the ammo cost of firing the weapon by this amount. Works on integer intervals of charge level only."), target.additionalAmmoPerCharge);
					}
				}else if(gunTipo == gunTypes.melee){
					target.ammoPerClip = 1;
					target.infiniteAmmo = true;
					target.sharesAmmo = false; 
				}
				if(gunTipo != gunTypes.melee){
					if(!target.progressiveReload){
						target.ammoType = EditorGUILayout.EnumPopup(GUIContent("  Ammo Type: ","Does the ammo count refer to the number of clips remaining, or the number of individual shots remaining?"), target.ammoType);
					}
					target.ammoPerClip = EditorGUILayout.IntField(GUIContent("  Ammo Per Clip: ","The number of shots that can be fired before needing to reload"), target.ammoPerClip);
					if(gunTipo != gunTypes.spray){
						target.ammoPerShot = EditorGUILayout.IntField(GUIContent("  Ammo Used Per Shot: ","The amout of ammo used every time the gun is fired"), target.ammoPerShot);
					}else{
						target.ammoPerShot = EditorGUILayout.IntField(GUIContent("  Ammo Used Per Tick: ", "The amount of ammo drained every time ammo is drained. By default, ammo is drained once per second"), target.ammoPerShot);
						target.deltaTimeCoefficient = EditorGUILayout.FloatField(GUIContent("  Drain Coefficient: ", "By default, ammo is drained every second. The rate at which ammo is drained is multiplied by this value"), target.deltaTimeCoefficient);
					}
					target.sharesAmmo = EditorGUILayout.Toggle(GUIContent("  Shares Ammo:  ","If checked, this gun will be able to have a shared ammo reserve with one or more other weapons"), target.sharesAmmo);
					if(!target.sharesAmmo){
						target.infiniteAmmo = EditorGUILayout.Toggle(GUIContent("  Infinite Ammo: ", "If checked, this weapon will have infinite ammo"), target.infiniteAmmo);
						target.clips = EditorGUILayout.IntField(GUIContent("  Clips: ", "The amount of ammo for this weapon that the player has - either clips or bullets, depending on settings" ), target.clips);
						target.maxClips = EditorGUILayout.IntField(GUIContent("  Max Clips: ", "The maximum amount of ammo for this weapon that the player can carry"), target.maxClips);
					}else{
						target.managerObject = GameObject.FindWithTag("Manager");
						var popupContent = target.managerObject.GetComponent(AmmoManager).namesArray;
						var tempAmmoSet : int = target.ammoSetUsed;
						if(target.managerObject.GetComponent(AmmoManager).namesArray[0] == name){
							target.managerObject.GetComponent(AmmoManager).namesArray = target.managerObject.GetComponent(AmmoManager).tempNamesArray.ToBuiltin(String);
						}
						target.ammoSetUsed = EditorGUILayout.Popup("  Ammo Set Used:  ", tempAmmoSet, popupContent);
						target.managerObject.GetComponent(AmmoManager).namesArray[target.ammoSetUsed] = EditorGUILayout.TextField(GUIContent("  Rename Ammo Set:", "Type a new name for the ammo set"),target.managerObject.GetComponent(AmmoManager).namesArray[target.ammoSetUsed]);
						target.managerObject.GetComponent(AmmoManager).infiniteArray[target.ammoSetUsed] = EditorGUILayout.Toggle(GUIContent("  Infinite Ammo: ", "If checked, this set will have infinite ammo"), target.managerObject.GetComponent(AmmoManager).infiniteArray[target.ammoSetUsed]);
						target.managerObject.GetComponent(AmmoManager).clipsArray[target.ammoSetUsed] = EditorGUILayout.IntField(GUIContent("  Clips: ", "The amount of ammo the player has in this set - either clips or bullets, depending on settings"), target.managerObject.GetComponent(AmmoManager).clipsArray[target.ammoSetUsed]);		
						target.managerObject.GetComponent(AmmoManager).maxClipsArray[target.ammoSetUsed] = EditorGUILayout.IntField(GUIContent("  Max Clips: ", "The maximum amount of this type of ammo that the player can carry"), target.managerObject.GetComponent(AmmoManager).maxClipsArray[target.ammoSetUsed]);			
					}
					EditorGUILayout.Separator();
					target.reloadTime = EditorGUILayout.FloatField(GUIContent("  Reload Time: ","The time it takes to load the weapon if the user presses the reload key"), target.reloadTime);
					target.progressiveReset = EditorGUILayout.Toggle(GUIContent("  Clear Reload: ","If enabled, the gun will always start reloading at 0 rounds loaded, rather than the amount remaining in the clip"), target.progressiveReset);
					target.progressiveReload = EditorGUILayout.Toggle(GUIContent("  Progressive Reloading: ", "Do you reload this weapon one bullet/shell/whatever at a time?"), target.progressiveReload);
					if(target.progressiveReload){
						target.reloadInTime = EditorGUILayout.FloatField(GUIContent("  Enter Reload Time: ","The time it takes to start the reload cycle"), target.reloadInTime);
						target.reloadOutTime = EditorGUILayout.FloatField(GUIContent("  Exit Reload Time: ","The time it takes to exit the reload cycle"), target.reloadOutTime);
						target.addOneBullet = false;
					}
					if(!target.progressiveReload){
						target.addOneBullet = EditorGUILayout.Toggle(GUIContent("  Partial Reload Bonus: ","If enabled, the player will retain an additional round in the chamber when manually reloading"), target.addOneBullet);
					}
					//if(target.addOneBullet){
						target.emptyReloadTime = EditorGUILayout.FloatField(GUIContent("  Empty Reload Time:  ","The time it takes to reload the weapon if the user has completely emptied the weapon. This can be the same as the Reload Time"), target.emptyReloadTime);
					//} else {
					//	target.emptyReloadTime = target.reloadTime;
					//}
					target.waitforReload = EditorGUILayout.FloatField(GUIContent("  Wait For Reload: ","The time between pressing the reload key, and actually starting to reload"), target.waitforReload);
					EditorGUILayout.Separator();
			}
			EditorGUILayout.EndVertical();
		}
		
		//Audio/Visual
		EditorGUILayout.BeginVertical("toolbar");
		target.audioVisualFoldout = EditorGUILayout.Foldout(target.audioVisualFoldout, "Audio + Visual:");
		EditorGUILayout.EndVertical();
		
		if(target.audioVisualFoldout){
			EditorGUILayout.BeginVertical("textField");
				EditorGUILayout.Separator();
				if(gunTipo == gunTypes.hitscan || gunTipo == gunTypes.launcher){
					target.delay = EditorGUILayout.FloatField(GUIContent("  Delay: ","The delay between when the fire animation starts and when the gun actually fires"), target.delay);
					EditorGUILayout.Separator();
					if(gunTipo != gunTypes.launcher){
						target.tracer = EditorGUILayout.ObjectField(GUIContent("  Tracer: ","This optional field takes game object with a particle emitter to be used for tracer fire"), target.tracer,  GameObject, true);
						target.traceEvery = EditorGUILayout.IntField(GUIContent("  Shots Per Tracer: ","How many shots before displaying the tracer effect?"), target.traceEvery);
						target.simulateTime = EditorGUILayout.FloatField(GUIContent("  Simulate Tracer for: ", "The amount of time to simulate your tracer particle system before rendering the particles. Useful if your tracers have to start being emit from behind the muzzle of the gun"), target.simulateTime);
						EditorGUILayout.Separator();
					}
					if(gunTipo != gunTypes.hitscan){
						target.shellEjection = false;
					}else{
						target.shellEjection = EditorGUILayout.Toggle(GUIContent("  Shell Ejection: ","Does this weapon have shell ejection?"), target.shellEjection);
					}	
					if(target.shellEjection){ 
						EditorGUILayout.BeginVertical("textfield");
							target.shell = EditorGUILayout.ObjectField(GUIContent("  Shell: ","The GameObject to be instantiated when a shell is ejected"), target.shell, GameObject, false);
							target.ejectorPosition = EditorGUILayout.ObjectField(GUIContent("  Ejector Position: ","Shells will be instantiated from the position of this GameObject"), target.ejectorPosition,  GameObject, true);
							target.ejectDelay = EditorGUILayout.FloatField("Delay", target.ejectDelay);
						EditorGUILayout.EndVertical();
						EditorGUILayout.Separator();
					}	
				}
				target.sway = EditorGUILayout.Toggle(GUIContent("  Sway: ","Does this weapon use coded weapon sway?"), target.sway);
				if(target.sway){ 
					EditorGUILayout.BeginVertical("textField"); 
						target.overwriteSway = EditorGUILayout.Toggle(GUIContent("  Override Rates: ","Does this weapon override the global sway rates?"), target.overwriteSway);  
						
						if(target.overwriteSway)
						target.moveSwayRate = EditorGUILayout.Vector2Field("  Move Sway Rate:  ", target.moveSwayRate);
						target.moveSwayAmplitude = EditorGUILayout.Vector2Field("  Move Sway Amplitude:  ", target.moveSwayAmplitude); 
						
						if(target.overwriteSway)
						target.runSwayRate = EditorGUILayout.Vector2Field("  Run Sway Rate:  ", target.runSwayRate);
						target.runAmplitude = EditorGUILayout.Vector2Field("  Run Sway Amplitude:  ", target.runAmplitude); 
						
						if(target.overwriteSway)
						target.idleSwayRate = EditorGUILayout.Vector2Field("  Idle Sway Rate:  ", target.idleSwayRate);
						target.idleAmplitude = EditorGUILayout.Vector2Field("  Idle Sway Amplitude:  ", target.idleAmplitude);
					EditorGUILayout.EndVertical();
					EditorGUILayout.Separator();
				}
				
				target.useZKickBack = EditorGUILayout.Toggle(GUIContent("  Use Z KickBack: ","Does the gun move along the z axis when firing"), target.useZKickBack);
				if(target.useZKickBack){
					EditorGUILayout.BeginVertical("textfield");
						target.kickBackZ = EditorGUILayout.FloatField(GUIContent("  Z Kickback: ","The rate at which the gun moves backwards when firing"), target.kickBackZ);
						target.zRetRate = EditorGUILayout.FloatField(GUIContent("  Z Return: ","The rate at which the gun returns to position when not"), target.zRetRate);
						target.maxZ = EditorGUILayout.FloatField(GUIContent("  Z Max: ","The maximum amount the gun can kick back along the z axis"), target.maxZ);
					EditorGUILayout.EndVertical();
				}
				
				target.timeToIdle = EditorGUILayout.FloatField(GUIContent("  Idle Time: ", "The amount of time the player must be idle to start playing the idle animation"), target.timeToIdle);
				target.overrideAvoidance = EditorGUILayout.Toggle(GUIContent("  Override Avoidance: ","Does this weapon override the global object avoidance settings"), target.overrideAvoidance);
				if(target.overrideAvoidance){
					EditorGUILayout.BeginVertical("textField");
						target.avoids = EditorGUILayout.Toggle(GUIContent("  Use Avoidance: ","Does this weapon use object avoidance"), target.avoids);
						if(target.avoids){
							target.dist = EditorGUILayout.FloatField(GUIContent("  Avoid Start Dist: ","The distance from an object at which object avoidance will begin"), target.dist);
							target.minDist = EditorGUILayout.FloatField(GUIContent("  Avoid Closest Dist: ","The distance form an object at which avoidance will be maximized"), target.minDist);
							target.pos = EditorGUILayout.Vector3Field("  Avoid Position: ", target.pos);
							target.rot = EditorGUILayout.Vector3Field("  Avoid Rotation: ", target.rot);
						}
					EditorGUILayout.EndVertical();
				}
				/*
				var overrideAvoidance : boolean = false; //Does this weapon override global object avoidance values
				var avoids : boolean = true;
				var rot : Vector3;
				var pos : Vector3;
				var dist : float = 2;
				var minDist : float = 1.5;
				*/
				target.takeOutTime = EditorGUILayout.FloatField(GUIContent("  Take Out Time: ", "The time it takes to take out the weapon"), target.takeOutTime);
				target.putAwayTime = EditorGUILayout.FloatField(GUIContent("  Put Away Time: ", "The time it takes to put away the weapon"), target.putAwayTime);
				if(gunTipo == gunTypes.hitscan || gunTipo == gunTypes.launcher){
					if(!target.autoFire && !target.burstFire){
						target.fireAnim = EditorGUILayout.Toggle(GUIContent("  Morph Fire Anim to Fit: ","Maches the fire animation's speed to the time it takes to fire"), target.fireAnim);
					}
				}
				EditorGUILayout.Separator();
				target.fireSound = EditorGUILayout.ObjectField(GUIContent("  Fire Sound: ", "The sound to play when each shot is fired"), target.fireSound, AudioClip, false);
				if(gunTipo == gunTypes.spray){
					target.loopSound = EditorGUILayout.ObjectField(GUIContent( "  Looping Fire Sound: ", "The sound to loop while the weapon is firing"), target.loopSound, AudioClip, false);
					target.releaseSound = EditorGUILayout.ObjectField(GUIContent("  Stop Firing Sound: ", "The sound to play when the weapon stops firing"), target.releaseSound, AudioClip, false);
				}
				if(gunTipo == gunTypes.hitscan || gunTipo == gunTypes.launcher){
					if(target.chargeWeapon){
						target.chargeLoop = EditorGUILayout.ObjectField(GUIContent("  Charging Sound: ", "The sound to be looped while the weapon is charging"), target.chargeLoop, AudioClip, false);
					}
				}
				EditorGUILayout.LabelField(GUIContent("  Sound Pitch: ","The pitch to play the sound clip at. A value of 1 will play the sound at its natural pitch"));
				target.firePitch = EditorGUILayout.Slider(target.firePitch,-3, 3);
				EditorGUILayout.LabelField(GUIContent("   Sound Volume", "Volume of Fire Sound"));
				target.fireVolume = EditorGUILayout.Slider(target.fireVolume,0, 1);//EditorGUILayout.FloatField(GUIContent("  Sound Volume: ","The colume to play the sound clip at. A value of 1 will play the sound at max volume"), target.fireVolume);
				EditorGUILayout.Separator();
				
				if(gunTipo != gunTypes.melee){
					target.emptySound = EditorGUILayout.ObjectField(GUIContent("  Empty Sound: ", "The sound to play when sry firing"), target.emptySound, AudioClip, false);
					if(target.emptySound != null){
						EditorGUILayout.LabelField(GUIContent("  Sound Pitch: ","The pitch to play the sound clip at. A value of 1 will play the sound at its natural pitch"));
						target.emptyPitch = EditorGUILayout.Slider(target.emptyPitch,-3, 3);
						EditorGUILayout.LabelField(GUIContent("   Sound Volume", "Volume of Empty Sound"));
						target.emptyVolume = EditorGUILayout.Slider(target.emptyVolume,0, 1);//EditorGUILayout.FloatField(GUIContent("  Sound Volume: ","The colume to play the sound clip at. A value of 1 will play the sound at max volume"), target.fireVolume);
					}
				}
				
				EditorGUILayout.Separator();
				target.crosshairObj = EditorGUILayout.ObjectField(GUIContent("  Crosshair Plane: ", "Only use this if you are using a custom crosshair. Refer to documentation if needed"), target.crosshairObj, GameObject, true);
				if(target.crosshairObj != null){
					target.crosshairSize = EditorGUILayout.FloatField(GUIContent("  Crosshair Size: ", "The size of the default crosshair"), target.crosshairSize);
					target.scale = EditorGUILayout.Toggle(GUIContent("  Scale Crosshair: ", "Does the crosshair scale with accuracy? If disabled, the crosshair will always be a fixed size"), target.scale);
				}
				EditorGUILayout.Separator();
			EditorGUILayout.EndVertical();
		}
		
		
		EditorGUILayout.Separator();
			
//		EditorGUILayout.Separator();
		var tempText : String;
		if(target.gunDisplayed){
			tempText = "Deactivate Weapon";
		} else {
			tempText = "Activate Weapon";
		}
		if (GUILayout.Button(new GUIContent(tempText, "Toggle whether or not the gun is active"),"miniButton")){
			if(!target.gunDisplayed){
				target.gunDisplayed = true;
				target.EditorSelect();
			} else if (target.gunDisplayed){
				target.gunDisplayed = false;
				target.EditorDeselect();
			}
		}
		if (GUI.changed)
			EditorUtility.SetDirty(target);
		EditorGUILayout.Separator();
	}
}