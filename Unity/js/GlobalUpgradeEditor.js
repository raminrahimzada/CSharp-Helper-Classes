//FPS Constructor - Weapons
//Copyright© Dastardly Banana Productions 2010
//This script, and all others contained within the Dastardly Banana Weapons Package, may not be shared or redistributed. They can be used in games, either commerical or non-commercial, as long as Dastardly Banana Productions is attributed in the credits.
//Permissions beyond the scope of this license may be available at mailto://info@dastardlybanana.com.

@CustomEditor (GlobalUpgrade)

class GlobalUpgradeEditor extends Editor {
	//var player : PlayerWeapons;
	function OnInspectorGUI () {
		EditorGUIUtility.LookLikeInspector();
		
		EditorGUILayout.Separator();
		target.upgrade = EditorGUILayout.ObjectField(GUIContent("  Upgrade: ","Upgrade object to be applied globally"), target.upgrade,  Upgrade, true);
		EditorGUILayout.Separator();
		EditorGUILayout.BeginVertical("textField");
		EditorGUILayout.LabelField("Applicable Classes");
		EditorGUILayout.Separator();
		
		var ws : weaponClasses[] = weaponClasses.GetValues(weaponClasses);
		
		if(target.classesAllowed == null)
			UpdateArray();
		
		if(target.classesAllowed.length < ws.length)
			UpdateArray();
		
		for (var i : int = 0; i < ws.length; i++) {
			var w : weaponClasses = ws[i];
			if(w == weaponClasses.Null) break;
			var className = w.ToString().Replace("_", " " );
			target.classesAllowed[i] = EditorGUILayout.Toggle(className, target.classesAllowed[i]);
		}
		EditorGUILayout.Separator();
		
		EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Enable All", EditorStyles.miniButtonLeft)){
				for(i = 0; i < target.classesAllowed.length; i++){
					target.classesAllowed[i] = true;
				}
			}
			if(GUILayout.Button("Disable All", EditorStyles.miniButtonRight)){
				for(i = 0; i < target.classesAllowed.length; i++){
					target.classesAllowed[i] = false;
				}
			}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		EditorGUILayout.EndVertical();
	
	}
	
	function UpdateArray () {
		var tempArray = new Array(target.classesAllowed);
		var tempArray2 : boolean[] = tempArray.ToBuiltin(boolean) as boolean[];
		
		target.classesAllowed = new boolean[weaponClasses.GetValues(weaponClasses).length];
		for(var i : int = 0; i < tempArray2.length; i++){
			target.calssesAllowed[i] = tempArray2[i];
		}
		
	if (GUI.changed)
		EditorUtility.SetDirty(target);
	}
}
