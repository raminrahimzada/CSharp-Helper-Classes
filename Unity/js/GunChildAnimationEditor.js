/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

@CustomEditor (GunChildAnimation)
class GunChildAnimationEditor extends Editor {
	
	function OnInspectorGUI() {
		EditorGUIUtility.LookLikeInspector();
		EditorGUILayout.BeginVertical();
		EditorGUILayout.Separator();
		target.gs = EditorGUILayout.ObjectField(GUIContent("  Gun Script: ", "This is the GunScript of this weapon"), target.gs,  GunScript, true);
		target.hasSecondary = EditorGUILayout.Toggle("  Has Secondary: ", target.hasSecondary);
		target.melee = EditorGUILayout.Toggle("  Is Melee: ", target.melee);
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Animations", "Animation Name");
		EditorGUILayout.Separator();
		if(!target.melee){
			target.reloadAnim = EditorGUILayout.TextField("  Reload: ", target.reloadAnim);
			target.emptyReloadAnim = EditorGUILayout.TextField("  Empty Reload: ", target.emptyReloadAnim);
			target.fireAnim = EditorGUILayout.TextField("  Fire: ", target.fireAnim);
			target.emptyFireAnim = EditorGUILayout.TextField("  Dry Fire: ", target.emptyFireAnim);
		}
		target.takeOutAnim = EditorGUILayout.TextField("  Take Out: ", target.takeOutAnim);
		target.putAwayAnim = EditorGUILayout.TextField("  Put Away: ", target.putAwayAnim);
		
		if(!target.melee){
			target.reloadIn = EditorGUILayout.TextField("  Enter Reload: ", target.reloadIn);
			target.reloadOut = EditorGUILayout.TextField("  Exit Reload: ", target.reloadOut);
		}
		target.walkAnimation = EditorGUILayout.TextField("  Walk: ", target.walkAnimation);
		
		target.sprintAnimation = EditorGUILayout.TextField("  Sprint: ", target.sprintAnimation);
		
		target.walkSpeedModifier = EditorGUILayout.FloatField("  Walk Speed Modifier: ", target.walkSpeedModifier);
		target.walkWhenAiming = EditorGUILayout.Toggle("  Walk When Aiming: ", target.walkWhenAiming);
		target.nullAnim = EditorGUILayout.TextField("  Null: ", target.nullAnim);
		
		target.idleAnim = EditorGUILayout.TextField("  Idle: ", target.idleAnim);
		
		if(target.hasSecondary){
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Secondary Animations", "Animation Name");
			EditorGUILayout.Separator();
			target.secondaryReloadAnim = EditorGUILayout.TextField("  Reload: ", target.secondaryReloadAnim);
			target.secondaryReloadEmpty = EditorGUILayout.TextField("  Empty Reload: ", target.secondaryReloadEmpty);
			target.secondaryFireAnim = EditorGUILayout.TextField("  Fire: ", target.secondaryFireAnim);
			target.secondaryEmptyFireAnim = EditorGUILayout.TextField("  Dry Fire: ", target.secondaryEmptyFireAnim);
			target.enterSecondaryAnim = EditorGUILayout.TextField("  Enter Secondary: ", target.enterSecondaryAnim);
			target.exitSecondaryAnim = EditorGUILayout.TextField("  Exit Secondary: ", target.exitSecondaryAnim);
			target.secondaryWalkAnim = EditorGUILayout.TextField("  Walk: ", target.secondaryWalkAnim);
			target.secondarySprintAnim = EditorGUILayout.TextField("  Sprint: ", target.secondarySprintAnim);
			target.secondaryNullAnim = EditorGUILayout.TextField("  Null: ", target.secondaryNullAnim);
			target.secondaryIdleAnim = EditorGUILayout.TextField("  Idle: ", target.secondaryIdleAnim);
		}
		if(target.melee){
			EditorGUILayout.Separator();
			target.animCount = EditorGUILayout.IntField("  Animations: ", target.animCount);
			target.random = EditorGUILayout.Toggle("  Random: ", target.random);
			if(!target.random){
				target.resetTime = EditorGUILayout.FloatField("  Chain Reset Time: ", target.resetTime);
			}
			for(var i = 0; i < target.animCount; i++){
				target.fireAnims[i] = EditorGUILayout.TextField("    Attack: ", target.fireAnims[i]);
				target.reloadAnims[i] = EditorGUILayout.TextField("    Return: ", target.reloadAnims[i]);
				EditorGUILayout.Separator();
			}
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.Separator();
		if (GUI.changed)
			EditorUtility.SetDirty(target);
	}

}