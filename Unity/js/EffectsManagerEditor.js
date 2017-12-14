/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

@CustomEditor (EffectsManager)
class EffectsManagerEditor extends Editor {
	
	var nameArray : String[] = new String [1];
	
	function OnEnable() {
		if (target.highestSet == 0) {
			target.CreateSet();
		}
		updateNameArray();
	}
	
	function updateNameArray() {
		nameArray = new String[target.setNameArray.length + 1];
		for (i=0; i< nameArray.length-1 ; i++) {
			nameArray[i] = target.setNameArray[i];
		}
			nameArray[nameArray.length-1] = "New Set";
	}
	function OnInspectorGUI() {		
		var markForDelete : boolean = false;
			
		EditorGUIUtility.LookLikeInspector();
		EditorGUILayout.BeginVertical();
		target.maxDecals = EditorGUILayout.IntField("  Max. Decals in World: ", target.maxDecals);
		

		
		if(target.highestSet > 0){
			target.selectedSet = EditorGUILayout.Popup("  Select Set to Edit: ", target.selectedSet, nameArray);
			if(target.selectedSet == nameArray.length -1) {
				if(target.highestSet < target.maxSets){
					target.CreateSet();
					updateNameArray();
				}else{
					Debug.LogWarning("Effects Set not created - too many decal sets exist already!");
				}			
			}
			var setName : String = target.setNameArray[target.selectedSet];
			
			//EditorGUILayout.Separator();
			setName = EditorGUILayout.TextField("  Effects Set Name:  ", setName);
			if( target.setNameArray[target.selectedSet] != setName) {
				target.Rename(setName);
				updateNameArray();
			}
			
			EditorGUILayout.Separator();
			if(GUILayout.Button("Delete Set")) {
				markForDelete = true;
			}
			
			EditorGUILayout.Separator();
		
			
			if(target.setArray.length > 0){
				//Bullet Decals
				target.setArray[target.selectedSet].bulletDecalsFolded = EditorGUILayout.Foldout(target.setArray[target.selectedSet].bulletDecalsFolded, "  Bullet Decals: ");
				if(target.setArray[target.selectedSet].bulletDecalsFolded){
					for(var i : int = 0; i<target.setArray[target.selectedSet].bulletDecals.length; i++){
						target.setArray[target.selectedSet].bulletDecals[i] = EditorGUILayout.ObjectField("  Bullet Decal: ", target.setArray[target.selectedSet].bulletDecals[i], GameObject, false);
						if(target.setArray[target.selectedSet].bulletDecals[i] == null){
							if( i < target.setArray[target.selectedSet].bulletDecals.length){
								if(target.setArray[target.selectedSet].bulletDecals[i + 1] == null){
									target.setArray[target.selectedSet].lastBulletDecal = i;
									break;
								}else{
									for(var m : int = i+1; m<target.setArray[target.selectedSet].bulletDecals.length; m++){
										if(target.setArray[target.selectedSet].bulletDecals[m] == null){
											target.setArray[target.selectedSet].bulletDecals[m-1] = target.setArray[target.selectedSet].bulletDecals[m];
											break;
										}else{
											target.setArray[target.selectedSet].bulletDecals[m-1] = target.setArray[target.selectedSet].bulletDecals[m];
										}
									}
								}
							}
						}
					}
				}
			
				//Dent Decals
				target.setArray[target.selectedSet].dentDecalsFolded = EditorGUILayout.Foldout(target.setArray[target.selectedSet].dentDecalsFolded, "  Dent Decals: ");
			if(target.setArray[target.selectedSet].dentDecalsFolded){
					for(i = 0; i<target.setArray[target.selectedSet].dentDecals.length; i++){
						target.setArray[target.selectedSet].dentDecals[i] = EditorGUILayout.ObjectField("  Dent Decal: ", target.setArray[target.selectedSet].dentDecals[i], GameObject, false);
						if(target.setArray[target.selectedSet].dentDecals[i] == null){
							if( i < target.setArray[target.selectedSet].dentDecals.length){
								if(target.setArray[target.selectedSet].dentDecals[i + 1] == null){
									target.setArray[target.selectedSet].lastDentDecal = i;
									break;
								}else{
									for(m = i+1; m<target.setArray[target.selectedSet].dentDecals.length; m++){
										if(target.setArray[target.selectedSet].dentDecals[m] == null){
											target.setArray[target.selectedSet].dentDecals[m-1] = target.setArray[target.selectedSet].dentDecals[m];
											break;
										}else{
											target.setArray[target.selectedSet].dentDecals[m-1] = target.setArray[target.selectedSet].dentDecals[m];
										}
									}
								}
							}
						}
					}
				}
		
				//Hit Particles
				target.setArray[target.selectedSet].hitParticlesFolded = EditorGUILayout.Foldout(target.setArray[target.selectedSet].hitParticlesFolded, "  Hit Particles: ");
			if(target.setArray[target.selectedSet].hitParticlesFolded){
					for(i = 0; i<target.setArray[target.selectedSet].hitParticles.length; i++){
						target.setArray[target.selectedSet].hitParticles[i] = EditorGUILayout.ObjectField("  Hit Particle: ", target.setArray[target.selectedSet].hitParticles[i], GameObject, false);
						if(target.setArray[target.selectedSet].hitParticles[i] == null){
							if( i < target.setArray[target.selectedSet].hitParticles.length){
								if(target.setArray[target.selectedSet].hitParticles[i + 1] == null){
									target.setArray[target.selectedSet].lastHitParticle = i;
									break;
								}else{
									for(m = i+1; m<target.setArray[target.selectedSet].hitParticles.length; m++){
										if(target.setArray[target.selectedSet].hitParticles[m] == null){
											target.setArray[target.selectedSet].hitParticles[m-1] = target.setArray[target.selectedSet].hitParticles[m];
											break;
										}else{
											target.setArray[target.selectedSet].hitParticles[m-1] = target.setArray[target.selectedSet].hitParticles[m];
										}
									}
								}
							}
						}
					}
				}

				//Bullet Sounds
				target.setArray[target.selectedSet].bulletSoundsFolded = EditorGUILayout.Foldout(target.setArray[target.selectedSet].bulletSoundsFolded, "  Bullet Sounds: ");
				if(target.setArray[target.selectedSet].bulletSoundsFolded){
					for(i = 0; i<target.setArray[target.selectedSet].bulletSounds.length; i++){
						target.setArray[target.selectedSet].bulletSounds[i] = EditorGUILayout.ObjectField("  Bullet Sound: ", target.setArray[target.selectedSet].bulletSounds[i], AudioClip, false);
						if(target.setArray[target.selectedSet].bulletSounds[i] == null){
							if( i < target.setArray[target.selectedSet].bulletSounds.length){
								if(target.setArray[target.selectedSet].bulletSounds[i + 1] == null){
									target.setArray[target.selectedSet].lastBulletSound = i;
									break;
								}else{
									for(m = i+1; m<target.setArray[target.selectedSet].bulletSounds.length; m++){
										if(target.setArray[target.selectedSet].bulletSounds[m] == null){
											target.setArray[target.selectedSet].bulletSounds[m-1] = target.setArray[target.selectedSet].bulletSounds[m];
											break;
										}else{
											target.setArray[target.selectedSet].bulletSounds[m-1] = target.setArray[target.selectedSet].bulletSounds[m];
										}
									}
								}
							}
						}
					}
				}

				//Collision Sounds
				target.setArray[target.selectedSet].collisionSoundsFolded = EditorGUILayout.Foldout(target.setArray[target.selectedSet].collisionSoundsFolded, "  Collision Sounds: ");
				if(target.setArray[target.selectedSet].collisionSoundsFolded){
					for(i = 0; i<target.setArray[target.selectedSet].collisionSounds.length; i++){
						target.setArray[target.selectedSet].collisionSounds[i] = EditorGUILayout.ObjectField("  Collision Sound: ", target.setArray[target.selectedSet].collisionSounds[i], AudioClip, false);
						if(target.setArray[target.selectedSet].collisionSounds[i] == null){
							if( i < target.setArray[target.selectedSet].collisionSounds.length){
								if(target.setArray[target.selectedSet].collisionSounds[i + 1] == null){
									target.setArray[target.selectedSet].lastCollisionSound = i;
									break;
								}else{
									for(m = i+1; m<target.setArray[target.selectedSet].collisionSounds.length; m++){
										if(target.setArray[target.selectedSet].collisionSounds[m] == null){
											target.setArray[target.selectedSet].collisionSounds[m-1] = target.setArray[target.selectedSet].collisionSounds[m];
											break;
										}else{
											target.setArray[target.selectedSet].collisionSounds[m-1] = target.setArray[target.selectedSet].collisionSounds[m];
										}
									}
								}
							}
						}
					}
				}
		
				//Footstep Sounds
				target.setArray[target.selectedSet].footstepSoundsFolded = EditorGUILayout.Foldout(target.setArray[target.selectedSet].footstepSoundsFolded, "  Footstep Sounds: ");
				if(target.setArray[target.selectedSet].footstepSoundsFolded){
					for(i = 0; i<target.setArray[target.selectedSet].footstepSounds.length; i++){
						target.setArray[target.selectedSet].footstepSounds[i] = EditorGUILayout.ObjectField("  Footstep Sound: ", target.setArray[target.selectedSet].footstepSounds[i], AudioClip, false);
						if(target.setArray[target.selectedSet].footstepSounds[i] == null){
							if( i < target.setArray[target.selectedSet].footstepSounds.length){
								if(target.setArray[target.selectedSet].footstepSounds[i + 1] == null){
									target.setArray[target.selectedSet].lastFootstepSound = i;
									break;
								}else{
									for(m = i+1; m<target.setArray[target.selectedSet].footstepSounds.length; m++){
										if(target.setArray[target.selectedSet].footstepSounds[m] == null){
											target.setArray[target.selectedSet].footstepSounds[m-1] = target.setArray[target.selectedSet].footstepSounds[m];
											break;
										}else{
											target.setArray[target.selectedSet].footstepSounds[m-1] = target.setArray[target.selectedSet].footstepSounds[m];
										}
									}
								}
							}
						}
					}
				}
				
				//Crawl Sounds	
				target.setArray[target.selectedSet].crawlSoundsFolded = EditorGUILayout.Foldout(target.setArray[target.selectedSet].crawlSoundsFolded, "  Crawl (Prone) Sounds: ");
				if(target.setArray[target.selectedSet].crawlSoundsFolded){
					for(i = 0; i<target.setArray[target.selectedSet].crawlSounds.length; i++){
						target.setArray[target.selectedSet].crawlSounds[i] = EditorGUILayout.ObjectField("  Crawl Sound: ", target.setArray[target.selectedSet].crawlSounds[i], AudioClip, false);
						if(target.setArray[target.selectedSet].crawlSounds[i] == null){
							if( i < target.setArray[target.selectedSet].crawlSounds.length){
								if(target.setArray[target.selectedSet].crawlSounds[i + 1] == null){
									target.setArray[target.selectedSet].lastCrawlSound = i;
									break;
								}else{
									for(m = i+1; m<target.setArray[target.selectedSet].crawlSounds.length; m++){
										if(target.setArray[target.selectedSet].crawlSounds[m] == null){
											target.setArray[target.selectedSet].crawlSounds[m-1] = target.setArray[target.selectedSet].crawlSounds[m];
											break;
										}else{
											target.setArray[target.selectedSet].crawlSounds[m-1] = target.setArray[target.selectedSet].crawlSounds[m];
										}
									}
								}
							}
						}
					}
				}
				/*	var crawlSounds : AudioClip[] = new AudioClip[maxOfEach];
	var crawlSoundsFolded : boolean = false;
	var lastCrawlSound : int = 0;*/
			}
		}
		if(markForDelete) {	
			if(target.highestSet == 1) {
				Debug.Log("Can't Delete Last Effects Set");
			} else {	
				target.DeleteSet(target.selectedSet);
				updateNameArray();
				target.selectedSet = 0;
			}
		}
		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}

}