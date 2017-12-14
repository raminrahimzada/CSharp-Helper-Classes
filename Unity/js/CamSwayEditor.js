/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/
@CustomEditor (CamSway)
class CamSwayEditor extends Editor {
	function OnInspectorGUI() {
	/*	if(target.CM == null){
			target.CM = target.gameObject.GetComponent("CharacterMotorDB");
		}*/
			EditorGUILayout.Separator();
			
			//Move Sway
			EditorGUILayout.BeginVertical("toolbar");
					EditorGUILayout.LabelField("Move Sway");
				EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical("textField");
				target.moveSwayRate = EditorGUILayout.Vector2Field("   Move Sway Rate: ", target.moveSwayRate);
				target.moveSwayAmplitude = EditorGUILayout.Vector2Field("   Move Sway Amplitude: ", target.moveSwayAmplitude);
			EditorGUILayout.EndVertical();
			
			//Run Sway
			EditorGUILayout.BeginVertical("toolbar");
					EditorGUILayout.LabelField("Run Sway");
				EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical("textField");
				target.runSwayRate = EditorGUILayout.Vector2Field("   Run Sway Rate: ", target.runSwayRate);
				target.runSwayAmplitude = EditorGUILayout.Vector2Field("   Run Sway Amplitude: ", target.runSwayAmplitude);
			EditorGUILayout.EndVertical();
			
			//Idle Sway
			EditorGUILayout.BeginVertical("toolbar");
					EditorGUILayout.LabelField("Idle Sway");
				EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical("textField");
				target.idleSwayRate = EditorGUILayout.Vector2Field("   Idle Sway Rate: ", target.idleSwayRate);
				target.idleAmplitude = EditorGUILayout.Vector2Field("   Idle Sway Amplitude: ", target.idleAmplitude);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();
			
		if (GUI.changed)
			EditorUtility.SetDirty(target);
	}
}