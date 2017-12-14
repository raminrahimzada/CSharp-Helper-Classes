/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

@CustomEditor (AimMode)
class AimModeEditor extends Editor {
	var foldout : boolean;
	var foldout1 : boolean;

 	
	function OnInspectorGUI() {
		
		EditorGUIUtility.LookLikeInspector();
		EditorGUILayout.BeginVertical();
		
		target.aim = EditorGUILayout.Toggle("  Weapon Aims: ", target.aim);
		
		if(target.aim){
			if(!target.sightsZoom1)
				target.scoped1 = EditorGUILayout.Toggle("  Has Scope: ", target.scoped1);
			if(!target.scoped1){
				target.sightsZoom1 = EditorGUILayout.Toggle("  Zoom Down Sights: ", target.sightsZoom1);
				target.crosshairWhenAiming = EditorGUILayout.Toggle("  Show Crosshair: ", target.crosshairWhenAiming);
			} else {
				target.scopeTexture = EditorGUILayout.ObjectField("  Scope Texture: ", target.scopeTexture, Texture, false);
				target.st169 = EditorGUILayout.ObjectField("  Scope Texture 16:9: ", target.st169, Texture, false);
				target.st1610 = EditorGUILayout.ObjectField("  Scope Texture 16:10: ", target.st1610, Texture, false);
				target.st54 = EditorGUILayout.ObjectField("  Scope Texture 5:4: ", target.st54, Texture, false);
				target.st43 = EditorGUILayout.ObjectField("  Scope Texture 4:3: ", target.st43, Texture, false);

			}
			if(target.scoped1 || target.sightsZoom1)
				target.zoomFactor1 = EditorGUILayout.FloatField("  Zoom Factor: ", target.zoomFactor1);
		}
		target.aimRate = EditorGUILayout.FloatField("  Aim Rate: ", target.aimRate);
		target.sprintRate = EditorGUILayout.FloatField("  Sprint Rate: ", target.sprintRate);
		target.retRate = EditorGUILayout.FloatField("  Return Rate: ", target.retRate);

		EditorGUILayout.Separator();  
		
		target.overrideSprint = EditorGUILayout.Toggle("  Override Sprint: ", target.overrideSprint);
		if(target.overrideSprint){
			target.sprintDuration = EditorGUILayout.FloatField("  Sprint Duration: ", target.sprintDuration);
			target.sprintAddStand = EditorGUILayout.FloatField("  Standing Sprint Return Rate: ", target.sprintAddStand);
			target.sprintAddWalk = EditorGUILayout.FloatField("  Walking Sprint Return Rate: ", target.sprintAddWalk);
			target.sprintMin = EditorGUILayout.FloatField("  Sprint Minimum: ", target.sprintMin);
			target.recoverDelay = EditorGUILayout.FloatField("  Sprint Recovery Delay: ", target.recoverDelay);
			target.exhaustedDelay = EditorGUILayout.FloatField("  Exhausted Recovery Delay: ", target.exhaustedDelay);
		}

		EditorGUILayout.Separator();  
	
		target.hasSecondary = EditorGUILayout.Toggle("  Has Secondary: ", target.hasSecondary);
		target.secondaryAim = EditorGUILayout.Toggle("  Weapon Aims: ", target.secondaryAim);
		
		if(target.secondaryAim && target.hasSecondary){
			if(!target.sightsZoom2)
				target.scoped2 = EditorGUILayout.Toggle("  Has Scope: ", target.scoped2);
			if(!target.scoped2)
				target.sightsZoom2 = EditorGUILayout.Toggle("  Zoom Down Sights: ", target.sightsZoom2);
			if(target.scoped2 || target.sightsZoom2)
				target.zoomFactor2 = EditorGUILayout.FloatField("  Zoom Factor: ", target.zoomFactor2);
		}


		EditorGUILayout.EndVertical();

		EditorGUILayout.Separator();  
		foldout = EditorGUILayout.Foldout(foldout, "Configure Primary Weapon Positions");
			if(foldout) {
				EditorGUILayout.BeginVertical();
				EditorGUILayout.BeginHorizontal();
				
					if(GUILayout.Button(new GUIContent("Move to Hip Position", "Move Weapon to Hip Position"),"miniButton")) {
					target.transform.localPosition = target.hipPosition1;
					target.transform.localEulerAngles = target.hipRotation1;
				}
				if(GUILayout.Button(new GUIContent("Configure Hip Position", "Set Hip Position to Current Position"),"miniButton")) {
					target.hipPosition1 = target.transform.localPosition;
					target.hipRotation1 = target.transform.localEulerAngles;
					EditorUtility.SetDirty (target);
				}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginVertical("textField");
			target.hipPosition1 = EditorGUILayout.Vector3Field("hipPosition", target.hipPosition1);
			target.hipRotation1 = EditorGUILayout.Vector3Field("hipRotation", target.hipRotation1);
			EditorGUILayout.EndVertical();
			
		
			
			EditorGUILayout.Separator();  
		
			
			if(target.aim){
				EditorGUILayout.BeginHorizontal();
					if(GUILayout.Button(new GUIContent("Move to Aim Position", "Move Weapon to Aim Position"),"miniButton")) {
						target.transform.localPosition = target.aimPosition1;
						target.transform.localEulerAngles = target.aimRotation1;
					}
						if(GUILayout.Button(new GUIContent("Configure Aim Position", "Set Aim Position to Current Position"),"miniButton")) {
						target.aimPosition1 = target.transform.localPosition;
						target.aimRotation1 = target.transform.localEulerAngles;
						EditorUtility.SetDirty (target);
					}
				EditorGUILayout.EndHorizontal();
		
			EditorGUILayout.BeginVertical("textField");
			target.aimPosition1 = EditorGUILayout.Vector3Field("aimPosition", target.aimPosition1);
			target.aimRotation1 = EditorGUILayout.Vector3Field("aimRotation", target.aimRotation1);
			EditorGUILayout.EndVertical();
			}



			EditorGUILayout.Separator(); 



			EditorGUILayout.BeginHorizontal();
		
				if(GUILayout.Button(new GUIContent("Move to Sprint Position", "Move Weapon to Sprint Position"),"miniButton")) {
					target.transform.localPosition = target.sprintPosition1;
					target.transform.localEulerAngles = target.sprintRotation1;
				}
					if(GUILayout.Button(new GUIContent("Configure Sprint Position", "Set Sprint Position to Position"),"miniButton")) {
					target.sprintPosition1 = target.transform.localPosition;
					target.sprintRotation1 = target.transform.localEulerAngles;
					EditorUtility.SetDirty (target);
				}
			EditorGUILayout.EndHorizontal();
		
			EditorGUILayout.BeginVertical("textField");
			target.sprintPosition1 = EditorGUILayout.Vector3Field("sprintPosition", target.sprintPosition1);
			target.sprintRotation1 = EditorGUILayout.Vector3Field("sprintRotation", target.sprintRotation1);
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndVertical();
		}
		
		///****************************
		///****************************

		if(target.hasSecondary){
		foldout1 = EditorGUILayout.Foldout(foldout1, "Configure Secondary Weapon Positions");
		if(foldout1) {
			EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal();
			
				if(GUILayout.Button(new GUIContent("Move to Hip Position", "Move Weapon to Hip Position"),"miniButton")) {
					target.transform.localPosition = target.hipPosition2;
					target.transform.localEulerAngles = target.hipRotation2;
				}
				if(GUILayout.Button(new GUIContent("Configure Hip Position", "Set Hip Position to Current Position"),"miniButton")) {
					target.hipPosition2 = target.transform.localPosition;
					target.hipRotation2 = target.transform.localEulerAngles;
					EditorUtility.SetDirty (target);
				}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginVertical("textField");
			target.hipPosition2 = EditorGUILayout.Vector3Field("hipPosition", target.hipPosition2);
			target.hipRotation2 = EditorGUILayout.Vector3Field("hipRotation", target.hipRotation2);
			EditorGUILayout.EndVertical();
			
		
			
			EditorGUILayout.Separator();  
		
			
			if(target.secondaryAim){
				EditorGUILayout.BeginHorizontal();
		
				if(GUILayout.Button(new GUIContent("Move to Aim Position", "Move Weapon to Aim Position"),"miniButton")) {
					target.transform.localPosition = target.aimPosition2;
					target.transform.localEulerAngles = target.aimRotation2;
				}
					if(GUILayout.Button(new GUIContent("Configure Aim Position", "Set Aim Position to Current Position"),"miniButton")) {
					target.aimPosition2 = target.transform.localPosition;
					target.aimRotation2 = target.transform.localEulerAngles;
					EditorUtility.SetDirty (target);
				}
				EditorGUILayout.EndHorizontal();
		
			EditorGUILayout.BeginVertical("textField");
			target.aimPosition2 = EditorGUILayout.Vector3Field("aimPosition", target.aimPosition2);
			target.aimRotation2 = EditorGUILayout.Vector3Field("aimRotation", target.aimRotation2);
			EditorGUILayout.EndVertical();
			}

			EditorGUILayout.Separator(); 

			EditorGUILayout.BeginHorizontal();
		
				if(GUILayout.Button(new GUIContent("Move to Sprint Position", "Move Weapon to Sprint Position"),"miniButton")) {
					target.transform.localPosition = target.sprintPosition2;
					target.transform.localEulerAngles = target.sprintRotation2;
				}
					if(GUILayout.Button(new GUIContent("Configure Sprint Position", "Set Sprint Position to Position"),"miniButton")) {
					target.sprintPosition2 = target.transform.localPosition;
					target.sprintRotation2 = target.transform.localEulerAngles;
					EditorUtility.SetDirty (target);
				}
			EditorGUILayout.EndHorizontal();
		
			EditorGUILayout.BeginVertical("textField");
			target.sprintPosition2 = EditorGUILayout.Vector3Field("sprintPosition", target.sprintPosition2);
			target.sprintRotation2 = EditorGUILayout.Vector3Field("sprintRotation", target.sprintRotation2);
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndVertical();
		}
		}
    	if (GUI.changed)
			EditorUtility.SetDirty (target);
    }
}

